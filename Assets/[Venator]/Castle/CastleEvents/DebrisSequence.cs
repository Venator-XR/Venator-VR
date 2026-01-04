using System.Collections;
using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class DebrisSequence : MonoBehaviour
{
    PlayerMobilityManager playerMobilityManager;

    [Header("References")]
    [SerializeField] Transform destination;
    [SerializeField] GameObject debris;
    public XRBaseInteractor handInteractor;
    [SerializeField] private XRKnobLever targetLever;
    public DynamicMoveProvider dynamicMoveProvider;
    public GameObject vampireGameObject;

    [Header("Past rooms")]
    public GameObject[] pastRooms;

    [Header("Animators")]
    [SerializeField] Animator fadeAnim;
    [SerializeField] Animator lightningAnim;

    [Header("Audio")]
    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource audioSource;

    FollowPlayerAgent followPlayerAgent;

    void Start()
    {
        playerMobilityManager = GetComponent<PlayerMobilityManager>();
    }

    public IEnumerator DebrisCoroutine()
    {
        Debug.Log("Coroutine started!");

        dynamicMoveProvider.moveSpeed = 1.2f;

        // Stop Vampire movement
        followPlayerAgent = vampireGameObject.GetComponent<FollowPlayerAgent>();
        if (followPlayerAgent != null)
        {
            followPlayerAgent.enabled = false;
            Debug.Log("FollowPlayer stopped");
        }
        else Debug.LogWarning("FollowPlayerAgent not found");

        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, false);

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

        // deselect (force hand to let go)
        ForceRelease();

        // TP player to designated transform
        playerMobilityManager.TeleportTo(destination);

        // Play SFXs audio track
        // audioSource.PlayOneShot(audioClip);

        // activate debris game object
        debris.SetActive(true);

        // wait until sfxs audio track ends
        // yield return new WaitWhile(() => audioSource.isPlaying);

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

    void ForceRelease()
    {
        if (targetLever != null && targetLever.isSelected)
        {
            // get manager and interactor
            var manager = targetLever.interactionManager;
            var interactor = targetLever.interactorsSelecting[0];

            manager.SelectExit(interactor, targetLever);
        }
    }
}
