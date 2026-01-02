using UnityEngine;
using System.Collections;

public class DamageTunnelingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private MeshRenderer vignetteRenderer;

    [Header("Tunneling Configuration")]
    [Range(0f, 1f)]
    [SerializeField] private float hurtApertureSize = 0.9f;
    
    [Range(0f, 1f)]
    [SerializeField] private float criticalApertureSize = 0.7f;

    [Range(0f, 1f)]
    [SerializeField] private float feathering = 0.5f;

    [Header("Alignment (Transform Y)")]
    [Tooltip("Mueve esto para centrar el circulo. Unity usa valores entre -0.2 y 0.2")]
    [Range(-0.5f, 0.5f)]
    [SerializeField] private float verticalOffset = 0.0f;

    [Header("Blood Visuals")]
    [SerializeField] private Color bloodColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float pulseSpeed = 3f;

    [Header("Damage intensity (Alpha 0-1)")]
    [Range(0f, 1f)] public float hurtMinAlpha = 0.05f;
    [Range(0f, 1f)] public float hurtMaxAlpha = 0.35f;
    [Range(0f, 1f)] public float criticalMinAlpha = 0.4f;
    [Range(0f, 1f)] public float criticalMaxAlpha = 0.7f;

    private int _apertureID;
    private int _featheringID;
    private int _colorID;

    private MaterialPropertyBlock _propBlock;
    private Coroutine bloodPulseCoroutine;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = GetComponentInParent<PlayerHealth>();
        if (vignetteRenderer == null) vignetteRenderer = GetComponent<MeshRenderer>();

        _propBlock = new MaterialPropertyBlock();

        _apertureID = Shader.PropertyToID("_ApertureSize");
        _featheringID = Shader.PropertyToID("_FeatheringEffect");
        _colorID = Shader.PropertyToID("_VignetteColor");

        UpdateFeathering();
        UpdateVerticalPosition();

        SetVignetteVisuals(0f, hurtApertureSize);
        if (vignetteRenderer != null) vignetteRenderer.enabled = false;
    }

    private void OnValidate()
    {
        if (vignetteRenderer != null) UpdateFeathering(); UpdateVerticalPosition();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnStateChanged += UpdateUI;
            UpdateUI(playerHealth.State);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnStateChanged -= UpdateUI;
    }

    private void UpdateFeathering()
    {
        if (vignetteRenderer == null) return;
        if (_propBlock == null) _propBlock = new MaterialPropertyBlock();

        vignetteRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(_featheringID, feathering);
        vignetteRenderer.SetPropertyBlock(_propBlock);
    }

    private void UpdateVerticalPosition()
    {
        Vector3 localPos = transform.localPosition;
        if (!Mathf.Approximately(localPos.y, verticalOffset))
        {
            localPos.y = verticalOffset;
            transform.localPosition = localPos;
        }
    }

    private void UpdateUI(HealthState state)
    {
        if (bloodPulseCoroutine != null)
        {
            StopCoroutine(bloodPulseCoroutine);
            bloodPulseCoroutine = null;
        }

        switch (state)
        {
            case HealthState.Healthy:
                if (vignetteRenderer != null) vignetteRenderer.enabled = false;
                break;

            case HealthState.Hurt:
            case HealthState.Critical:
            case HealthState.Dead:
                if (vignetteRenderer != null) vignetteRenderer.enabled = true;
                UpdateFeathering();
                UpdateVerticalPosition();
                
                bloodPulseCoroutine = StartCoroutine(BloodPulseEffect(state)); 
                break;
        }
    }

    private IEnumerator BloodPulseEffect(HealthState currentState)
    {
        while (vignetteRenderer != null && playerHealth.State != HealthState.Healthy)
        {
            float minA, maxA;
            float currentSpeed;
            float currentAperture;

            if (currentState == HealthState.Critical)
            {
                minA = criticalMinAlpha;
                maxA = criticalMaxAlpha;
                currentSpeed = pulseSpeed * 2.5f;
                currentAperture = criticalApertureSize;
            }
            else
            {
                minA = hurtMinAlpha;
                maxA = hurtMaxAlpha;
                currentSpeed = pulseSpeed;
                currentAperture = hurtApertureSize;
            }

            float time = Time.time * currentSpeed;
            float wave = Mathf.Abs(Mathf.Sin(time));
            
            float finalAlpha = Mathf.Lerp(minA, maxA, wave);

            SetVignetteVisuals(finalAlpha, currentAperture);

            currentState = playerHealth.State;

            yield return null;
        }
    }

    private void SetVignetteVisuals(float alpha, float aperture)
    {
        if (vignetteRenderer == null) return;

        vignetteRenderer.GetPropertyBlock(_propBlock);

        Color finalColor = bloodColor;
        finalColor.a = alpha;

        _propBlock.SetColor(_colorID, finalColor);
        _propBlock.SetFloat(_apertureID, aperture);

        vignetteRenderer.SetPropertyBlock(_propBlock);
    }
}