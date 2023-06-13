using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

//[ExecuteInEditMode]
public class SudokuGrid : MonoBehaviour
{
    public SudokuCell[] cells;

    private SudokuCell[,] grid;

    public bool gridIsBuilding { get; private set; }

    private void OnEnable()
    {
        foreach (SudokuCell cell in cells)
            cell.onSetNumber += OnCellSetNumber;
    }

    private void OnDisable()
    {
        foreach (SudokuCell cell in cells)
            cell.onSetNumber -= OnCellSetNumber;
    }

    private void Awake()
    {
        grid = new SudokuCell[9, 9];

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                grid[i, j] = cells[i * 9 + j];

                grid[i, j].gridPosition = new Vector2Int(i, j);
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    private void OnCellSetNumber(SudokuCell cell, int? number)
    {
        if (gridIsBuilding)
            return;

        if (cell.number == cell.expectedNumber)
        {
            bool gridIsFinish = cells.Count(c => c.number == null) == 0;

            if (gridIsFinish)
            {
                StartCoroutine(EndAnimation());
            }
            else
            {
                List<SudokuCell> compleatedHouseCells = new List<SudokuCell>();

                SudokuCell[] cellsInSameRow = GetCellsInSameRow(cell);
                SudokuCell[] cellsInSameColumn = GetCellsInSameColumn(cell);
                SudokuCell[] cellsInSameBox = GetCellsInSameBox(cell);

                if (cellsInSameRow.Count(c => c.number == null) == 0)
                    compleatedHouseCells.AddRange(cellsInSameRow);
                if (cellsInSameColumn.Count(c => c.number == null) == 0)
                    compleatedHouseCells.AddRange(cellsInSameColumn);
                if (cellsInSameBox.Count(c => c.number == null) == 0)
                    compleatedHouseCells.AddRange(cellsInSameBox);

                if (compleatedHouseCells.Count > 0)
                {
                    StartCoroutine(CompleatedHouseAnimation(compleatedHouseCells.Distinct()));
                }
                else
                {
                    cell.GoodNumber();
                }

                SudokuCell[] inerationCells = GetCellsInIneration(cell);

                foreach (SudokuCell inerationCell in inerationCells)
                    inerationCell.RemoveCandidateNumber(number.Value);
            }
        }
        else
        {
            cell.BadNumber();

            foreach (SudokuCell conflictingCell in GetConflictingCells(cell))
                conflictingCell.HighLightCell();
        }
    }

    private IEnumerator CompleatedHouseAnimation(IEnumerable<SudokuCell> compleatedHouseCells)
    {
        List<List<SudokuCell>> cellsList = new List<List<SudokuCell>>(17);

        for (int i = 0; i < 17; i++)
            cellsList.Add(new List<SudokuCell>());

        foreach (SudokuCell cell in compleatedHouseCells)
        { 
            int index = cell.gridPosition.x + cell.gridPosition.y;

            cellsList[index].Add(cell);
        }

        cellsList.RemoveAll(l => l.Count() == 0);

        foreach (List<SudokuCell> cells in cellsList)
        {
            foreach (SudokuCell cell in cells)
                cell.GoodNumber();

            yield return new WaitForSeconds(0.06f);
        }
    }

    private IEnumerator EndAnimation()
    {
        for (int a = 0; a < 17; a++)
        {
            for (int b = Mathf.Max(a - 8, 0); b <= Mathf.Min(a, 8); b++)
                grid[a - b, b].GoodNumber();

            yield return new WaitForSeconds(0.06f);
        }

        yield return new WaitForSeconds(2);

        GameManager.instance.BackToMenu();
    }

    public int?[] ToNumbers()
    {
        return cells.Select(c => c.number).ToArray();
    }

    public void MakeMinimal()
    {
        SudokuCell[] shuffledCells = cells.OrderBy(c => UnityEngine.Random.value).ToArray();

        foreach (SudokuCell cell in shuffledCells)
        {
            int cellNumber = cell.number.Value;

            cell.ClearNumber();

            int solutionCount = Solve();

            if (solutionCount > 1)
                cell.SetNumber(cellNumber);
        }
    }

    public int Solve()
    {
        SudokuCell emptyCell = cells.FirstOrDefault(c => c.number == null);

        if (emptyCell == null)
            return 1;

        int solutionCount = 0;

        List<int> possibleNumbers = GetPossibleNumbers(emptyCell);

        foreach (int number in possibleNumbers)
        {
            emptyCell.SetNumber(number);

            solutionCount += Solve();

            if (solutionCount > 1)
                break;
        }

        emptyCell.ClearNumber();

        return solutionCount;
    }

    public void ClearGrid()
    { 
        foreach (SudokuCell cell in cells)
            cell.ClearNumber();
    }

    public void CreateGrid()
    {
        gridIsBuilding = true;

        int attempt = 0;

        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        timer.Start();
        while (attempt < 10000)
        {
            ClearGrid();

            attempt++;

            if (AttemptCreateGrid() == true)
                break;
        }
        Debug.Log("attempt : " + attempt + ", time : " + timer.Elapsed.ToString());
        timer.Stop();

        foreach (SudokuCell cell in cells)
            cell.expectedNumber = cell.number.Value;

        timer.Start();
        MakeMinimal();
        Debug.Log("MakeMinimal time : " + timer.Elapsed.ToString());
        timer.Stop();

        gridIsBuilding = false;

        UpdateCellGraphic();
    }

    private void UpdateCellGraphic()
    {
        foreach (SudokuCell cell in cells)
        {
            if (cell.number.HasValue)
                cell.SetNumber(cell.number.Value);
            else
                cell.ClearNumber();
        }
    }

    private bool AttemptCreateGrid()
    {
        SudokuCell[] shuffledCells = cells;//.OrderBy(c => UnityEngine.Random.value).ToArray();

        foreach (SudokuCell cell in shuffledCells)
        {
            List<int> possibleNumbers = GetPossibleNumbers(cell);

            if (possibleNumbers.Count == 0)
                return false;

            cell.SetNumber(possibleNumbers[UnityEngine.Random.Range(0, possibleNumbers.Count)]);
        }

        return true;
    }

    public List<int> GetPossibleNumbers(SudokuCell cell)
    {
        List<int> possibleNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9};

        IEnumerable<int?> otherNumbers = GetCellsInIneration(cell).Select(c => c.number).Distinct();

        possibleNumbers.RemoveAll(n => otherNumbers.Contains(n));

        return possibleNumbers;
    }

