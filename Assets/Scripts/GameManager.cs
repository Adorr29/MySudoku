using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator transitionsAnimator;
    public SudokuGridVisual sudokuGrid; // tmp

    public static GameManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        transitionsAnimator.Play("TransitionIn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackToMenu()
    {
        transitionsAnimator.Play("TransitionOut");

        Invoke(nameof(LoadMenuScene), 1);
    }

    private void LoadMenuScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
