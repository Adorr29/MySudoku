using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LastDigit : SolvingTechnique
{
    public override int difficulty => 50;

    private SudokuCell[] sameNumberCells;
    private SudokuCell findCell;
    private byte findNumber;

    public LastDigit(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Find()
    {
        List<SudokuCell>[] sameNumberCellsList = new List<SudokuCell>[9];

        for (int i = 0; i < 9; i++)
            sameNumberCellsList[i] = new List<SudokuCell>();

        foreach (SudokuCell cell in sudokuGrid.GetCells())
            if (cell.number != null)
                sameNumberCellsList[cell.number.Value - 1].Add(cell);

        for (int i = 0; i < 9; i++)
            if (sameNumberCellsList[i].Count == 8)
            {
                sameNumberCells = sameNumberCellsList[i].ToArray();
                findCell = FindLastCell();
                findNumber = (byte)(i + 1);
                return true;
            }

        return false;
    }

    private SudokuCell FindLastCell()
    {
        int x = Enumerable.Range(0, 9).Except(sameNumberCells.Select(c => c.gridPosition.x)).Single();
        int y = Enumerable.Range(0, 9).Except(sameNumberCells.Select(c => c.gridPosition.y)).Single();

        return sudokuGrid.grid[x, y];
    }

    public override IEnumerator DisplayHelp()
    {
        isAnimationPlaying = true;
        SudokuHelp.currentSolvingTechnique = this;

        Color numberColor = Color.blue;
        Color cellColor = Color.green;
        Color hintBackgroundColor = new Color(1, 0.5f, 0.5f);

        SudokuHelp.SetTitle("Last digit");

        string description = $"Il y a exactement huit <color=#{numberColor.ToHexString()}>{findNumber}</color> dans la grille.\nIl ne reste donc plus qu'un {findNumber} à mettre dans de la grille, et cette <color=#{cellColor.ToHexString()}>case</color> est le seul emplacement possible.";
        SudokuHelp.SetDescription(description);

        foreach (SudokuCell sameNumberCell in sameNumberCells.OrderBy(p => Random.value))
        {
            SudokuHelp.ColorizeCellNumber(numberColor, sameNumberCell);

            SudokuHelp.ColorizeArea(hintBackgroundColor, new RectInt(sameNumberCell.gridPosition.x, 0, 1, 9));
            SudokuHelp.ColorizeArea(hintBackgroundColor, new RectInt(0, sameNumberCell.gridPosition.y, 9, 1));

            yield return Wait(1f);
        }

        SudokuHelp.ColorizeCellFrame(cellColor, findCell);

        isAnimationPlaying = false;
    }

    public override void Apply()
    {
        sudokuGrid.SetNumberAndUpdateCandidate(findCell, findNumber);
    }
}
