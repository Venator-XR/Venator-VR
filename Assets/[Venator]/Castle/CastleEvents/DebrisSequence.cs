using System.Collections;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DebrisSequence : MonoBehaviour
{

    [Header("References")]
    public PlayerMobilityManager playerMobilityManager;
    public PersecutionManager persecutionManager;
    [SerializeField] Transform destination;
    [SerializeField] GameObject debris;
    public XRBaseInteractor handInteractor;
    [SerializeField] private XRKnobLever targetLever;
    public DynamicMoveProvider dynamicMoveProvider;
    public GameObject vampireGameObject;
    public VRFootstepController footstepController;

    [Header("Past rooms")]
    public GameObject[] pastRooms;

    [Header("Animators")]
    [SerializeField] Animator fadeAnim;
    [SerializeField] Animator lightningAnim;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;

    FollowPlayerAgent followPlayerAgent;

    void Start()
    {
    }

    public IEnumerator DebrisCoroutine()
    {
        Debug.Log("Coroutine started!");

        dynamicMoveProvider.moveSpeed = 1.2f;
        // footstepController.isRunning = false;

        // Stop Vampire movement
        persecutionManager.enabled = false;
        followPlayerAgent = vampireGameObject.GetComponent<FollowPlayerAgent>();
        if (followPlayerAgent != null)
        {
            followPlayerAgent.enabled = false;
            Debug.Log("FollowPlayer stopped");
        }
        else Debug.LogWarning("FollowPlayerAgent not found");

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, false);
        // TP player to designated transform
        playerMobilityManager.ForceTeleport(destination);

        // deselect (force hand to let go)
        if (targetLever != null)
        {
            targetLever.ForceEndInteractionAndFade();
        }

        // Play SFXs audio track
        audioSource.Play();

        // activate debris game object
        debris.SetActive(true);

        // wait until sfxs audio track ends
        yield return new WaitForSeconds(6.5f);

        // fade from black
        fadeAnim.Play("fadeOut");
        // enable movement, camera turning and collider
        playerMobilityManager.SetPlayerMobility(true, true);
        yield return new WaitForSeconds(1f);

        // play lightning animation + sfx
        // lightningAnim.Play("");

        // deactivate past rooms
        foreach (GameObject room in pastRooms) room.SetActive(false);

        yield break;
    }
}
