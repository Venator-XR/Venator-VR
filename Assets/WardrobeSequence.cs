using System.Collections;
using UnityEngine;

public class WardrobeSequence : MonoBehaviour
{
    PlayerMobilityManager playerMobilityManager;

    [Header("References")]
    [SerializeField] Transform insideDestination;
    [SerializeField] Transform outsideDestination;
    [SerializeField] Animator fadeAnim;
    
    [Header("Vampire References")]
    [SerializeField] ShapeshiftManager shapeshiftManager;

    [Header("Audio")]
    [SerializeField] AudioClip enteringAudioClip;
    [SerializeField] AudioClip exitingAudioClip;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        playerMobilityManager = GetComponent<PlayerMobilityManager>();
    }

    private IEnumerator StartWardrobeCoroutine()
    {
        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, false);

        // fade to black
        fadeAnim.SetTrigger("");

        // tp player inside wardrobe looking through the hole
        playerMobilityManager.TeleportTo(insideDestination);

        // play sfx: wardrobe opening | steps | wardrobe closing
        audioSource.PlayOneShot(enteringAudioClip);

        // fade from black
        fadeAnim.SetTrigger("");

        // start vampire animation:
        // transform into bat
        // fly through hole in debris
        // transform back inside the room where player clearly sees
        // leave through door closing it behind

        // wait 2 seconds
        yield return new WaitForSeconds(2);

        // fade to black
        fadeAnim.SetTrigger("");

        // tp player outside wardrobe looking at door
        playerMobilityManager.TeleportTo(outsideDestination);

        // play sfx: wardrobe opening | steps | wardrobe closing
        audioSource.PlayOneShot(exitingAudioClip);

        // fade from black
        fadeAnim.SetTrigger("");

        // InventoryTutorialManager.StartInventoryTutorial();

        yield break;
    }
}
