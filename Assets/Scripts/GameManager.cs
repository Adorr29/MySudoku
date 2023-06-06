using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int selectedNumber { get; private set; }

    public static Action onSelectNumber;

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        SelectNumber(PlayerPrefs.GetInt(nameof(selectedNumber), 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SelectNumber(int number)
    {
        selectedNumber = number;

        onSelectNumber?.Invoke();

        PlayerPrefs.SetInt(nameof(selectedNumber), selectedNumber);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
