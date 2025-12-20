using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private GameObject lightSource; // La luz (Spot Light)
    [SerializeField] private GameObject lightCone;   // El cono visual

    [Header("Configuración Agitar")]
    [SerializeField] private float shakeThreshold = 2.0f;
    [SerializeField] private float shakeCooldown = 1.0f;
    [SerializeField] private bool showDebugLogs = true;

    // Propiedad pública por si otro script quiere leer si está encendida
    // AQUI ES DÓNDE EL VAMPIRO COGE ESTA PROPIEDAD Y LA CAMBIA PARA APAGAR LA LINTERNA MARITO
    public bool IsOn { get; private set; } = false;

    private bool isHeld = false;
    private Vector3 lastTipPosition;
    private float lastShakeTime;

    void Start()
    {
        // Inicialización
        if (lightSource != null) lastTipPosition = lightSource.transform.position;
        // Forzamos el estado inicial visual
        UpdateLightVisuals();
    }

    void Update()
    {
        if (lightSource == null) return;

        // 1. Cálculo de físicas (siempre activo para tener datos frescos)
        float speed = Vector3.Distance(lightSource.transform.position, lastTipPosition) / Time.deltaTime;
        lastTipPosition = lightSource.transform.position;

        // 2. Lógica de Agitar (Solo si está en mano y APAGADA)
        if (isHeld && !IsOn)
        {
            if (showDebugLogs && speed > 0.5f) //Debug.Log($"Speed: {speed:F2}");

            if (speed > shakeThreshold && Time.time > lastShakeTime + shakeCooldown)
            {
                TurnOn(); // Llamamos al método público
                lastShakeTime = Time.time;
            }
        }
    }

    // =========================================================
    // API PÚBLICA (Usa esto desde otros scripts o eventos Unity)
    // =========================================================

    // Úsalo para forzar un estado específico (ej: Batería agotada -> false)
    public void SetLightState(bool newState)
    {
        IsOn = newState;
        UpdateLightVisuals();
    }

    // Úsalo para eventos simples (ej: Entrar en una zona oscura -> TurnOff)
    public void TurnOn() => SetLightState(true);
    public void TurnOff() => SetLightState(false);

    // Úsalo en el XR Interactable (Activate Event)
    public void ToggleLight() => SetLightState(!IsOn);

    // Eventos de XR Grab
    public void OnGrab(bool grabbed)
    {
        isHeld = grabbed;
        if (lightSource != null) lastTipPosition = lightSource.transform.position;
    }

    // =========================================================
    // INTERNO
    // =========================================================

    private void UpdateLightVisuals()
    {
        if (lightSource != null) lightSource.SetActive(IsOn);
        if (lightCone != null) lightCone.SetActive(IsOn);
    }

    public void SetHeldState(bool state)
    {
        isHeld = state;
        if (lightSource != null) lastTipPosition = lightSource.transform.position;
    }
}