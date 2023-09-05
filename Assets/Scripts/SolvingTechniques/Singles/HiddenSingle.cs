using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenSingle : SolvingTechnique
{
    public override int difficulty => 300;

    private HouseType houseType;
    private SudokuCell findCell;
    private byte findNumber;

    public HiddenSingle(SudokuGrid sudokuGrid) : base(sudokuGrid)
    {
    }

    public override bool Find()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (FindSingleCandidateForBox(i * 3, j * 3) == true)
                    return true;

        for (int j = 0; j < 9; j++)
            if (FindSingleCandidateForRow(j) == true)
                return true;

        for (int i = 0; i < 9; i++)
            if (FindSingleCandidateForColumn(i) == true)
                return true;

        return false;
    }

    private List<SudokuCell>[] CreateEmptyCellsPerCandidate()
    {
        List<SudokuCell>[] cellsPerCandidate = new List<SudokuCell>[9];

        for (int i = 0; i < 9; i++)
            cellsPerCandidate[i] = new List<SudokuCell>();

        return cellsPerCandidate;
    }

    private bool FindSingleCandidate(List<SudokuCell>[] cellsPerCandidate)
    {
        for (int i = 0; i < 9; i++)
        {
            List<SudokuCell> cells = cellsPerCandidate[i];

            if (cells.Count == 1)
            {
                SudokuCell cell = cells[0];

                findCell = cell;
                findNumber = (byte)(i + 1);

                return true;
            }
        }

        return false;
    }

    private bool FindSingleCandidateForColumn(int x)
    {
        List<SudokuCell>[] cellsPerCandidate = CreateEmptyCellsPerCandidate();

        for (int j = 0; j < 9; j++)
        {
            SudokuCell cell = sudokuGrid.grid[x, j];

            if (cell.number != null)
                continue;

            cell.candidateNumbers.ForEach(n => cellsPerCandidate[n - 1].Add(cell));
        }

        if (FindSingleCandidate(cellsPerCandidate))
        {
            houseType = HouseType.Column;
            return true;
        }

        return false;
    }

    private bool FindSingleCandidateForRow(int y)
    {
        List<SudokuCell>[] cellsPerCandidate = CreateEmptyCellsPerCandidate();

        for (int i = 0; i < 9; i++)
        {
            SudokuCell cell = sudokuGrid.grid[i, y];

            if (cell.number != null)
                continue;

            cell.candidateNumbers.ForEach(n => cellsPerCandidate[n - 1].Add(cell));
        }

        if (FindSingleCandidate(cellsPerCandidate))
        {
            houseType = HouseType.Row;
            return true;
        }

        return false;
    }

    private bool FindSingleCandidateForBox(int boxStartX, int boxStartY)
    {
        List<SudokuCell>[] cellsPerCandidate = CreateEmptyCellsPerCandidate();

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                SudokuCell cell = sudokuGrid.grid[i + boxStartX, j + boxStartY];

                if (cell.number != null)
                    continue;

                cell.candidateNumbers.ForEach(n => cellsPerCandidate[n - 1].Add(cell));
            }

        if (FindSingleCandidate(cellsPerCandidate))
        {
            houseType = HouseType.Box;
            return true;
        }

        return false;
    }

    public override IEnumerator DisplayHelp()
    {
        isAnimationPlaying = true;
        SudokuHelp.currentSolvingTechnique = this;

        Color houseBackbroundColor = Color.blue;
        houseBackbroundColor.a = 0.4f;
        Color cellColor = Color.green;
        Color hintCellNumberColor = new Color(0.5f, 0, 0);
        Color hintBackgroundColor = new Color(1, 0.5f, 0.5f);

        SudokuHelp.SetTitle("Hidden single");

        string word1 = "";
        string word2 = "";

        IEnumerable<SudokuCell> houseCells = null;

        if (houseType == HouseType.Row)
        {
            word1 = "cette";
            word2 = "ligne";

            houseCells = sudokuGrid.GetCellsInRow(findCell.gridPosition);
        }
        else if (houseType == HouseType.Column)
        {
            word1 = "cette";
            word2 = "colonne";

            houseCells = sudokuGrid.GetCellsInColumn(findCell.gridPosition);
        }
        else if (houseType == HouseType.Box)
        {
            word1 = "ce";
            word2 = "bloc";

            houseCells = sudokuGrid.GetCellsInBox(findCell.gridPosition);
        }

        string description = $"Cette <color=#{cellColor.ToHtmlStringRGB()}>case</color> est la seule case pouvant contenir un {findNumber} dans {word1} <color=#{houseBackbroundColor.ToHtmlStringRGB()}>{word2}</color>.";
        SudokuHelp.SetDescription(description);

        foreach (SudokuCell houseCell in houseCells)
            SudokuHelp.ColorizeCellBackground(houseBackbroundColor, houseCell);

        List<SudokuCell> hintCells = GetHintCells();

        foreach (SudokuCell hintCell in hintCells)
        {
            SudokuHelp.ColorizeCellNumber(hintCellNumberColor, hintCell);
            SudokuHelp.ColorizeArea(hintBackgroundColor, GetHintArea(hintCell));
        }

        SudokuHelp.ColorizeCellFrame(cellColor, findCell);

        yield return null;

        isAnimationPlaying = false;
    }

    private RectInt GetHintArea(SudokuCell hintCell)
    {
        if (houseType == HouseType.Row)
            return GetHintAreaOfRow(hintCell);
        else if (houseType == HouseType.Column)
            return GetHintAreaOfColumn(hintCell);
        else if (houseType == HouseType.Box)
            return GetHintAreaOfBox(hintCell);

        return new RectInt();
    }

    private RectInt GetHintAreaOfRow(SudokuCell hintCell)
    {
        Vector2Int startBox = new Vector2Int();
        startBox.x = hintCell.gridPosition.x / 3 * 3;
        startBox.y = hintCell.gridPosition.y / 3 * 3;

        if (findCell.gridPosition.y >= startBox.y && findCell.gridPosition.y < startBox.y + 3)
            return new RectInt(startBox.x, startBox.y, 3, 3);

        RectInt hintArea = new RectInt();
        hintArea.x = hintCell.gridPosition.x;
        hintArea.y = Mathf.Min(findCell.gridPosition.y, hintCell.gridPosition.y);
        hintArea.width = 1;
        hintArea.height = Mathf.Abs(findCell.gridPosition.y - hintCell.gridPosition.y) + 1;

        return hintArea;
    }

    private RectInt GetHintAreaOfColumn(SudokuCell hintCell)
    {
        Vector2Int startBox = new Vector2Int();
        startBox.x = hintCell.gridPosition.x / 3 * 3;
        startBox.y = hintCell.gridPosition.y / 3 * 3;

        if (findCell.gridPosition.x >= startBox.x && findCell.gridPosition.x < startBox.x + 3)
            return new RectInt(startBox.x, startBox.y, 3, 3);

        RectInt hintArea = new RectInt();
        hintArea.x = Mathf.Min(findCell.gridPosition.x, hintCell.gridPosition.x);
        hintArea.y = hintCell.gridPosition.y;
        hintArea.width = Mathf.Abs(findCell.gridPosition.x - hintCell.gridPosition.x) + 1;
        hintArea.height = 1;

        return hintArea;
    }

    private RectInt GetHintAreaOfBox(SudokuCell hintCell)
    {
        Vector2Int startBox = new Vector2Int();
        startBox.x = findCell.gridPosition.x / 3 * 3;
        startBox.y = findCell.gridPosition.y / 3 * 3;

        RectInt hintArea = new RectInt();

        if (hintCell.gridPosition.x >= startBox.x && hintCell.gridPosition.x < startBox.x + 3)
        {
            int opposed = hintCell.gridPosition.y < startBox.y ? startBox.y + 2 : startBox.y;

            hintArea.x = hintCell.gridPosition.x;
            hintArea.y = Mathf.Min(startBox.y, hintCell.gridPosition.y);
            hintArea.width = 1;
            hintArea.height = Mathf.Abs(opposed - hintCell.gridPosition.y) + 1;
        }
        else
        {
            int opposed = hintCell.gridPosition.x < startBox.x ? startBox.x + 2 : startBox.x;

            hintArea.x = Mathf.Min(startBox.x, hintCell.gridPosition.x);
            hintArea.y = hintCell.gridPosition.y;
            hintArea.width = Mathf.Abs(opposed - hintCell.gridPosition.x) + 1;
            hintArea.height = 1;
        }

        return hintArea;
    }

    private List<SudokuCell> GetHintCells()
    {
        if (houseType == HouseType.Row)
            return GetHintCellsOfRow();
        else if (houseType == HouseType.Column)
            return GetHintCellsOfColumn();
        else if (houseType == HouseType.Box)
            return GetHintCellsOfBox();

        return null;
    }

    private List<SudokuCell> GetHintCellsOfRow()
    {
        List<SudokuCell> hintCells = new List<SudokuCell>();

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                SudokuCell cell = sudokuGrid.grid[i, j];

                if (cell.number == findNumber)
                    hintCells.Add(cell);
            }

        return hintCells;
    }

    private List<SudokuCell> GetHintCellsOfColumn()
    {
        List<SudokuCell> hintCells = new List<SudokuCell>();

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                SudokuCell cell = sudokuGrid.grid[i, j];

                if (cell.number == findNumber)
                    hintCells.Add(cell);
            }

        return hintCells;
    }

    private List<SudokuCell> GetHintCellsOfBox()
    {
        List<SudokuCell> hintCells = new List<SudokuCell>();

        int boxStartX = findCell.gridPosition.x / 3 * 3;
        int boxStartY = findCell.gridPosition.y / 3 * 3;

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                if (i >= boxStartX && i <= boxStartX + 2 || j >= boxStartY && j <= boxStartY + 2)
                {
                    SudokuCell cell = sudokuGrid.grid[i, j];

                    if (cell.number == findNumber)
                        hintCells.Add(cell);
                }

        return hintCells;
    }

    public override void Apply()
    {
        sudokuGrid.SetNumberAndUpdateCandidate(findCell, findNumber);
    }
}
