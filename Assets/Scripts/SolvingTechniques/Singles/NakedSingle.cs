using System.Collections.Generic;

public class NakedSingle : SolvingTechnique
{
    public NakedSingle(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Apply()
    {
        foreach (SudokuCell cell in sudokuGrid.cells)
            if (cell.number == null)
            {
                List<int> possibleNumbers = sudokuGrid.GetPossibleNumbers(cell);

                if (possibleNumbers.Count == 1)
                {
                    cell.SetNumber(possibleNumbers[0]);

                    return true;
                }
            }

        return false;
    }
}
