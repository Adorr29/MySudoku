using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private AudioClip[] musics;
    private bool musicEnabled;

    public static MusicPlayer instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        { 
            Destroy(gameObject);
            return;
        }

        instance = this;

        musics = Resources.LoadAll<AudioClip>("Musics");

        SceneManager.activeSceneChanged += OnSceneChange;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ToggleButton muteMusicToggleButton = GetMuteMusicToggleButton();

        if (muteMusicToggleButton.enable && !musicEnabled)
            PlayRandomMusic();
    }

    private void OnSceneChange(Scene current, Scene next)
    {
        Debug.Log("OnSceneChange " + current.name + " -> " + next.name);

        ToggleButton muteMusicToggleButton = GetMuteMusicToggleButton();

        muteMusicToggleButton.onToggle += OnMuteMusicToggle;
    }

    private ToggleButton GetMuteMusicToggleButton()
    {
        return GameObject.Find("MuteMusicToggleButton").GetComponent<ToggleButton>();
    }

    private void OnMuteMusicToggle(bool enable)
    {
        musicEnabled = enable;

        if (musicEnabled)
            PlayRandomMusic();
        else
            StopMusic();
    }

    private void PlayRandomMusic()
    {
        audioSource.clip = musics[Random.Range(0, musics.Length)];

        audioSource.Play();

        Invoke(nameof(PlayRandomMusic), audioSource.clip.length);
    }

    private void StopMusic()
    {
        audioSource.Stop();

        CancelInvoke(nameof(PlayRandomMusic));
    }
}
