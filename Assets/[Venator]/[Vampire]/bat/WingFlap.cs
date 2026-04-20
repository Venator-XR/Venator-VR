using UnityEngine;

public class WingFlap : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFXs")]
    [SerializeField] private AudioClip[] sfx;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFlap()
    {
        audioSource.PlayOneShot(sfx[Random.Range(0, sfx.Length)]);
    }
}
