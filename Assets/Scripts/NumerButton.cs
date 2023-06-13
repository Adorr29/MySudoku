using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NumerButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text text;
    [Space]
    [SerializeField] private SudokuCell cell;
    [Space]
    [SerializeField] private int number;

    // Start is called before the first frame update
    private void Start()
    {
        text.text = number.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SetNumber();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ToggleNumber();
        }
    }

    private void SetNumber()
    {
        if (text.enabled == false)
            return;

        cell.SetNumber(number);
    }

    private void ToggleNumber() // TODE find better name
    {
        text.enabled = !text.enabled;
    }
}
