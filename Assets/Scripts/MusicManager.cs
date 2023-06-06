using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] ToggleButton muteMusicToggleButton;

    private AudioClip[] musics;
    private bool musicEnabled;

    // Start is called before the first frame update
    void Start()
    {
        musics = Resources.LoadAll<AudioClip>("Musics");

        muteMusicToggleButton.onToggle += OnMuteMusicToggle;

        OnMuteMusicToggle(muteMusicToggleButton.enable);
    }

    private void PlayRandomMusic()
    {
        MusicPlayer.musicSource.clip = musics[Random.Range(0, musics.Length)];

        MusicPlayer.musicSource.Play();

        if (musicEnabled)
            Invoke(nameof(PlayRandomMusic), MusicPlayer.musicSource.clip.length);
    }

    private void OnMuteMusicToggle(bool enable)
    {
        musicEnabled = enable;

        if (musicEnabled)
        {
            MusicPlayer.musicSource.volume = 0.5f;

            if (MusicPlayer.musicSource.isPlaying == false)
                PlayRandomMusic();
        }
        else
            MusicPlayer.musicSource.volume = 0;
    }
}
