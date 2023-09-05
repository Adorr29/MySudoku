using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator transitionsAnimator;
    public SudokuGridVisual sudokuGridVisual; // tmp

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        GenerateSudokuGrid();

        transitionsAnimator.Play("TransitionIn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackToMenu()
    {
        transitionsAnimator.Play("TransitionOut");

        Invoke(nameof(LoadMenuScene), 1);
    }

    private void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }

    private void GenerateSudokuGrid()
    {
        int targetDifficulty = PlayerPrefs.GetInt("Difficulty", 1200);

        SudokuGrid sudokuGrid = SudokuGridGenerator.CreateGrid(targetDifficulty);

        sudokuGrid.ClearAllCandidateNumbers();

        SudokuGridGenerator.PrintGrid(sudokuGrid); // tmp

        sudokuGridVisual.Init(sudokuGrid);
    }
}
