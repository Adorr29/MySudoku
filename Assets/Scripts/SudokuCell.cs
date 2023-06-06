using System;
using TMPro;
using UnityEngine;

public class SudokuCell : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text;

    [HideInInspector] public Vector2Int gridPosition;

    public int? number { get; private set; }

    public Action<SudokuCell, int?> onSetNumber;

    public void ClearNumber()
    {
        number = null;

        text.text = "";
    }

    public void SetNumber(int cellNumber)
    {
        number = cellNumber;

        text.text = number.ToString();

        onSetNumber?.Invoke(this, number);
    }

    public void OnClick()
    {
        if (number.HasValue)
            return;

        SetNumber(GameManager.selectedNumber);
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
