public abstract class SolvingTechnique
{
    protected SudokuGrid sudokuGrid;

    public SolvingTechnique(SudokuGrid sudokuGrid)
    {
        this.sudokuGrid = sudokuGrid;
    }

    public abstract bool Apply();
}
