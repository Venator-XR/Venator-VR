using Unity.AI.Navigation.Samples;
using UnityEngine;
using UnityEngine.AI;
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
    public Animator flashlightAnim;

    private bool chaseStarted = false;
    private bool approachStarted = false;

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
        flashlightAnim.SetBool("dimmmed", true);
        yield return new WaitForSeconds(.5f);
        flashlightController.TurnOff();
        vampire.SetActive(true);

        vampireAgent.speed = approachSpeed;
        vampireAgent.SetDestination(approachDestination.position);

        vampireAgent.speed = approachSpeed;
        vampireAgent.SetDestination(approachDestination.position);
        yield return new WaitForSeconds(2f);
        flashlightController.TurnOn();
        flashlightAnim.SetBool("dimmed", false);
        dynamicMoveProvider.moveSpeed = 3f;
        approachStarted = true;
    }
}
