using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ButtonLock : MonoBehaviour
{
    [Header("Configuration")]
    public Transform visualButtonObject;
    public float lockedHeightY = -0.05f;
    public float originalHeightY = 0.0f;

    [Header("SFX")]
    private AudioSource audioSource;

    private XRBaseInteractable interactable;
    private bool isLocked = false;
    private XRPokeFollowAffordance xRPokeFollowAffordance;
    private XRPokeFilter xRPokeFilter;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        interactable = GetComponent<XRBaseInteractable>();

        xRPokeFollowAffordance = GetComponent<XRPokeFollowAffordance>();
        xRPokeFilter = GetComponent<XRPokeFilter>();

        // Listen to OnPressed Event
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnPressed);
        }

        if (visualButtonObject != null)
            originalHeightY = visualButtonObject.localPosition.y;

        Debug.Log("found: " + xRPokeFilter + xRPokeFollowAffordance);
    }


    void OnPressed(SelectEnterEventArgs args)
    {
        isLocked = true;
        Debug.LogWarning("isLocked: " + isLocked);
        audioSource.Play();

        // disable every script interacting with button
        xRPokeFollowAffordance.enabled = false;
        xRPokeFilter.enabled = false;
        interactable.enabled = false;

        // lock button down
        Vector3 targetPosition = visualButtonObject.localPosition;
        targetPosition.y = lockedHeightY;
        visualButtonObject.localPosition = targetPosition;
    }

    // Method call when closing settings
    public void UnlockButton()
    {
        isLocked = false;
        xRPokeFollowAffordance.enabled = true;
        xRPokeFilter.enabled = true;
        interactable.enabled = true;

        Debug.LogWarning("isLocked: " + isLocked);
        audioSource.Stop();
    }
}