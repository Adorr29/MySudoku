using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text text;
    [Space]
    [SerializeField] private SudokuCellVisual cell;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [Space]
    [SerializeField] public byte number;

    public bool isVisible { get; private set; } = false;
    private bool mouseHover = false;
    private Color targetColor;

    private void OnEnable()
    {
        text.color = new Color(0, 0, 0, 0);
    }

    // Start is called before the first frame update
    private void Start()
    {
        text.text = number.ToString();

        if (isVisible == true)
            Show();
        else
            Hide();

        text.color = targetColor;
    }

    private void Update()
    {
        if (text.color != targetColor)
        {
            float speed = 10;

            if (mouseHover == true)
                speed = 20;

            text.color = Color.Lerp(text.color, targetColor, Time.deltaTime * speed);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SetNumber();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isVisible == true)
                Hide();
            else
                Show();
        }
    }

    private void SetNumber()
    {
        mouseHover = false;

        UpdateTextColor();

        cell.sudokuCell.SetNumber(number);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseHover = true;

        UpdateTextColor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseHover = false;

        UpdateTextColor();
    }

    public void Show()
    {
        isVisible = true;

        UpdateTextColor();
    }

    public void Hide()
    {
        isVisible = false;

        UpdateTextColor();
    }

    private void UpdateTextColor()
    {
        if (isVisible)
        {
            if (mouseHover == true)
                targetColor = new Color(0, 0, 0, 1); // TODO use param color
            else
                targetColor = new Color(0, 0, 0, 0.5f); // TODO use param color
        }
        else
        {
            if (mouseHover == true)
            {
                if (cell.HaveNoNumberButtonVisible() == true)
                    targetColor = new Color(0, 0, 0, 1); // TODO use param color
                else
                    targetColor = new Color(0, 0, 0, 0.25f); // TODO use param color
            }
            else
                targetColor.a = 0;
        }
    }
}
