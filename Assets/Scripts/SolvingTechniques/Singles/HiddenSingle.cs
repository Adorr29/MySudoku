public class HiddenSingle : SolvingTechnique
{
    public HiddenSingle(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Apply()
    {
        for (byte n = 0; n < 9; n++)
            if (CheckNumber(n) == true)
                return true;

        return false;
    }

    private bool CheckNumber(byte boxIndex)
    {
        for (byte n = 1; n <= 9; n++)
            if (FindAndSetNumber(boxIndex, n) == true)
                return true;

        return false;
    }

    private bool FindAndSetNumber(byte boxIndex, byte number)
    {
        int startBoxX = boxIndex % 3 * 3;
        int startBoxY = boxIndex / 3 * 3;

        SudokuCell singleCellWithNumber = null;

        for (byte i = 0; i < 3; i++)
            for (byte j = 0; j < 3; j++)
            {
                SudokuCell cell = sudokuGrid.grid[startBoxX + i, startBoxY + j];

                if (cell.number != null)
                    continue;

                if (sudokuGrid.GetPossibleNumbers(cell).Contains(number))
                {
                    if (singleCellWithNumber == null)
                        singleCellWithNumber = cell;
                    else
                        return false;
                }
            }

        if (singleCellWithNumber == null)
            return false;

        singleCellWithNumber.SetNumber(number);

        return true;
    }
}
