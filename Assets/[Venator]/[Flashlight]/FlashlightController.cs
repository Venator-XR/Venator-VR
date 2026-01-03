using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject lightSource;
    [SerializeField] private GameObject lightCone;
    [SerializeField] private Animator handAnim;

    [Header("Input Settings")]
    public InputActionProperty toggleButtonSource;

    [Header("Raycast Config")]
    public float maxDistance = 8f;
    public LayerMask obstacleLayer;
    public float lerpSpeed = 10f;
    public bool showRaycast = true;

    public bool IsOn { get; private set; } = true;


    private void OnEnable()
    {
        // Si tienes la acción definida "a mano", hay que encenderla explícitamente
        if (toggleButtonSource.action != null)
            toggleButtonSource.action.Enable();
    }

    private void OnDisable()
    {
        // Y apagarla cuando el objeto se desactive para no dar errores
        if (toggleButtonSource.action != null)
            toggleButtonSource.action.Disable();
    }
    // -----------------------------

    void Start()
    {
        UpdateLightVisuals();
    }

    void Update()
    {
        // Button input detection
        if (toggleButtonSource.action != null && toggleButtonSource.action.WasPressedThisFrame())
        {
            Debug.Log("Flashlight button pressed!");
            handAnim.Play("flashlightButton", -1, 0f);
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

    public void TurnOff()
    {
        if (!IsOn) return;
        ToggleLight();
    }
    
    public void TurnOn()
    {
        if (IsOn) return;
        ToggleLight();
    }
}