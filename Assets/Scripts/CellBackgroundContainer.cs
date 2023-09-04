using UnityEngine;

public class CellBackgroundContainer : MonoBehaviour
{
    public ColorAnimation[] cellBackgrounds;

    public ColorAnimation[,] cellBackgroundGrid { get; private set; }

    private void Awake()
    {
        cellBackgroundGrid = new ColorAnimation[9, 9];

        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
                cellBackgroundGrid[i, j] = cellBackgrounds[i * 9 + j];
    }
}
