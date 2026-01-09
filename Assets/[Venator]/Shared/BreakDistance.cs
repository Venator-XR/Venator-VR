using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // XRI 3.x

public class BreakDistance : MonoBehaviour
{
    [Tooltip("Max distance before the object is released")]
    public float maxDistance = 0.5f; 

    private XRBaseInteractable currentInteractable;

    void Awake()
    {
        currentInteractable = GetComponent<XRBaseInteractable>();

        if (currentInteractable == null)
        {
            Debug.LogWarning($"Interactable not found");
            this.enabled = false;
        }
    }

    void Update()
    {
        if (currentInteractable == null || !currentInteractable.isSelected) return;

        // get grabbing hand
        if (currentInteractable.interactorsSelecting.Count == 0) return;

        var interactor = currentInteractable.interactorsSelecting[0];
        
        // Calculate distance from hand to object
        float currentDistance = Vector3.Distance(interactor.GetAttachTransform(currentInteractable).position, transform.position);

        if (currentDistance > maxDistance)
        {
            // Force select exit
            currentInteractable.interactionManager.SelectExit(interactor, currentInteractable);
        }
    }
}