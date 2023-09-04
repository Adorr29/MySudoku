using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuGrid
{
    public SudokuCell[,] grid { get; private set; } = new SudokuCell[9, 9];

    public Action<SudokuCellVisual, byte?> onSetNumber;

    public static SudokuGrid CreateEmptyGrid()
    {
        SudokuGrid grid = new SudokuGrid();

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                grid.grid[i, j] = new SudokuCell();

                grid.grid[i, j].sudokuGrid = grid;
                grid.grid[i, j].gridPosition = new Vector2Int(i, j);

                grid.grid[i, j].SetAllCandidateNumbers();
            }

        return grid;
    }

    public SudokuGrid CreateCopy()
    {
        SudokuGrid grid = new SudokuGrid();

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                grid.grid[i, j] = this.grid[i, j].CreateCopy();
                grid.grid[i, j].sudokuGrid = grid;
            }

        return grid;
    }

    public IEnumerable<SudokuCell> GetCells()
    {
        foreach (SudokuCell cell in grid)
            yield return cell;
    }

    public void Clear()
    {
        foreach (SudokuCell cell in grid)
            cell.Clear();
    }

    public void ClearAllCandidateNumbers()
    {
        foreach (SudokuCell cell in grid)
            cell.ClearCandidateNumbers();
    }

    public void RemoveCandidateNumberFromAllIntersectingCells(Vector2Int position, byte number)
    {
        IEnumerable<SudokuCell> intersectingCells = GetIntersectingCells(position);

        foreach (SudokuCell intersectingCell in intersectingCells)
            intersectingCell.RemoveFromCandidateNumbers(number);
    }

    public IEnumerable<SudokuCell> GetCellsInRow(Vector2Int position)
    {
        for (int i = 0; i < 9; i++)
            yield return grid[i, position.y];
    }

    public IEnumerable<SudokuCell> GetCellsInColumn(Vector2Int position)
    {
        for (int j = 0; j < 9; j++)
            yield return grid[position.x, j];
    }

    public IEnumerable<SudokuCell> GetCellsInBox(Vector2Int position)
    {
        int boxStartX = position.x / 3 * 3;
        int boxStartY = position.y / 3 * 3;

        for (int i = boxStartX; i < boxStartX + 3; i++)
            for (int j = boxStartY; j < boxStartY + 3; j++)
                    yield return grid[i, j];
    }

    public IEnumerable<SudokuCell> GetIntersectingCells(Vector2Int position)
    {
        // Row
        for (int i = 0; i < 9; i++)
            yield return grid[i, position.y];

        // Column
        for (int j = 0; j < 9; j++)
            if (j != position.y)
                yield return grid[position.x, j];

        // Box
        int boxStartX = position.x / 3 * 3;
        int boxStartY = position.y / 3 * 3;

        for (int i = boxStartX; i < boxStartX + 3; i++)
            for (int j = boxStartY; j < boxStartY + 3; j++)
                if (i != position.x && j != position.y)
                    yield return grid[i, j];
    }

    public void SetAllCandidateNumbersForAllCells()
    {
        IEnumerable<SudokuCell> cells = GetCells();

        foreach (SudokuCell cell in cells)
            cell.SetAllCandidateNumbers();
    }

    public void RecalculateCandidateNumbersForAllCells()
    {
        SetAllCandidateNumbersForAllCells();

        IEnumerable<SudokuCell> cellWithNumbers = GetCells().Where(c => c.number != null);

        foreach (SudokuCell cell in cellWithNumbers)
            RemoveCandidateNumberFromAllIntersectingCells(cell.gridPosition, cell.number.Value);
    }

    public void ClearCellNumber(SudokuCell cell) // ??
    {
        cell.number = null;

        RecalculateCandidateNumbersForAllCells();
    }

    public void SetNumberAndUpdateCandidate(SudokuCell cell, byte number)
    {
        if (number != cell.expectedNumber && cell.expectedNumber != 0)
        {
            Debug.LogError("Set a invalid number");
        }

        cell.SetNumber(number);

        RemoveCandidateNumberFromAllIntersectingCells(cell.gridPosition, number);
    }
}