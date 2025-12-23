using System.Collections;
using UnityEngine;

public class DebrisSequence : MonoBehaviour
{
    PlayerMobilityManager playerMobilityManager;

    [Header("References")]
    [SerializeField] Transform destination;
    [SerializeField] GameObject debris;

    [Header("Animators")]
    [SerializeField] Animator fadeAnim;
    [SerializeField] Animator lightningAnim;

    [Header("Audio")]
    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        playerMobilityManager = GetComponent<PlayerMobilityManager>();
    }

    public IEnumerator StartDebrisCoroutine()
    {
        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, false);

        // fade to black
        fadeAnim.SetTrigger("");

        // TP player to designated transform
        playerMobilityManager.TeleportTo(destination);

        // Play SFXs audio track
        audioSource.PlayOneShot(audioClip);

        // activate debris game object
        debris.SetActive(true);

        // wait until sfxs audio track ends
        yield return new WaitWhile(() => audioSource.isPlaying);

        // fade from black
        fadeAnim.SetTrigger("");

        // enable movement, camera turning and collider
        playerMobilityManager.SetPlayerMobility(true, true);

        // play lightning animation + sfx
        lightningAnim.Play("");

        yield break;
    }
}
