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
        flashlightController.TurnOff();
        vampire.SetActive(true);
        Light[] pointLights = vampire.GetComponentsInChildren<Light>();
        foreach (Light light in pointLights)
        {
            StartCoroutine(ApproachCoroutine(light));
        }
    }

    public void StartChase()
    {
        chaseStarted = true;
        followScript.enabled = true;
    }

    private System.Collections.IEnumerator ApproachCoroutine(Light light)
    {
        float targetIntensity = light.intensity;
        light.intensity = 0f;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(0f, targetIntensity, elapsed / duration);
            yield return null;
        }

        light.intensity = targetIntensity;

        vampireAgent.speed = approachSpeed;
        vampireAgent.SetDestination(approachDestination.position);
        yield return new WaitForSeconds(2f);
        dynamicMoveProvider.moveSpeed = 3f;
        approachStarted = true;
    }
}
