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
        grab.selectEntered.AddListener(Selected);
        grab.selectEntered.AddListener(Deselected);
    }

    public void PlaySound()
    {
        Debug.Log("Playing sound");
        int randomIndex = Random.Range(0, 100) < 80 ? Random.Range(0, 5) : 5;
        audioSource.PlayOneShot(audioClips[randomIndex]);
        if (randomIndex == 5) KillVampire();
    }

    public void Selected(SelectEnterEventArgs args)
    {
        transform.position += new Vector3(0, -1f, 0);
        hand.SetActive(true);
    }

    public void Deselected(SelectEnterEventArgs args)
    {
        hand.SetActive(false);
    }

    // secret hehehe
    public void KillVampire()
    {
        Debug.Log("This does nothing yet");
    }
}
