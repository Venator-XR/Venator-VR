using System.Collections;
using UnityEngine;

/// <summary>
/// Orchestrates the final boss fight sequence including intro, combat, and end states.
/// </summary>
public class FinalFightManager : MonoBehaviour
{
    [Header("Actor References")]
    [SerializeField] private VampireFightBrain vampireBrain;
    [SerializeField] private MonoBehaviour playerController;
    
    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;
    
    [Header("Transitions")]
    [SerializeField] private CanvasGroup blackFadeCanvas;
    [SerializeField] private float introDelay = 3f;
    [SerializeField] private float endSequenceDelay = 2f;
    [SerializeField] private float fadeDuration = 1f;

    private IHealth _vampireHealth;
    private IHealth _playerHealth;

    private void Awake()
    {
        if (vampireBrain != null)
            _vampireHealth = vampireBrain.GetComponent<IHealth>();
        
        if (playerController != null)
            _playerHealth = playerController.GetComponent<IHealth>();
    }

    private void OnEnable()
    {
        if (_vampireHealth != null)
            _vampireHealth.OnDeath += OnVampireDefeated;
        
        if (_playerHealth != null)
            _playerHealth.OnDeath += OnPlayerDefeated;
    }

    private void OnDisable()
    {
        if (_vampireHealth != null)
            _vampireHealth.OnDeath -= OnVampireDefeated;
        
        if (_playerHealth != null)
            _playerHealth.OnDeath -= OnPlayerDefeated;
    }

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        // Disable combat during intro
        if (vampireBrain != null)
            vampireBrain.enabled = false;

        Debug.Log("Coffin opening...");
        // TODO: Trigger coffin opening animation
        
        yield return new WaitForSeconds(introDelay);

        // Start boss music and enable combat
        if (musicSource != null && bossMusic != null)
        {
            musicSource.clip = bossMusic;
            musicSource.Play();
        }
        
        if (vampireBrain != null)
            vampireBrain.enabled = true;

        Debug.Log("The hunt begins!");
    }

    private void OnVampireDefeated()
    {
        Debug.Log("Victory: The vampire has been defeated.");
        StartCoroutine(EndSequence(true));
    }

    private void OnPlayerDefeated()
    {
        Debug.Log("Defeat: The hunter has fallen.");
        StartCoroutine(EndSequence(false));
    }

    private IEnumerator EndSequence(bool victory)
    {
        // Stop combat
        if (vampireBrain != null)
            vampireBrain.enabled = false;

        // Change music
        if (musicSource != null)
        {
            musicSource.Stop();
            AudioClip endClip = victory ? victoryMusic : defeatMusic;
            if (endClip != null)
                musicSource.PlayOneShot(endClip);
        }

        yield return new WaitForSeconds(endSequenceDelay);

        // Fade to black (important for VR comfort)
        if (blackFadeCanvas != null)
        {
            float elapsed = 0;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                blackFadeCanvas.alpha = elapsed / fadeDuration;
                yield return null;
            }
            blackFadeCanvas.alpha = 1f;
        }

        Debug.Log(victory ? "Loading credits..." : "Restarting fight...");
        // TODO: Load appropriate scene
    }
}