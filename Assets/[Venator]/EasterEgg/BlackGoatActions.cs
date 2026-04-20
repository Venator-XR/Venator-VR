using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BlackGoatActions : MonoBehaviour
{
    XRBaseInteractable grab;
    AudioSource audioSource;
    public AudioClip[] audioClips;
    public GameObject hand;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        grab = GetComponent<XRBaseInteractable>();
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
