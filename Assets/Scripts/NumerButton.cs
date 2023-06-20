using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumerButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text text;
    [Space]
    [SerializeField] private SudokuCell cell;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [Space]
    [SerializeField] private int number;

    public bool isVisible { get; private set; } = true;
    private bool mouseHover = false;
    private Color targetColor;

    // Start is called before the first frame update
    private void Start()
    {
        text.text = number.ToString();

        if (isVisible == true)
            Show();
        else
            Hide();
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
            if (isVisible || cell.NoNumberButtonVisible() == true)
                cell.SetNumber(number);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isVisible == true)
                Hide();
            else
                Show();
        }
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
                if (cell.NoNumberButtonVisible() == true)
                    targetColor = new Color(0, 0, 0, 1); // TODO use param color
                else
                    targetColor = new Color(0, 0, 0, 0.15f); // TODO use param color
            }
            else
                targetColor.a = 0;
        }
    }
}
