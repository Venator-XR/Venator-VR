using System.Collections;
using UnityEngine;

public class BlackGoatActions : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] audioClips;
    public GameObject hand;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        Debug.Log("Playing sound");
        int randomIndex = Random.Range(0, 100) < 80 ? Random.Range(0, 5) : 5;
        audioSource.PlayOneShot(audioClips[randomIndex]);
        if (randomIndex == 5) KillVampire();
    }

    public void Selected()
    {
        transform.position += new Vector3(0, -1f, 0);
        hand.SetActive(true);
    }

    public void Deselected()
    {
        hand.SetActive(false);
    }

    // secret hehehe
    public void KillVampire()
    {
        Debug.Log("This does nothing yet");
    }
}
