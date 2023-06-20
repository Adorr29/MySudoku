using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public static AudioSource musicSource => instance.audioSource;

    public static MusicPlayer instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        { 
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
}
