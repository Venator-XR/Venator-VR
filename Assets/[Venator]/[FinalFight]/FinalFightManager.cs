using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Orchestrates the final boss fight sequence including intro, combat, and end states.
/// </summary>
public class FinalFightManager : MonoBehaviour
{
    [Header("Vampire Brain")]
    [SerializeField] private VampireFightBrain vampireBrain;

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
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip bossMusic;

    private IHealth _vampireHealth;
    // player privates
    private IHealth _playerHealth;
    private PlayerMobilityManager _playerMobilityManager;
    private InventoryController _inventoryController;


    private void Awake()
    {

        if (vampireBrain == null) Debug.LogError("vampireBrain not assigned");
        else _vampireHealth = vampireBrain.GetComponent<IHealth>();

        if (player == null) Debug.LogError("player not assigned");
        else
        {
            _playerHealth = player.GetComponent<IHealth>();
            _inventoryController = player.GetComponent<InventoryController>();
        }

        _playerMobilityManager = GetComponent<PlayerMobilityManager>();
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
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        transiton.Play("fadeIn");
        yield return new WaitForSeconds(2f);

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
        yield return new WaitForSeconds(1f);

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
            // TODO: music slowly dies
        }

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