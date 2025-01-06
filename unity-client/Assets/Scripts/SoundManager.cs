using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject play;
    public GameObject mute;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Mute the audio

    public void Start() {
        DontDestroyOnLoad(transform.parent);
    }
    public void MuteAudio()
    {
        if (audioSource != null)
        {
            audioSource.mute = true;
            play.SetActive(true);
            mute.SetActive(false);
        }
    }

    // Unmute the audio
    public void UnmuteAudio()
    {
        if (audioSource != null)
        {
            audioSource.mute = false;
            play.SetActive(false);
            mute.SetActive(true);
        }
    }
}
