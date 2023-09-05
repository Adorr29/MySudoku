using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SudokuHelp : MonoBehaviour
{
    [SerializeField] private CellBackgroundContainer cellBackgroundContainer;
    [SerializeField] private RectTransform areaContainer;
    [SerializeField] private ColorAnimation areaPrefab;
    [Space]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;

    private static SudokuHelp instance;

    public static SolvingTechnique currentSolvingTechnique;

    private static List<SudokuCellVisual> colorizedCellNumbers = new List<SudokuCellVisual>();
    private static List<SudokuCellVisual> framedCells = new List<SudokuCellVisual>();
    private static List<ColorAnimation> backgroundedCells = new List<ColorAnimation>();
    private static List<ColorAnimation> areas = new List<ColorAnimation>();
    //private static List<SudokuCellVisual> colorizedCellCandidateNumbers = new List<SudokuCellVisual>(); // TODO

    private void Awake()
    {
        instance = this;
    }

    public static void Clear()
    {
        currentSolvingTechnique.skipAnimation = true;
        currentSolvingTechnique = null;

        instance.titleText.text = null;
        instance.descriptionText.text = null;

        foreach (SudokuCellVisual colorizedCellNumber in colorizedCellNumbers)
            colorizedCellNumber.targetColor = Color.black;
        colorizedCellNumbers.Clear();

        foreach (SudokuCellVisual framedCell in framedCells)
            framedCell.frame.FadeOut();
        framedCells.Clear();

        foreach (ColorAnimation backgroundedCell in backgroundedCells)
            backgroundedCell.FadeOut();
        backgroundedCells.Clear();

        foreach (ColorAnimation area in areas)
            Destroy(area.gameObject);
        areas.Clear();

        // TODO reset colorizedCellCandidateNumbers taget color
    }

    public static void SetTitle(string description)
    {
        if (currentSolvingTechnique == null)
            return;

        instance.titleText.text = description;
    }

    public static void SetDescription(string title)
    {
        if (currentSolvingTechnique == null)
            return;

        instance.descriptionText.text = title;
    }

    public static void ColorizeCellNumber(Color color, SudokuCell cell)
    {
        if (currentSolvingTechnique == null)
            return;

        SudokuCellVisual cellVisual = GetCellVisualFromCell(cell);

        cellVisual.targetColor = color;

        colorizedCellNumbers.Add(cellVisual);
    }

    public static void ColorizeCellFrame(Color color, SudokuCell cell)
    {
        if (currentSolvingTechnique == null)
            return;

        SudokuCellVisual cellVisual = GetCellVisualFromCell(cell);

        Color startColor = color;
        startColor.a = 0;
        cellVisual.frame.Animate(startColor, color);

        framedCells.Add(cellVisual);
    }

    public static void ColorizeCellBackground(Color color, SudokuCell cell)
    {
        if (currentSolvingTechnique == null)
            return;

        ColorAnimation cellBackground = instance.cellBackgroundContainer.cellBackgroundGrid[cell.gridPosition.x, cell.gridPosition.y];

        Color startColor = color;
        startColor.a = 0;
        cellBackground.Animate(startColor, color);

        backgroundedCells.Add(cellBackground);
    }

    public static void ColorizeArea(Color color, RectInt areaRect)
    {
        if (currentSolvingTechnique == null)
            return;

        ColorAnimation area = Instantiate(instance.areaPrefab, instance.areaContainer);
        RectTransform areaRectTransform = area.GetComponent<RectTransform>();

        RectTransform startCell = GetCellVisualFromCell(new Vector2Int(areaRect.x, areaRect.y)).GetComponent<RectTransform>();
        RectTransform endCell = GetCellVisualFromCell(new Vector2Int(areaRect.x + areaRect.width - 1, areaRect.y + areaRect.height - 1)).GetComponent<RectTransform>();

        Vector2 anchorMin;
        anchorMin.x = startCell.anchorMin.x;
        anchorMin.y = endCell.anchorMin.y;
        areaRectTransform.anchorMin = anchorMin;

        Vector2 anchorMax;
        anchorMax.x = endCell.anchorMax.x;
        anchorMax.y = startCell.anchorMax.y;
        areaRectTransform.anchorMax = anchorMax;

        Color startColor = color;
        startColor.a = 0;
        area.Animate(startColor, color);

        areas.Add(area);
    }

    public static void ColorizeCellCandidateNumber(Color color, SudokuCell cell, byte candidateNumber)
    {
        if (currentSolvingTechnique == null)
            return;

        // TODO
    }

    private static SudokuCellVisual GetCellVisualFromCell(SudokuCell cell)
    {
        return GetCellVisualFromCell(cell.gridPosition);
    }

    private static SudokuCellVisual GetCellVisualFromCell(Vector2Int gridPosition)
    {
        SudokuCellVisual cellVisual = GameManager.instance.sudokuGridVisual.visualGrid[gridPosition.x, gridPosition.y];

        return cellVisual;
    }
}
