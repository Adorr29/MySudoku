using UnityEngine;
using UnityEngine.UI;

public class ColorAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float speed = 10;

    private Color targetColor = new Color(0, 0, 0, 0);

    // Update is called once per frame
    private void Update()
    {
        if (image.color != targetColor)
        {
            image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * speed);
        }
    }

    public void Animate(Color startColor, Color endColor)
    {
        image.color = startColor;
        targetColor = endColor;
    }

    public void FadeOut()
    {
        targetColor.a = 0;
    }
}
