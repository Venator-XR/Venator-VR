using UnityEngine;
using System;

public class LightningController : MonoBehaviour
{
    public AudioClip[] thunders;
    private AudioSource _audioSource;
    private Collider _collider;

    public event Action OnLightningStrike;

    void Awake()
    {
        _audioSource = GetComponentInChildren<AudioSource>();
        _collider = GetComponent<BoxCollider>();
    }

    public void PlaySFX(int value)
    {
        _audioSource.PlayOneShot(thunders[value - 1]);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("OnLightningStrike.Invoke()");
            OnLightningStrike?.Invoke();

            _collider.enabled = false;
            Destroy(gameObject, 7f);
        }
    }
}
