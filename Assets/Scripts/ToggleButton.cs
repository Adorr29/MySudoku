using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [Space]
    [SerializeField] private Sprite enableSprite;
    [SerializeField] private Sprite disableSprite;
    [SerializeField] private bool saveStateInPlayerPrefs;

    public bool enable { get; private set; }

    public Action<bool> onToggle;

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    private void Awake()
    {
        enable = PlayerPrefs.GetInt(name, 1) != 0;

        UpadateSprite();
    }

    private void Toggle()
    {
        enable = !enable;

        UpadateSprite();

        onToggle?.Invoke(enable);

        PlayerPrefs.SetInt(name, enable ? 1 : 0);
    }

    private void UpadateSprite()
    {
        if (enable)
            image.sprite = enableSprite;
        else
            image.sprite = disableSprite;
    }
}