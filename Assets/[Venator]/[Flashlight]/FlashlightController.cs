using UnityEngine;
using UnityEngine.InputSystem; // IMPORTANTE: Necesario para el input

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject lightSource;
    [SerializeField] private GameObject lightCone;

    [Header("Input Settings")]
    public InputActionProperty toggleButtonSource; 

    // public encapsulated state
    

    [Header("Raycast Config")]
    public float maxDistance = 8f;
    public LayerMask obstacleLayer;
    public float lerpSpeed = 10f;
    public bool showRaycast = true;

    public bool IsOn { get; private set; } = true;

    void Start()
    {
        UpdateLightVisuals();
    }

    void Update()
    {
        // Button input detection
        if (toggleButtonSource.action != null && toggleButtonSource.action.WasPressedThisFrame())
        {
            ToggleLight();
        }

        if (IsOn) RaycastUpdate();
    }

    private void RaycastUpdate()
    {
        RaycastHit hit;
        float targetLength = maxDistance;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, maxDistance, obstacleLayer))
        {
            targetLength = hit.distance;
            if (showRaycast) Debug.DrawRay(origin, direction * hit.distance, Color.red);
        }
        else
        {
            if (showRaycast) Debug.DrawRay(origin, direction * maxDistance, Color.green);
        }

        // proportional scale
        float scaleFactor = targetLength / maxDistance;
        float targetScaleXY = 1 * scaleFactor;

        // Smooth out size change
        float currentLength = Mathf.Lerp(lightCone.transform.localScale.z, targetLength, Time.deltaTime * lerpSpeed);
        float currentXY = Mathf.Lerp(lightCone.transform.localScale.x, targetScaleXY, Time.deltaTime * lerpSpeed);

        lightCone.transform.localScale = new Vector3(currentXY, currentXY, currentLength);
    }

    private void UpdateLightVisuals()
    {
        if (lightSource != null) lightSource.SetActive(IsOn);
        if (lightCone != null) lightCone.SetActive(IsOn);
    }

    public void ToggleLight()
    {
        IsOn = !IsOn;
        UpdateLightVisuals();
    }
    
    public void TurnOn() 
    {
        if (IsOn) return;
        if (!IsOn) ToggleLight();
    }
}