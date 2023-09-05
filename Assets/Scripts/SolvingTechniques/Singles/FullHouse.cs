using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HouseType
{ 
    Row,
    Column,
    Box
}

public class FullHouse : SolvingTechnique
{
    public override int difficulty => 100;

    private SudokuCell[] houseCells = new SudokuCell[9];
    private HouseType houseType;
    private SudokuCell findCell;
    private byte findNumber;

    public FullHouse(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Find()
    {
        for (int n = 0; n < 9; n++)
            if (CheckBox(n) == true)
                return true;

        for (int j = 0; j < 9; j++)
            if (CheckRow(j) == true)
                return true;

        for (int i = 0; i < 9; i++)
            if (CheckColumn(i) == true)
                return true;

        return false;
    }

    private bool CheckColumn(int columIndex)
    {
        SudokuCell singleUnsolvedCell = null;
        List<byte> candidateNumber = new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int j = 0; j < 9; j++)
        {
            SudokuCell cell = sudokuGrid.grid[columIndex, j];

            houseCells[j] = cell;

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

        houseType = HouseType.Column;
        findCell = singleUnsolvedCell;
        findNumber = candidateNumber[0];

        return true;
    }

    private bool CheckRow(int rowIndex)
    {
        SudokuCell singleUnsolvedCell = null;
        List<byte> candidateNumber = new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int i = 0; i < 9; i++)
        {
            SudokuCell cell = sudokuGrid.grid[i, rowIndex];

            houseCells[i] = cell;

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

        houseType = HouseType.Row;
        findCell = singleUnsolvedCell;
        findNumber = candidateNumber[0];

        return true;
    }

    private bool CheckBox(int boxIndex)
    {
        SudokuCell singleUnsolvedCell = null;
        List<byte> candidateNumber = new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        int startBoxX = boxIndex % 3 * 3;
        int startBoxY = boxIndex / 3 * 3;

        for (int n = 0; n < 9; n++)
        {
            SudokuCell cell = sudokuGrid.grid[startBoxX + n % 3, startBoxY + n / 3];

            houseCells[n] = cell;

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

        houseType = HouseType.Box;
        findCell = singleUnsolvedCell;
        findNumber = candidateNumber[0];

        return true;
    }

    public override IEnumerator DisplayHelp()
    {
        isAnimationPlaying = true;
        SudokuHelp.currentSolvingTechnique = this;

        Color backbroundColor = Color.blue;
        backbroundColor.a = 0.4f;
        Color cellColor = Color.green;

        SudokuHelp.SetTitle("Full house");

        string word1 = "";
        string word2 = "";

        if (houseType == HouseType.Row)
        {
            word1 = "cette";
            word2 = "ligne";
        }
        else if (houseType == HouseType.Column)
        {
            word1 = "cette";
            word2 = "colonne";
        }
        else if (houseType == HouseType.Box)
        {
            word1 = "ce";
            word2 = "bloc";
        }

        string description = $"Il ne manque plus que un chiffre dans {word1} <color=#{backbroundColor.ToHtmlStringRGB()}>{word2}</color>.\nCela ne laisse plus qu'un chiffre possible pour le dernier emplacement.";
        SudokuHelp.SetDescription(description);

        foreach (SudokuCell houseCell in houseCells)
            SudokuHelp.ColorizeCellBackground(backbroundColor, houseCell);

        SudokuHelp.ColorizeCellFrame(cellColor, findCell);

        yield return null;

        isAnimationPlaying = false;
    }

    public override void Apply()
    {
        sudokuGrid.SetNumberAndUpdateCandidate(findCell, findNumber);
    }
}
