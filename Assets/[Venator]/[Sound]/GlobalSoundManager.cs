using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GlobalSoundManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private float fadeDuration = 2.0f;
    [SerializeField] private float targetVolume = 1.0f;

    public AudioSource _audioSource;
    private Coroutine _fadeCoroutine;
    private int _currentIndex = -1; // Starts at -1 so first play hits index 0

    void Awake()
    {
        _audioSource.volume = 0f;
        _audioSource.loop = true; // Loops current track until Stop is called
    }

    void Start()
    {
        PlayNextSequence();
    }

    // Call this to start the NEXT track with Fade In
    public void PlayNextSequence()
    {
        if (tracks.Length == 0) return;

        // Calculate next index (loop back to 0 at the end)
        _currentIndex = (_currentIndex + 1) % tracks.Length;

        // Setup audio source
        _audioSource.Stop(); // Ensure clean cut before swapping
        _audioSource.clip = tracks[_currentIndex];
        _audioSource.volume = 0f; 
        _audioSource.Play();

        FadeTo(targetVolume);
    }

    // Call this to Fade Out and Stop
    public void StopSequence()
    {
        FadeTo(0f, () => 
        {
            _audioSource.Stop();
        });
    }

    // Generic Fade Coroutine handler
    private void FadeTo(float targetVol, System.Action onComplete = null)
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine(targetVol, onComplete));
    }

    private IEnumerator FadeRoutine(float targetVol, System.Action onComplete)
    {
        float startVol = _audioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVol, targetVol, timer / fadeDuration);
            yield return null;
        }

        _audioSource.volume = targetVol;
        onComplete?.Invoke();
    }
}