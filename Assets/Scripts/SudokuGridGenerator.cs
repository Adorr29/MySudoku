using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuGridGenerator
{
    public static List<Type> solvingTechniqueTypes => new List<Type> {
        typeof(LastDigit),
        typeof(FullHouse),
        typeof(HiddenSingle),
        typeof(NakedSingle)
    };

    public static SudokuGrid CreateGrid(int targetDifficulty)
    {
        int solvingTechniqueCount = 1;

        for (int i = 0; i < 100; i++)
        {
            SudokuGrid sudokuGrid = AttemptCreateGrid(targetDifficulty, solvingTechniqueTypes.GetRange(0, solvingTechniqueCount));

            if (sudokuGrid != null)
                return sudokuGrid;

            if (solvingTechniqueCount < solvingTechniqueTypes.Count)
                solvingTechniqueCount++;
        }

        return null;
    }

    private static SudokuGrid AttemptCreateGrid(int targetDifficulty, List<Type> solvingTechniqueTypes)
    {
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

        SudokuGrid sudokuGrid = SudokuGrid.CreateEmptyGrid();

        timer.Restart();
        bool gridIsFilled = FillGrid(sudokuGrid);
        timer.Stop();

        Debug.Log("FillGrid time : " + timer.Elapsed.ToString());

        if (gridIsFilled == false)
            return null;

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                sudokuGrid.grid[i, j].expectedNumber = sudokuGrid.grid[i, j].number.Value;

        timer.Restart();
        int solutionDifficulty = RemoveCells(sudokuGrid, targetDifficulty, solvingTechniqueTypes);
        timer.Stop();

        Debug.Log("RemoveCells time : " + timer.Elapsed.ToString());

        int difficultyDifference = Mathf.Abs(solutionDifficulty - targetDifficulty);

        Debug.Log("difficultyDifference : " + difficultyDifference + ", must be <= " + 500);

        if (difficultyDifference > 500)
            return null;

        return sudokuGrid;
    }

    private static int RemoveCells(SudokuGrid sudokuGrid, int targetDifficulty, List<Type> solvingTechniqueTypes)
    {
        int? solutionDifficulty = null;
        IEnumerable<SudokuCell> cells = sudokuGrid.GetCells().OrderBy(p => UnityEngine.Random.value);

        foreach (SudokuCell cell in cells)
        {
            byte cellNumber = cell.number.Value;

            sudokuGrid.ClearCellNumber(cell);

            int? currentSolutionDifficulty = Solve(sudokuGrid.CreateCopy(), solvingTechniqueTypes);

            if (currentSolutionDifficulty.HasValue == false)
            {
                sudokuGrid.SetNumberAndUpdateCandidate(cell, cellNumber);
                continue;
            }

            if (solutionDifficulty.HasValue == false || Mathf.Abs(currentSolutionDifficulty.Value - targetDifficulty) < Mathf.Abs(solutionDifficulty.Value - targetDifficulty))
            {
                solutionDifficulty = currentSolutionDifficulty;
            }
            else
            {
                sudokuGrid.SetNumberAndUpdateCandidate(cell, cellNumber);
            }
        }

        return solutionDifficulty.Value;
    }

    private static int? Solve(SudokuGrid sudokuGrid, List<Type> solvingTechniqueTypes)
    {
        int solutionDifficulty = 0;

        SolvingTechnique[] solvingTechniques = new SolvingTechnique[solvingTechniqueTypes.Count];

        for (int i = 0; i < solvingTechniqueTypes.Count; i++)
        {
            solvingTechniques[i] = (SolvingTechnique)Activator.CreateInstance(solvingTechniqueTypes[i], sudokuGrid);
        }

        solvingTechniques = solvingTechniques.OrderBy(t => t.difficulty).ToArray();

        while (SudokuGridIsSolved(sudokuGrid) == false)
        {
            int? appliedSolvingTechniqueDifficulty = TryToApplySolvingTechniques(solvingTechniques);

            if (appliedSolvingTechniqueDifficulty == null)
                return null;

            solutionDifficulty += appliedSolvingTechniqueDifficulty.Value;
        }

        return solutionDifficulty;
    }

    private static int? TryToApplySolvingTechniques(SolvingTechnique[] solvingTechniques)
    {
        foreach (SolvingTechnique solvingTechnique in solvingTechniques)
            if (solvingTechnique.Find() == true)
            {
                solvingTechnique.Apply();

                return solvingTechnique.difficulty;
            }

        return null;
    }

    private static bool SudokuGridIsSolved(SudokuGrid sudokuGrid)
    {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                if (sudokuGrid.grid[i, j].number == null)
                    return false;

        return true;
    }

    private static bool FillGrid(SudokuGrid sudokuGrid)
    {
        for (int i = 0; i < 10000; i++)
        {
            if (AttemptFillGrid(sudokuGrid) == true)
                return true;

            sudokuGrid.Clear();
        }

        Debug.LogWarning("FillGrid faild !");

        return false;
    }

    private static bool AttemptFillGrid(SudokuGrid sudokuGrid)
    {
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                SudokuCell cell = sudokuGrid.grid[i, j];

                if (cell.HaveCandidateNumbers() == false)
                    return false;

                byte randomCandidateNumber = cell.GetRandomCandidateNumber();
                sudokuGrid.SetNumberAndUpdateCandidate(cell, randomCandidateNumber);
            }

        return true;
    }

    public static void PrintGrid(SudokuGrid sudokuGrid)
    {
        string str = "";

        for (int j = 0; j < 9; j++)
        {
            for (int i = 0; i < 9; i++)
            {
                str += "   ";

                if (sudokuGrid.grid[i, j].number == null)
                    str += "_";
                else
                    str += sudokuGrid.grid[i, j].number.ToString();
            }

            str += "\n";
        }

        Debug.Log(str);
    }
}
