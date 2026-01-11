using UnityEngine;

public class TallActions : MonoBehaviour
{
    public AudioClip sfx;
    private Animator _animator;
    private AudioSource _audioSource;
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    public void Move()
    {
        _animator.SetTrigger("move");
        _audioSource.PlayOneShot(sfx);
    }
}
