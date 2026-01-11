using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Orchestrates the final boss fight sequence including intro, combat, and end states.
/// </summary>
public class FinalFightManager : MonoBehaviour
{
    [Header("Vampire Refs")]
    [SerializeField] private VampireFightBrain vampireBrain;
    [SerializeField] private Animator vampAnimator;
    [SerializeField] private ParticleSystem vampirePS;
    [SerializeField] private Animator coffinAnimator;
    [SerializeField] private GameObject[] candelabra;
    [SerializeField] private AudioSource vampAudioSource;
    [SerializeField] private AudioClip vampireHeartbeat;
    [SerializeField] private AudioClip laughSFX;

    [Header("Player References")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform playerFightStartPos;
    [SerializeField] private InventoryItemData pistolData;
    [SerializeField] private HandEquipmentManager handEquipmentManager;

    [Header("Screens")]
    [SerializeField] private string victorySceneName = "";
    [SerializeField] private string deafeatSceneName = "";

    [Header("Extra values")]
    [SerializeField] private float introDelay = 2f;
    [SerializeField] private float endSequenceDelay = 2f;

    [Header("Transition")]
    [SerializeField] private Animator transiton;

    [Header("Audio")]
    [SerializeField] private GlobalSoundManager globalSoundManager;
    public AudioClip coffinOpenSFX;

    private IHealth _vampireHealth;
    // player privates
    private IHealth _playerHealth;
    private PlayerMobilityManager _playerMobilityManager;
    private InventoryController _inventoryController;


    private void Awake()
    {
        if(vampirePS != null) vampirePS.Stop(); 
        if (vampireBrain == null) Debug.LogError("vampireBrain not assigned");
        else _vampireHealth = vampireBrain.GetComponent<IHealth>();

        if (player == null) Debug.LogError("player not assigned");
        else
        {
            _playerHealth = player.GetComponent<IHealth>();
            _inventoryController = player.GetComponent<InventoryController>();
        }

        _playerMobilityManager = GetComponent<PlayerMobilityManager>();

        vampAnimator.SetTrigger("coffinIddle");
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

    public void StartFight()
    {
        globalSoundManager.StopSequence();
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        transiton.Play("fadeIn");
        yield return new WaitForSeconds(2f);

        vampAudioSource.PlayOneShot(vampireHeartbeat);

        // Disable player Mobility
        _playerMobilityManager.SetPlayerMobility(false, true);

        // Set player pos and rotation
        player.transform.position = playerFightStartPos.position;

        // equip pistol and disable inventory
        handEquipmentManager.EquipItem(pistolData);
        _inventoryController.enabled = false;

        // Make sure brain is disabled
        if (vampireBrain != null)
            vampireBrain.enabled = false;

        transiton.Play("fadeOut");
        Debug.Log("Coffin opening...");
        vampAudioSource.PlayOneShot(laughSFX);
        vampAudioSource.PlayOneShot(coffinOpenSFX);
        vampAnimator.SetTrigger("coffinExit");
        coffinAnimator.SetTrigger("open");

        yield return new WaitForSeconds(introDelay);

        foreach(GameObject c in candelabra)
        {
            Light[] candles = c.GetComponentsInChildren<Light>();
            foreach(Light candle in candles) StartCoroutine(FadeLightOut(candle));
        }

        if(vampirePS != null) vampirePS.Play();

        if (vampireBrain != null) vampireBrain.enabled = true;

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

    private IEnumerator FadeLightOut(Light light)
    {
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        float initialIntensity = light.intensity;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            light.intensity = Mathf.Lerp(initialIntensity, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        light.enabled = false;
    }

    private IEnumerator EndSequence(bool victory)
    {
        // Stop combat
        if (vampireBrain != null)
            vampireBrain.enabled = false;

        // Change music
        globalSoundManager.StopSequence();

        yield return new WaitForSeconds(endSequenceDelay);

        // Fade to black (important for VR comfort)
        transiton.Play("fadeOut");
        yield return new WaitForSeconds(1f);

        if (victory)
            SceneManager.LoadScene(victorySceneName);
        else
            SceneManager.LoadScene(deafeatSceneName);
    }
}