using UnityEngine;
using UnityEngine.UI;

public class DifficultySlider : MonoBehaviour
{
    [SerializeField] Image backgroundColorImage;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        int difficulty = PlayerPrefs.GetInt("Difficulty", 1200);

        slider.value = difficulty;

        OnValueChanged(difficulty);
    }

    public void OnValueChanged(float value)
    {
        backgroundColorImage.fillAmount = value / slider.maxValue;

        PlayerPrefs.SetInt("Difficulty", (int)value);
    }
}
