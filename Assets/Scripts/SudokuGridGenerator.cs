using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SudokuGridGenerator : MonoBehaviour
{
    private class Cell
    {
        public byte? number;
        public List<byte> possibleNumbers;

        public Cell CreateCopy()
        { 
            Cell cell = new Cell();

            cell.number = number;
            cell.possibleNumbers = new List<byte>(possibleNumbers);

            return cell;
        }

        public void None() // TODO rename
        {
            possibleNumbers = new List<byte>();
        }

        public void All() // TODO rename
        {
            possibleNumbers = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public void Clear()
        {
            number = null;

            All();
        }

        public void RemoveFromPossibleNumbers(byte numberToRemove)
        {
            possibleNumbers.Remove(numberToRemove);
        }

        public bool HavePossibleNumbers()
        {
            return possibleNumbers.Count > 0;
        }

        public bool HaveOnlyOnePossibleNumber()
        {
            return possibleNumbers.Count == 1;
        }

        public void SetRandomNumberFromPossibleNumbers()
        {
            number = possibleNumbers[Random.Range(0, possibleNumbers.Count)];
        }

        public bool SetNumberIfOnlyOnepossibleNumber()
        {
            if (HaveOnlyOnePossibleNumber() == false)
                return false;

            number = possibleNumbers[0];

            return true;
        }
    }

    private class Grid
    { 
        public Cell[,] cells = new Cell[9, 9];

        public static Grid CreateEmptyGrid()
        {
            Grid grid = new Grid();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    grid.cells[i, j] = new Cell();

                    grid.cells[i, j].All();
                }

            return grid;
        }

        public Grid CreateCopy()
        {
            Grid grid = new Grid();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    grid.cells[i, j] = cells[i, j].CreateCopy();

            return grid;
        }

        public void Clear()
        {
            foreach (Cell cell in cells)
                cell.Clear();
        }

        public void ConstrainNumber(int x, int y, byte number)
        {
            // Row
            for (int i = 0; i < x; i++)
                cells[i, y].RemoveFromPossibleNumbers(number);

            for (int i = x + 1; i < 9; i++)
                cells[i, y].RemoveFromPossibleNumbers(number);

            // Column
            for (int j = 0; j < y; j++)
                cells[x, j].RemoveFromPossibleNumbers(number);

            for (int j = y + 1; j < 9; j++)
                cells[x, j].RemoveFromPossibleNumbers(number);

            // Box
            int boxStartX = x / 3 * 3;
            int boxStartY = y / 3 * 3;

            for (int i = boxStartX; i < boxStartX + 3; i++)
                for (int j = boxStartY; j < boxStartY + 3; j++)
                    if (i != x && j != y)
                        cells[i, j].RemoveFromPossibleNumbers(number);
        }

        public void RemoveConstrains()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    cells[i, j].All();
        }

        public void Reconstrain()
        {
            RemoveConstrains();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (cells[i, j].number != null)
                        ConstrainNumber(i, j, cells[i, j].number.Value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Grid grid = CreateGrid();
        PrintGrid(grid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Grid CreateGrid()
    {
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        Grid grid = Grid.CreateEmptyGrid();

        int attempt = 0;

        timer.Start();
        while (attempt < 10000)
        {
            attempt++;

            if (AttemptFillGrid(grid) == true)
                break;

            grid.Clear();
        }
        timer.Stop();

        Debug.Log("Attempt : " + attempt + ", Time : " + timer.Elapsed.ToString());

        RemoveCells(grid);

        return grid;
    }

    void RemoveCells(Grid grid)
    {
        Vector2Int[] cellPositions = new Vector2Int[9 * 9];

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                cellPositions[i * 9 + j] = new Vector2Int(i, j);

        cellPositions = cellPositions.OrderBy(p => Random.value).ToArray();

        foreach (Vector2Int cellPosition in cellPositions)
        {
            byte cellNumber = grid.cells[cellPosition.x, cellPosition.y].number.Value;

            grid.cells[cellPosition.x, cellPosition.y].number = null;

            grid.Reconstrain();

            byte solutionCount = Solve();

            if (solutionCount > 1)
            {
                grid.cells[cellPosition.x, cellPosition.y].number = cellNumber;
            }
        }
    }

    private byte Solve()
    {
        return 1;
    }

    private bool AttemptFillGrid(Grid grid)
    {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                Cell cell = grid.cells[i, j];

                if (cell.HavePossibleNumbers() == false)
                    return false;

                cell.SetRandomNumberFromPossibleNumbers();

                grid.ConstrainNumber(i, j, cell.number.Value);
            }

        return true;
    }

    private void PrintGrid(Grid grid)
    {
        string str = "";

        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                str += "   ";

                if (grid.cells[i, j].number == null)
                    str += "_";
                else
                    str += grid.cells[i, j].number.ToString();
            }

            str += "\n";
        }

        Debug.Log(str);
    }
}
