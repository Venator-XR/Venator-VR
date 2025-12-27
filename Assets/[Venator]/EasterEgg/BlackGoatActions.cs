using System.Collections;
using UnityEngine;

public class BlackGoatActions : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] audioClips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        Debug.Log("Playing sound");
        int randomIndex = Random.Range(0, 100) < 80 ? Random.Range(0, 4) : 4;
        audioSource.PlayOneShot(audioClips[randomIndex]);
        if (randomIndex == 4) KillVampire();
    }

    // secret hehehe
    public void KillVampire()
    {
        Debug.Log("This does nothing yet");
    }
}
