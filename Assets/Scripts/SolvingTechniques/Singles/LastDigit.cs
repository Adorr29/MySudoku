public class LastDigit : SolvingTechnique
{
    public LastDigit(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Apply()
    {
        byte[] numberCount = new byte[9];

        foreach (SudokuCell cell in sudokuGrid.cells)
            if (cell.number != null)
                numberCount[cell.number.Value - 1]++;

        for (byte i = 0; i < 9; i++)
            if (numberCount[i] == 8)
            {
                FindAndSetNumber((byte)(i + 1));

                return true;
            }

        return false;
    }

    private void FindAndSetNumber(byte number)
    {
        const byte undefined = 10;

        byte gridPositionX = undefined;
        byte gridPositionY = undefined;

        for (byte n = 0; n < 9; n++)
        {
            if (gridPositionX == undefined && NumberIsInColum(n, number) == false)
                gridPositionX = n;

            if (gridPositionY == undefined && NumberIsInRow(n, number) == false)
                gridPositionY = n;
        }

        sudokuGrid.grid[gridPositionX, gridPositionY].SetNumber(number);
    }

    private bool NumberIsInRow(byte columnIndex, byte number)
    {
        for (byte i = 0; i < 9; i++)
            if (sudokuGrid.grid[i, columnIndex].number == number)
                return true;

        return false;
    }

    private bool NumberIsInColum(byte columnIndex, byte number)
    {
        for (byte j = 0; j < 9; j++)
            if (sudokuGrid.grid[columnIndex, j].number == number)
                return true;

        return false;
    }
}
