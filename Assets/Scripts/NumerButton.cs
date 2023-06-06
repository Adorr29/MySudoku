using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class NumerButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    [Space]
    [SerializeField] private Color unselectdColor;
    [SerializeField] private Color selectdColor;
    [Space]
    [SerializeField] private int number;
    [SerializeField] private InputActionReference shortcut;

    private void OnEnable()
    {
        GameManager.onSelectNumber += OnSelectNumber;
        shortcut.action.started += OnShortcutPressed;
    }

    private void OnDisable()
    {
        GameManager.onSelectNumber -= OnSelectNumber;
        shortcut.action.started -= OnShortcutPressed;
    }

    // Start is called before the first frame update
    private void Start()
    {
        text.text = number.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.SelectNumber(number);
    }

    private void OnShortcutPressed(InputAction.CallbackContext obj)
    {
        GameManager.SelectNumber(number);
    }

    private void OnSelectNumber()
    {
        if (GameManager.selectedNumber == number)
            image.color = selectdColor;
        else
            image.color = unselectdColor;
    }
}
