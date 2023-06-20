using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class SudokuCell : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text;
    [Space]
    [SerializeField] private GameObject numberButtonsParent;
    [SerializeField] private NumerButton[] numberButtons;

    [HideInInspector] public Vector2Int gridPosition;

    public int? number { get; private set; }
    [HideInInspector] public int expectedNumber;

    public Action<SudokuCell, int?> onSetNumber;

    public void ClearNumber()
    {
        number = null;

        if (GameManager.instance.sudokuGrid.gridIsBuilding)
            return;

        text.text = null;

        numberButtonsParent.SetActive(true);
    }

    public void SetNumber(int cellNumber)
    {
        number = cellNumber;

        if (GameManager.instance.sudokuGrid.gridIsBuilding)
            return;

        text.text = number.ToString();

        numberButtonsParent.SetActive(false);

        onSetNumber?.Invoke(this, number);
    }

    public bool NoNumberButtonVisible()
    {
        return numberButtons.Any(b => b.isVisible) == false;
    }

    public void RemoveCandidateNumber(int candidateNumber)
    {
        numberButtons[candidateNumber - 1].Hide();
    }

    public void BadNumber()
    {
        animator.Play("BadNumber", 0, 0);

        Invoke(nameof(ClearNumber), 0.5f);
    }

    public void GoodNumber()
    {
        animator.Play("GoodNumber", 0, 0);
    }

    public void HighLightCell()
    {
        animator.Play("HighLightCell", 0, 0);
    }
}
