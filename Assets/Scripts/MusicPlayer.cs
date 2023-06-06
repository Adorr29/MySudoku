using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public static AudioSource musicSource => instance.audioSource;

    private static MusicPlayer instance;

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
