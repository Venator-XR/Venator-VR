using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class VampireChaseManager : MonoBehaviour
{
    [Header("Vampire References")]
    public GameObject vampire;
    public NavMeshAgent vampireAgent;
    public FollowPlayerAgent followScript;

    [Header("Approach config")]
    public Transform approachDestination;
    public float approachSpeed = 1.5f;

    [Header("Player")]
    public DynamicMoveProvider dynamicMoveProvider;
    public FlashlightController flashlightController;
    public VRFootstepController footstepController;

    [Header("SFX")]
    public AudioSource audioSource;
    public GameObject globalSoundManagerGO;
    public AudioClip stingerSFX;
    public AudioClip heavyBreathing;
    GlobalSoundManager globalSoundManager;
    MixerController mixerController;
    
    private bool chaseStarted = false;
    private bool approachStarted = false;

    void Awake()
    {
        globalSoundManager = globalSoundManagerGO.GetComponent<GlobalSoundManager>();
        mixerController = globalSoundManagerGO.GetComponent<MixerController>();
    }

    void Update()
    {
        if (!chaseStarted && approachStarted && vampire.activeSelf == true)
        {
            if (vampireAgent.remainingDistance <= vampireAgent.stoppingDistance)
            {
                Debug.Log("Vampire reached approach destination, chase started");
                StartChase();
            }
        }
    }

    public void StartApproach()
    {
        StartCoroutine(ApproachCoroutine());
    }

    public void StartChase()
    {
        chaseStarted = true;
        followScript.enabled = true;
    }

    private System.Collections.IEnumerator ApproachCoroutine()
    {
        globalSoundManager.StopSequence();
        flashlightController.Malfunction(true);
        vampire.SetActive(true);


        audioSource.PlayOneShot(stingerSFX);
        yield return new WaitForSeconds(1f);

        globalSoundManager.PlayNextSequence();

        yield return new WaitForSeconds(1f);
        audioSource.clip = heavyBreathing;
        audioSource.loop = true;
        mixerController.SetGroupVolume("PlayerMain", 0.1f);
        audioSource.Play();

        vampireAgent.speed = approachSpeed;
        vampireAgent.SetDestination(approachDestination.position);

        vampireAgent.speed = approachSpeed;
        vampireAgent.SetDestination(approachDestination.position);
        yield return new WaitForSeconds(1f);
        dynamicMoveProvider.moveSpeed = 3f;
        // footstepController.isRunning = true;
        approachStarted = true;
    }
}
