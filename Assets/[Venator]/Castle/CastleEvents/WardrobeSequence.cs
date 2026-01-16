using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WardrobeSequence : MonoBehaviour
{
    [SerializeField] GameObject wardrobe;

    [Header("Player Refs")]
    public PlayerMobilityManager playerMobilityManager;
    public XRBaseInteractor handInteractor;
    public FlashlightController flashlightController;
    public PlayerHealth playerHealth;

    [Header("Destinations")]
    [SerializeField] Transform insideDestination;
    [SerializeField] Transform outsideDestination;

    [Header("Extras")]
    [SerializeField] Animator fadeAnim;
    public GameObject candles;
    public TutorialManager inventoryTutorialManager;
    public Material normalCurtainsMat;
    public GameObject curtains;

    [Header("Vampire Refs")]
    public GameObject vampire;
    ShapeshiftManager shapeshiftManager;
    NavMeshAgent vampireNavAgent;
    [SerializeField] Transform vampireStart;
    [SerializeField] Transform batDestination;
    [SerializeField] Transform vampireDestination;

    [Header("Next rooms")]
    public GameObject[] nextRooms;
    public XRKnobDoor nextDoorScript;

    [Header("Audio")]
    [SerializeField] AudioClip enteringAudioClip;
    [SerializeField] AudioClip exitingAudioClip;
    [SerializeField] AudioClip doorShutAudio;
    [SerializeField] AudioSource playerAudioSource;
    [SerializeField] AudioClip silentBreathingSFX;
    [SerializeField] AudioClip tenseBreathingSFX;
    [SerializeField] AudioClip stingerSFX;
    [SerializeField] GlobalSoundManager globalSoundManager;
    private AudioSource audioSource;

    private XRKnobLever targetLever;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public IEnumerator WardrobeCoroutine()
    {
        Debug.Log("WardrobeCoroutine()");
        shapeshiftManager = vampire.GetComponentInChildren<ShapeshiftManager>();
        vampireNavAgent = vampire.GetComponent<NavMeshAgent>();
        wardrobe.GetComponentInChildren<OutlineTrigger>().enabled = false;
        wardrobe.GetComponentInChildren<Outline>().enabled = false;
        targetLever = wardrobe.GetComponentInChildren<XRKnobLever>();

        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, true);
        Debug.Log("disable movement and camera");

        // Stop Music
        globalSoundManager.StopSequence();

        // play sfx: wardrobe opening | steps | wardrobe closing
        playerAudioSource.PlayOneShot(enteringAudioClip);

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

        playerHealth.Heal();

        if (targetLever != null)
        {
            targetLever.ForceEndInteractionAndFade();
        }

        flashlightController.TurnOff();
        flashlightController.enabled = false;

        // tp player inside wardrobe looking through the hole
        Debug.Log("TeleportTo(" + insideDestination + ")");
        playerMobilityManager.ForceTeleport(insideDestination);

        // tp vampire, disable nav agent to evade smooth movement for this
        vampireNavAgent.enabled = false;
        vampire.transform.position = vampireStart.position;
        vampire.transform.rotation = Quaternion.Euler(0, vampireStart.eulerAngles.y, 0);

        yield return new WaitForSeconds(5f);

        targetLever.value = 0;

        // transform into bat
        shapeshiftManager.Shapeshift();
        vampireNavAgent.enabled = true;

        // fade from black
        fadeAnim.Play("fadeOut");

        // play sfx: silent breathing
        playerAudioSource.clip = silentBreathingSFX;
        playerAudioSource.loop = true;
        playerAudioSource.Play();

        yield return new WaitForSeconds(3f);

        // wait until VampireCoroutine completes
        yield return StartCoroutine(VampireCoroutine());

        // fade to black
        fadeAnim.Play("fadeIn");

        flashlightController.enabled = true;

        // tp player outside wardrobe looking at door
        playerMobilityManager.ForceTeleport(outsideDestination);
        wardrobe.GetComponentInChildren<XRKnobLever>().enabled = false;

        // play sfx: wardrobe opening | steps | wardrobe closing
        audioSource.PlayOneShot(exitingAudioClip);
        yield return new WaitForSeconds(5f);

        // fade from black
        fadeAnim.Play("fadeOut");
        inventoryTutorialManager.enabled = true;
        yield return new WaitForSeconds(0.5f);


        // next door and rooms ennabled now
        nextDoorScript.enabled = true;
        foreach (GameObject room in nextRooms) room.SetActive(true);

        globalSoundManager.PlayNextSequence();

        playerAudioSource.clip = tenseBreathingSFX;
        playerAudioSource.loop = true;
        playerAudioSource.Play();

        yield break;
    }

    //----------------------------------------

    private IEnumerator VampireCoroutine()
    {
        Debug.Log("VampireCoroutine start");

        // fly through hole in debris to batDestination
        vampireNavAgent.speed = 1f;
        vampireNavAgent.SetDestination(batDestination.position);

        yield return new WaitForSeconds(4f);

        // transform back inside the room where player clearly sees
        shapeshiftManager.Shapeshift();
        // turn off candles
        yield return new WaitForSeconds(1f);
        playerAudioSource.loop = false;
        playerAudioSource.PlayOneShot(stingerSFX);

        candles.SetActive(false);

        yield return new WaitForSeconds(2f);

        // move to vampireDestination
        vampireNavAgent.speed = 1.5f;
        vampireNavAgent.SetDestination(vampireDestination.position);
        yield return new WaitForSeconds(2f);

        playerAudioSource.clip = silentBreathingSFX;
        playerAudioSource.loop = true;
        playerAudioSource.Play();

        // play curtains moving SFX and change material
        curtains.GetComponent<Renderer>().material = normalCurtainsMat;

        yield return new WaitForSeconds(3f);

        // play door close SFX
        playerAudioSource.PlayOneShot(doorShutAudio);

        yield return new WaitForSeconds(2f);

        vampire.SetActive(false);
        Debug.Log("VampireCoroutine end");
        yield break;
    }
}
