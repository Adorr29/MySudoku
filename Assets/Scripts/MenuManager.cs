using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Animator transitionsAnimator;

    private static MenuManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        transitionsAnimator.Play("TransitionIn");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunGame()
    {
        transitionsAnimator.Play("TransitionOut");

        Invoke(nameof(LoadGameScene), 1);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void Exit()
    { 
        Application.Quit();
    }
}
