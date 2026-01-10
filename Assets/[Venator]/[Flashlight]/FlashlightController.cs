using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject lightSource;
    [SerializeField] private GameObject lightCone;
    [SerializeField] private Animator _animator;
    [SerializeField] private ShakeDetector _shakeDetector;

    [Header("Input Settings")]
    public InputActionProperty toggleButtonSource;

    [Header("Raycast Config")]
    public float maxDistance = 8f;
    public LayerMask obstacleLayer;
    public float lerpSpeed = 10f;
    public bool showRaycast = true;

    public bool IsOn { get; private set; } = true;
    public event Action OnFlashlightToggle;

    private bool canPushBt = true;


    private void OnEnable()
    {
        if (toggleButtonSource.action != null)
            toggleButtonSource.action.Enable();
    }

    private void OnDisable()
    {
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
        if (IsOn) RaycastUpdate();

        if (!canPushBt) return;

        // Button input detection
        if (toggleButtonSource.action != null && toggleButtonSource.action.WasPressedThisFrame())
        {
            Debug.Log("Flashlight button pressed!");
            _animator.Play("flashlightButton", -1, 0f);
            ToggleLight();
        }
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
        OnFlashlightToggle?.Invoke();
    }

    // called for occasions when we need the flashlight a specific way
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
    //------------------------

    public void ShakenOn()
    {
        TurnOn();
        Dim(false);
        canPushBt = true;
        _shakeDetector.enabled = false;
    }

    public void Dim(bool value)
    {
        _animator.SetBool("dimmed", value);
    }

    public void Malfunction(bool reActivate)
    {
        StartCoroutine(DimThenTurnOff(reActivate));
    }

    private IEnumerator DimThenTurnOff(bool reActivate)
    {
        canPushBt = false;
        Dim(true);
        yield return new WaitForSeconds(.5f);

        TurnOff();
        yield return new WaitForSeconds(.5f);

        if (reActivate)
        {
            TurnOn();
            yield return new WaitForSeconds(2f);
            Dim(false);
            canPushBt = true;
        }
        else
        {
            _shakeDetector.enabled = true;
        }
    }
}