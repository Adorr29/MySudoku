using System.Collections;
using UnityEngine;

public abstract class SolvingTechnique
{
    public abstract int difficulty { get; }

    public bool isAnimationPlaying { get; protected set; }
    public bool skipAnimation;

    protected SudokuGrid sudokuGrid;

    public SolvingTechnique(SudokuGrid sudokuGrid)
    {
        this.sudokuGrid = sudokuGrid;
    }

    public abstract bool Find();

    public abstract IEnumerator DisplayHelp();

    public abstract void Apply();

    protected WaitForSeconds Wait(float seconds)
    {
        if (skipAnimation)
            return null;

        return new WaitForSeconds(seconds);
    }
}
