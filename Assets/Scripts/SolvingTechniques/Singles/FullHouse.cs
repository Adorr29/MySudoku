using System.Collections.Generic;

public class FullHouse : SolvingTechnique
{
    static private int startRowIndex = 0;
    static private int startColumnIndex = 0;
    static private int startBoxIndex = 0;

    public FullHouse(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Apply()
    {
        // Check column
        for (int i = startRowIndex; i < 9; i++)
            if (CheckColumn(i) == true)
                return true;

        for (int i = 0; i < startRowIndex; i++)
            if (CheckColumn(i) == true)
                return true;

        // Check rows
        for (int j = startColumnIndex; j < 9; j++)
            if (CheckRow(j) == true)
                return true;

        for (int j = 0; j < startColumnIndex; j++)
            if (CheckRow(j) == true)
                return true;

        // Check box
        for (int n = startBoxIndex; n < 9; n++)
            if (CheckBox(n) == true)
                return true;

        for (int n = 0; n < startBoxIndex; n++)
            if (CheckBox(n) == true)
                return true;

        return false;
    }

    private bool CheckColumn(int rowIndex)
    {
        SudokuCell singleUnsolvedCell = null;
        List<int> candidateNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int j = 0; j < 9; j++)
        {
            SudokuCell cell = sudokuGrid.grid[rowIndex, j];

            if (cell.number == null)
            {
                if (singleUnsolvedCell != null)
                    return false;
                else
                    singleUnsolvedCell = cell;
            }
            else
            {
                candidateNumber.Remove(cell.number.Value);
            }
        }

        if (singleUnsolvedCell == null)
            return false;

        singleUnsolvedCell.SetNumber(candidateNumber[0]);

        startRowIndex = rowIndex + 1;

        return true;
    }

    private bool CheckRow(int columIndex)
    {
        SudokuCell singleUnsolvedCell = null;
        List<int> candidateNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int i = 0; i < 9; i++)
        {
            SudokuCell cell = sudokuGrid.grid[i, columIndex];

            if (cell.number == null)
            {
                if (singleUnsolvedCell != null)
                    return false;
                else
                    singleUnsolvedCell = cell;
            }
            else
            {
                candidateNumber.Remove(cell.number.Value);
            }
        }

        if (singleUnsolvedCell == null)
            return false;

        singleUnsolvedCell.SetNumber(candidateNumber[0]);

        startColumnIndex = columIndex + 1;

        return true;
    }

    private bool CheckBox(int boxIndex)
    {
        SudokuCell singleUnsolvedCell = null;
        List<int> candidateNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        int startBoxX = boxIndex % 3 * 3;
        int startBoxY = boxIndex / 3 * 3;

        for (int n = 0; n < 9; n++)
        {
            SudokuCell cell = sudokuGrid.grid[startBoxX + n % 3, startBoxY + n / 3];

            if (cell.number == null)
            {
                if (singleUnsolvedCell != null)
                    return false;
                else
                    singleUnsolvedCell = cell;
            }
            else
            {
                candidateNumber.Remove(cell.number.Value);
            }
        }

        if (singleUnsolvedCell == null)
            return false;

        singleUnsolvedCell.SetNumber(candidateNumber[0]);

        startBoxIndex = boxIndex + 1;

        return true;
    }
}