    public IEnumerable<SudokuCell> GetConflictingCells(SudokuCell cell)
    {
        return GetCellsInIneration(cell).Where(c => c.number == cell.number);
    }

    public bool CheckCellValidity(SudokuCell cell, out List<SudokuCell> conflictingCells)
    {
        conflictingCells = new List<SudokuCell>();

        conflictingCells.AddRange(GetCellsInIneration(cell).Where(c => c.number == cell.number));

        return conflictingCells.Count == 0;
    }

    public SudokuCell[] GetCellsInIneration(SudokuCell cell)
    {
        SudokuCell[] otherCells = new SudokuCell[20];

        Array.Copy(GetOhterCellsInSameRow(cell), 0, otherCells, 0, 8);
        Array.Copy(GetOtherCellsInSameColumn(cell), 0, otherCells, 8,  8);
        Array.Copy(GetOtherCellsInSameBoxWithoutAlignment(cell), 0, otherCells, 16, 4);

        return otherCells;
    }

    public SudokuCell[] GetOhterCellsInSameRow(SudokuCell cell)
    {
        SudokuCell[] otherCells = new SudokuCell[8];
        int index = 0;

        for (int i = 0; i < cell.gridPosition.x; i++)
            otherCells[index++] = grid[i, cell.gridPosition.y];

        for (int i = cell.gridPosition.x + 1; i < 9; i++)
            otherCells[index++] = grid[i, cell.gridPosition.y];

        return otherCells;
    }

    public SudokuCell[] GetOtherCellsInSameColumn(SudokuCell cell)
    {
        SudokuCell[] otherCells = new SudokuCell[8];
        int index = 0;

        for (int j = 0; j < cell.gridPosition.y; j++)
            otherCells[index++] = grid[cell.gridPosition.x, j];

        for (int j = cell.gridPosition.y + 1; j < 9; j++)
            otherCells[index++] = grid[cell.gridPosition.x, j];

        return otherCells;
    }

    public SudokuCell[] GetOtherCellsInSameBoxWithoutAlignment(SudokuCell cell)
    {
        SudokuCell[] otherCells = new SudokuCell[4];
        int index = 0;

        Vector2Int boxStart = Vector2Int.zero;
        boxStart.x = cell.gridPosition.x / 3 * 3;
        boxStart.y = cell.gridPosition.y / 3 * 3;

        for (int i = boxStart.x; i < boxStart.x + 3; i++)
            for (int j = boxStart.y; j < boxStart.y + 3; j++)
                if (i != cell.gridPosition.x && j != cell.gridPosition.y)
                    otherCells[index++] = grid[i, j];

        return otherCells;
    }

    public SudokuCell[] GetCellsInSameRow(SudokuCell cell)
    {
        SudokuCell[] otherCells = new SudokuCell[9];
        int index = 0;

        for (int i = 0; i < 9; i++)
            otherCells[index++] = grid[i, cell.gridPosition.y];

        return otherCells;
    }

    public SudokuCell[] GetCellsInSameColumn(SudokuCell cell)
    {
        SudokuCell[] otherCells = new SudokuCell[9];
        int index = 0;

        for (int j = 0; j < 9; j++)
            otherCells[index++] = grid[cell.gridPosition.x, j];

        return otherCells;
    }

    public SudokuCell[] GetCellsInSameBox(SudokuCell cell)
    {
        SudokuCell[] otherCells = new SudokuCell[9];
        int index = 0;

        Vector2Int boxStart = Vector2Int.zero;
        boxStart.x = cell.gridPosition.x / 3 * 3;
        boxStart.y = cell.gridPosition.y / 3 * 3;

        for (int i = boxStart.x; i < boxStart.x + 3; i++)
            for (int j = boxStart.y; j < boxStart.y + 3; j++)
                otherCells[index++] = grid[i, j];

        return otherCells;
    }
}
