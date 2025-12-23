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

    public IEnumerator DebrisCoroutine()
    {
        Debug.Log("Coroutine started!");
        // disable movement and camera turning
        playerMobilityManager.SetPlayerMobility(false, false);

        // fade to black
        fadeAnim.Play("fadeIn");
        yield return new WaitForSeconds(0.5f);

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
        yield return new WaitForSeconds(1.5f);


        // play lightning animation + sfx
        lightningAnim.Play("");

        yield break;
    }
}
