using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WardrobeSequence : MonoBehaviour
{
    PlayerMobilityManager playerMobilityManager;

    [Header("References")]
    [SerializeField] Transform insideDestination;
    [SerializeField] Transform outsideDestination;
    [SerializeField] Animator fadeAnim;
    public XRBaseInteractor handInteractor;
    [SerializeField] GameObject wardrobe;
    public FlashlightController flashlightController;
    public GameObject candles;
    public XRKnobDoor nextDoorScript;
    public TutorialManager inventoryTutorialManager;
    public Material normalCurtainsMat;
    public GameObject curtains;

    [Header("Vampire References")]
    public GameObject vampire;
    ShapeshiftManager shapeshiftManager;
    NavMeshAgent vampireNavAgent;
    [SerializeField] Transform vampireStart;
    [SerializeField] Transform batDestination;
    [SerializeField] Transform vampireDestination;

    [Header("Next rooms")]
    public GameObject[] nextRooms;

    [Header("Audio")]
    [SerializeField] AudioClip enteringAudioClip;
    [SerializeField] AudioClip exitingAudioClip;
    [SerializeField] AudioClip doorShutAudio;
    [SerializeField] AudioSource audioSource;

    private XRKnobLever targetLever;

    void Start()
    {

        targetLever = wardrobe.GetComponentInChildren<XRKnobLever>();
        playerMobilityManager = GetComponent<PlayerMobilityManager>();
    }

    public IEnumerator WardrobeCoroutine()
    {
        shapeshiftManager = vampire.GetComponentInChildren<ShapeshiftManager>();
        vampireNavAgent = vampire.GetComponent<NavMeshAgent>();

        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, true);

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

        ForceRelease();
        flashlightController.TurnOff();
        flashlightController.enabled = false;

        // tp player inside wardrobe looking through the hole
        playerMobilityManager.TeleportTo(insideDestination);

        // play sfx: wardrobe opening | steps | wardrobe closing
        // audioSource.PlayOneShot(enteringAudioClip);

        // tp vampire, disable nav agent to evade smooth movement for this
        vampireNavAgent.enabled = false;
        vampire.transform.position = vampireStart.position;
        vampire.transform.rotation = Quaternion.Euler(0, vampireStart.eulerAngles.y, 0);
        vampireNavAgent.enabled = true;

        // transform into bat
        shapeshiftManager.Shapeshift();

        // fade from black
        fadeAnim.Play("fadeOut");
        yield return new WaitForSeconds(4f);

        // wait until VampireCoroutine completes
        yield return StartCoroutine(VampireCoroutine());

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

        flashlightController.enabled = true;

        // tp player outside wardrobe looking at door
        playerMobilityManager.TeleportTo(outsideDestination);

        // play sfx: wardrobe opening | steps | wardrobe closing
        // audioSource.PlayOneShot(exitingAudioClip);

        // fade from black
        fadeAnim.Play("fadeOut");
        yield return new WaitForSeconds(0.5f);

        inventoryTutorialManager.enabled = true;

        // next door and rooms ennabled now
        nextDoorScript.enabled = true;
        foreach (GameObject room in nextRooms) room.SetActive(true);

        yield break;
    }

    private IEnumerator VampireCoroutine()
    {
        Debug.Log("VampireCoroutine start");

        // fly through hole in debris to batDestination
        vampireNavAgent.speed = 1f;
        vampireNavAgent.SetDestination(batDestination.position);

        yield return new WaitForSeconds(5f);

        // transform back inside the room where player clearly sees
        shapeshiftManager.Shapeshift();
        // turn off candles
        yield return new WaitForSeconds(1f);
        candles.SetActive(false);

        yield return new WaitForSeconds(2f);

        // move to vampireDestination
        vampireNavAgent.speed = 1.5f;
        vampireNavAgent.SetDestination(vampireDestination.position);
        yield return new WaitForSeconds(2f);

        // play curtains moving SFX and change material
        curtains.GetComponent<Renderer>().material = normalCurtainsMat;

        yield return new WaitForSeconds(3f);

        // play door close SFX
        audioSource.PlayOneShot(doorShutAudio);

        yield return new WaitForSeconds(2f);

        vampire.SetActive(false);
        Debug.Log("VampireCoroutine end");
        yield break;
    }

    void ForceRelease()
    {
        if (targetLever != null && targetLever.isSelected)
        {
            // Obtenemos el manager y el interactor que la tiene agarrada
            var manager = targetLever.interactionManager;
            var interactor = targetLever.interactorsSelecting[0]; // La primera mano que lo agarra

            manager.SelectExit(interactor, targetLever);
            wardrobe.GetComponentInChildren<XRKnobLever>().value = 0;
        }
    }
}
