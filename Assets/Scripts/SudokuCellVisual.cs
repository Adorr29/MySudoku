using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class SudokuCellVisual : MonoBehaviour
{
    [SerializeField] private SudokuGridVisual sudokuGridVisual;
    [Space]
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text text;
    [SerializeField] public ColorAnimation frame;
    [Space]
    [SerializeField] private GameObject numberButtonsParent;
    [SerializeField] private NumerButton[] numberButtons;

    [HideInInspector] public Vector2Int gridPosition;
    [HideInInspector] public Color targetColor;
    [HideInInspector] public bool playerNumber;

    public SudokuCell sudokuCell { get; private set; }

    private void Start()
    {
        targetColor = text.color;
    }

    private void Update()
    {
        if (text.color != targetColor)
        {
            text.color = Color.Lerp(text.color, targetColor, Time.deltaTime * 10);
        }
    }

    public void Init(SudokuCell cell)
    {
        sudokuCell = cell;

        if (cell.number != null)
        {
            text.text = cell.number.ToString();

            numberButtonsParent.SetActive(false);
        }
        else
        {
            foreach (byte candidateNumber in cell.candidateNumbers)
                OnAddCandidateNumber(cell, candidateNumber);
        }

        sudokuCell.onRemoveNumber += OnRemoveNumber;
        sudokuCell.onSetNumber += OnSetNumber;
        sudokuCell.onAddCandidateNumber += OnAddCandidateNumber;
        sudokuCell.onRemoveCandidateNumber += OnRemoveCandidateNumber;
    }

    private void OnRemoveNumber(SudokuCell sudokuCell)
    {
        text.text = null;

        numberButtonsParent.SetActive(true);
    }

    private void OnSetNumber(SudokuCell sudokuCell, byte number)
    {
        text.text = number.ToString();
        text.font = sudokuGridVisual.playerNumberFont;
        playerNumber = true;

        numberButtonsParent.SetActive(false);
    }

    private void OnAddCandidateNumber(SudokuCell sudokuCell, byte candidateNumber)
    {
        numberButtons[candidateNumber - 1].Show();
    }

    private void OnRemoveCandidateNumber(SudokuCell sudokuCell, byte candidateNumber)
    {
        numberButtons[candidateNumber - 1].Hide();
    }

    public void RemoveCellNumber()
    {
        sudokuCell.RemoveNumber();
    }

    public bool HaveNoNumberButtonVisible()
    {
        return numberButtons.Any(b => b.isVisible) == false;
    }

    public void ClearNumber()
    {
        if (playerNumber == false)
            return;

        PlayAnimation("ClearNumber");

        Invoke(nameof(RemoveCellNumber), 0.5f);
    }

    public void BadNumber()
    {
        PlayAnimation("BadNumber");

        Invoke(nameof(RemoveCellNumber), 0.5f);
    }

    public void GoodNumber()
    {
        PlayAnimation("GoodNumber");
    }

    public void HighlightCell()
    {
        PlayAnimation("HighlightCell");
    }

    private void PlayAnimation(string animationName)
    {
        animator.enabled = true;

        animator.Play(animationName, 0, 0);

        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        Invoke(nameof(DisenableAnimator), animationDuration);
    }

    private void DisenableAnimator()
    {
        animator.enabled = false;
    }
}
