using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour
{
	[Header("References")]
	public AudioSource audioPlayer;

	static AudioSource globalAudioPlayer;

    private void Start()
    {
        globalAudioPlayer = audioPlayer;
    }

    public static void PlaySound(AudioClip sound)
	{
        globalAudioPlayer.PlayOneShot(sound);
    }
}

