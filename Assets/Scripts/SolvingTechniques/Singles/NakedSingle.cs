using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NakedSingle : SolvingTechnique
{
    public override int difficulty => 400;

    private SudokuCell findCell;
    private byte findNumber;

    public NakedSingle(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Find()
    {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                SudokuCell cell = sudokuGrid.grid[i, j];

                if (cell.number != null)
                    continue;

                if (cell.candidateNumbers.Count == 1)
                {
                    findCell = cell;
                    findNumber = cell.candidateNumbers[0];

                    return true;
                }
            }

        return false;
    }

    public override IEnumerator DisplayHelp()
    {
        isAnimationPlaying = true;
        SudokuHelp.currentSolvingTechnique = this;

        Color backbroundColor = Color.white;
        backbroundColor.a = 0.5f;
        Color numberColor = Color.blue;
        Color cellColor = Color.green;

        SudokuHelp.SetTitle("Naked single");

        string description = $"Il y a huit <color=#{numberColor.ToHexString()}>chiffres différents</color> dans ligne, colonne et bloc.\nCela ne laisse plus qu'une possibilé pour cette <color=#{cellColor.ToHexString()}>case</color>.";
        SudokuHelp.SetDescription(description);

        IEnumerable<SudokuCell> intersectingCells = sudokuGrid.GetIntersectingCells(findCell.gridPosition);

        foreach (SudokuCell intersectingCell in intersectingCells)
            SudokuHelp.ColorizeCellBackground(backbroundColor, intersectingCell);

        for (byte number = 1; number <= 9; number++)
            if (number != findNumber)
            {
                SudokuCell cell = intersectingCells.First(cell => cell.number == number);

                SudokuHelp.ColorizeCellNumber(numberColor, cell);
            }

        SudokuHelp.ColorizeCellFrame(cellColor, findCell);

        yield return null;

        isAnimationPlaying = false;
    }

    public override void Apply()
    {
        sudokuGrid.SetNumberAndUpdateCandidate(findCell, findNumber);
    }
}
