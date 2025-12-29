using System.Collections;
using UnityEngine;
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

    [Header("Vampire References")]
    [SerializeField] GameObject vampire;
    [SerializeField] ShapeshiftManager shapeshiftManager;
    [SerializeField] Transform vampireDestination;
    [SerializeField] Transform batDestination;
    [SerializeField] Transform vampFinalDestination;

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
        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, false);

        // fade to black

        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

        ForceRelease();

        // tp player inside wardrobe looking through the hole
        playerMobilityManager.TeleportTo(insideDestination);

        // play sfx: wardrobe opening | steps | wardrobe closing
        // audioSource.PlayOneShot(enteringAudioClip);

        // tp vampire
        // vampire.transform.position = vampireDestination.position;
        // vampire.transform.rotation = Quaternion.Euler(0, vampireDestination.eulerAngles.y, 0);

        // fade from black
        fadeAnim.Play("fadeOut");
        yield return new WaitForSeconds(3.5f); // should be 0.5f, using more for testing

        // wait until VampireCoroutine completes
        // yield return StartCoroutine(VampireCoroutine());

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

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

        yield return new WaitForSeconds(3f); // time it takes to arrive

        // transform back inside the room where player clearly sees
        shapeshiftManager.Shapeshift();
        yield return new WaitForSeconds(1.5f);

        // leave through door closing it behind
        // move using navmesh to vampFinalDestination
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
