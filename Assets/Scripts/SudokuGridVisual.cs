using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SudokuGridVisual : MonoBehaviour
{
    public TMP_FontAsset playerNumberFont;
    [Space]
    public SudokuCellVisual[] cells;

    public SudokuCellVisual[,] visualGrid { get; private set; }

    public SudokuGrid sudokuGrid;

    private void Awake()
    {
        visualGrid = new SudokuCellVisual[9, 9];

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                visualGrid[i, j] = cells[i * 9 + j];

                visualGrid[i, j].gridPosition = new Vector2Int(i, j);
            }
    }

    public IEnumerable<SudokuCellVisual> GetCellVisuals()
    {
        foreach (SudokuCellVisual cell in visualGrid)
            yield return cell;
    }

    public void Init(SudokuGrid grid)
    {
        sudokuGrid = grid;

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                SudokuCellVisual cellVisual = visualGrid[i, j];
                SudokuCell cell = sudokuGrid.grid[i, j];

                cell.onSetNumber += OnCellSetNumber;

                cellVisual.Init(cell);
            }
    }

    private void OnCellSetNumber(SudokuCell sudokuCell, byte number)
    {
        SudokuHelp.Clear();

        bool sudokuGridIsCompleated = sudokuGrid.GetCells().All(cell => cell.number != null);

        if (sudokuGridIsCompleated == true)
        {
            List<SudokuCellVisual> incorrectCells = GetIncorrectCells();

            if (incorrectCells.Count == 0)
            {
                StartCoroutine(CompleatedAnimation(GetCellVisuals().ToList()));
                GameManager.instance.Invoke(nameof(GameManager.instance.BackToMenu), 3);
            }
            else
                StartCoroutine(RemoveIncorrectCells(incorrectCells));

            return;
        }

        List<SudokuCellVisual> compleatedCells = new List<SudokuCellVisual>();
        IEnumerable<SudokuCell> rowCells = sudokuGrid.GetCellsInRow(sudokuCell.gridPosition);
        IEnumerable<SudokuCell> columnCells = sudokuGrid.GetCellsInColumn(sudokuCell.gridPosition);
        IEnumerable<SudokuCell> boxCells = sudokuGrid.GetCellsInBox(sudokuCell.gridPosition);

        if (rowCells.All(cell => cell.number != null))
            compleatedCells.AddRange(rowCells.Select(cell => visualGrid[cell.gridPosition.x, cell.gridPosition.y]));
        if (columnCells.All(cell => cell.number != null))
            compleatedCells.AddRange(columnCells.Select(cell => visualGrid[cell.gridPosition.x, cell.gridPosition.y]));
        if (boxCells.All(cell => cell.number != null))
            compleatedCells.AddRange(boxCells.Select(cell => visualGrid[cell.gridPosition.x, cell.gridPosition.y]));

        if (compleatedCells.Count > 0)
        {
            StartCoroutine(CompleatedAnimation(compleatedCells));
            return;
        }

        visualGrid[sudokuCell.gridPosition.x, sudokuCell.gridPosition.y].GoodNumber();
    }

    private IEnumerator CompleatedAnimation(List<SudokuCellVisual> compleatedCells)
    {
        List<List<SudokuCellVisual>> cellsList = new List<List<SudokuCellVisual>>(17);

        for (int i = 0; i < 17; i++)
            cellsList.Add(new List<SudokuCellVisual>());

        foreach (SudokuCellVisual cell in compleatedCells)
        {
            int index = cell.gridPosition.x + cell.gridPosition.y;

            cellsList[index].Add(cell);
        }

        cellsList.RemoveAll(l => l.Count() == 0);

        foreach (List<SudokuCellVisual> cells in cellsList)
        {
            foreach (SudokuCellVisual cell in cells)
                cell.GoodNumber();

            yield return new WaitForSeconds(0.06f);
        }
    }

    public void Help()
    {
        List<SudokuCellVisual> incorrectCells = GetIncorrectCells();

        if (incorrectCells.Count > 0)
        {
            StartCoroutine(RemoveIncorrectCells(incorrectCells));
            return;
        }

        if (SudokuHelp.currentSolvingTechnique != null)
        {
            if (SudokuHelp.currentSolvingTechnique.isAnimationPlaying)
            {
                SudokuHelp.currentSolvingTechnique.skipAnimation = true;
                return;
            }

            SudokuHelp.currentSolvingTechnique.Apply();
            return;
        }

        SolvingTechnique[] solvingTechniques = {
            new LastDigit(sudokuGrid),
            new FullHouse(sudokuGrid),
            new HiddenSingle(sudokuGrid),
            new NakedSingle(sudokuGrid)
        };
        solvingTechniques = solvingTechniques.OrderBy(t => t.difficulty).ToArray();

        sudokuGrid.RecalculateCandidateNumbersForAllCells();

        foreach (SolvingTechnique solvingTechnique in solvingTechniques)
            if (solvingTechnique.Find() == true)
            {
                Debug.Log(solvingTechnique.GetType());
                StartCoroutine(solvingTechnique.DisplayHelp());

                return;
            }
    }

    private List<SudokuCellVisual> GetIncorrectCells()
    {
        List<SudokuCellVisual> incorrectCells = new List<SudokuCellVisual>();

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                SudokuCellVisual cellVisual = visualGrid[i, j];
                SudokuCell cell = cellVisual.sudokuCell;

                if (cell.number == null)
                    continue;

                if (cell.number == cell.expectedNumber)
                    continue;

                incorrectCells.Add(cellVisual);
            }

        return incorrectCells;
    }

    private IEnumerator RemoveIncorrectCells(List<SudokuCellVisual> IncorrectCells)
    {
        foreach (SudokuCellVisual incorrectCell in IncorrectCells.OrderBy(p => UnityEngine.Random.value))
        {
            incorrectCell.BadNumber();

            yield return new WaitForSeconds(0.4f);
        }
    }
}
