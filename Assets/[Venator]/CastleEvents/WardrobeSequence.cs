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

    [Header("Vampire References")]
    GameObject vampire;
    ShapeshiftManager shapeshiftManager;
    NavMeshAgent vampireNavAgent;
    [SerializeField] Transform vampireStart;
    [SerializeField] Transform batDestination;
    [SerializeField] Transform vampireDestination;

    [Header("Audio")]
    [SerializeField] AudioClip enteringAudioClip;
    [SerializeField] AudioClip exitingAudioClip;
    [SerializeField] AudioSource audioSource;

    private XRKnobLever targetLever;

    void Start()
    {
        
        targetLever = wardrobe.GetComponentInChildren<XRKnobLever>();
        playerMobilityManager = GetComponent<PlayerMobilityManager>();
    }

    public IEnumerator WardrobeCoroutine()
    {
        // search for vampire
        vampire = GameObject.FindGameObjectWithTag("Enemy");
        if (vampire == null) Debug.LogWarning("vampire not found by tag");
        else
        {
            shapeshiftManager = vampire.GetComponentInChildren<ShapeshiftManager>();
            vampireNavAgent = vampire.GetComponent<NavMeshAgent>();
        }

        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, false);

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

        // tp vampire
        vampire.transform.position = vampireStart.position;
        vampire.transform.rotation = Quaternion.Euler(0, vampireStart.eulerAngles.y, 0);

        // fade from black
        fadeAnim.Play("fadeOut");
        yield return new WaitForSeconds(0.5f); // should be 0.5f, using more for testing

        // wait until VampireCoroutine completes
        yield return StartCoroutine(VampireCoroutine());

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

        flashlightController.enabled = true;
        flashlightController.ToggleLight();

        // tp player outside wardrobe looking at door
        playerMobilityManager.TeleportTo(outsideDestination);

        // play sfx: wardrobe opening | steps | wardrobe closing
        // audioSource.PlayOneShot(exitingAudioClip);

        // fade from black
        fadeAnim.Play("fadeOut");
        yield return new WaitForSeconds(0.5f);

        // InventoryTutorialManager.StartInventoryTutorial();

        yield break;
    }

    private IEnumerator VampireCoroutine()
    {
        Debug.Log("VampireCoroutine end");

        // transform into bat
        shapeshiftManager.Shapeshift();
        yield return new WaitForSeconds(1.5f);

        // fly through hole in debris to batDestination
        vampireNavAgent.speed = 1f;
        vampireNavAgent.SetDestination(batDestination.position);

        yield return new WaitForSeconds(3f); // time it takes to arrive

        // transform back inside the room where player clearly sees
        shapeshiftManager.Shapeshift();
        yield return new WaitForSeconds(1.5f);

        // move to vampireDestination
        vampireNavAgent.speed = 1.5f;
        vampireNavAgent.SetDestination(vampireDestination.position);

        // play door close SFX


        yield return new WaitForSeconds(1.5f); // time it takes until door closed + extra 1s

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
        }
    }
}
