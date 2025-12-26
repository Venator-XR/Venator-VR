using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class ButtonLock : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastra aquí el objeto visual que se mueve (el engranaje)")]
    public Transform visualButtonObject;

    [Tooltip("La altura Y local donde se quedará bloqueado (ej: -0.02)")]
    public float lockedHeightY = -0.05f;

    [Tooltip("La altura Y original (cuando está subido)")]
    public float originalHeightY = 0.0f;

    private XRBaseInteractable interactable;
    private bool isLocked = false;
    private XRPokeFollowAffordance xRPokeFollowAffordance;
    private XRPokeFilter xRPokeFilter;

    void Start()
    {
        // Buscamos el componente interactable en este objeto
        interactable = GetComponent<XRBaseInteractable>();

        // Nos suscribimos al evento de pulsar
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnPressed);
        }

        // Guardamos la altura actual como la "original" por si acaso
        if (visualButtonObject != null)
            originalHeightY = visualButtonObject.localPosition.y;

        xRPokeFollowAffordance = GetComponent<XRPokeFollowAffordance>();
        xRPokeFilter = GetComponent<XRPokeFilter>();

        Debug.Log("found: " + xRPokeFilter + xRPokeFollowAffordance);
    }

    void OnPressed(SelectEnterEventArgs args)
    {
        // Al pulsar, activamos el bloqueo
        isLocked = true;
        Debug.LogWarning("isLocked: " + isLocked);

        xRPokeFollowAffordance.enabled = false;
        xRPokeFilter.enabled = false;
        interactable.enabled = false;

        Vector3 targetPosition = visualButtonObject.localPosition;
        targetPosition.y = lockedHeightY;
        visualButtonObject.localPosition = targetPosition;
    }

    // LateUpdate ocurre DESPUÉS de las físicas. 
    // Aquí forzamos la posición para anular cualquier muelle que intente subirlo.
    // void LateUpdate()
    // {
    //     if (isLocked && visualButtonObject != null)
    //     {
    //         Vector3 targetPosition = visualButtonObject.localPosition;
    //         targetPosition.y = lockedHeightY;
    //         visualButtonObject.localPosition = targetPosition;
    //     }
    // }

    // --- FUNCION PARA LLAMAR DESDE TU UI (Botón Cerrar Ajustes) ---
    public void UnlockButton()
    {
        isLocked = false;
        xRPokeFollowAffordance.enabled = true;
        xRPokeFilter.enabled = true;
        interactable.enabled = true;
        // Al soltar el lock, el script de física original del botón
        // (el que no encontramos) volverá a tomar el control y lo subirá.
        // Vector3 targetPosition = visualButtonObject.localPosition;
        // targetPosition.y = originalHeightY;
        // visualButtonObject.localPosition = targetPosition;
        Debug.LogWarning("isLocked: " + isLocked);
    }
}