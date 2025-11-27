using UnityEngine;
using UnityEngine.UI;

public class SettingsCanvas : MonoBehaviour
{
    [Header("Data & Logic")]
    public PlayerSettingsData settings;
    public SettingsManager settingsManager;

    [Header("UI References")]
    [SerializeField] Slider turnAmountSlider;
    [SerializeField] Toggle vignetteToggle;
    [SerializeField] Dropdown movementDropdown; // 0: Teleport, 1: Continuous
    [SerializeField] Dropdown cameraDropdown;   // 0: Snap, 1: Smooth

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        vignetteToggle.isOn = settings.useVignette;

        movementDropdown.value = settings.useTeleport ? 0 : 1;

        cameraDropdown.value = settings.useSnapTurn ? 0 : 1;

        turnAmountSlider.value = settings.snapTurnAngle;
    }


    public void OnMovementChanged()
    {
        int index = movementDropdown.value;

        if (index == 0) settings.useTeleport = true;
        else settings.useTeleport = false;

        settingsManager.ApplySettings();
    }

    public void OnCameraChanged()
    {
        int index = cameraDropdown.value;

        if (index == 0) settings.useSnapTurn = true;
        else settings.useSnapTurn = false;

        settingsManager.ApplySettings();
    }

    public void OnVignetteToggle(bool isOn)
    {
        settings.useVignette = isOn;
        settingsManager.ApplySettings();
    }

    // Asegúrate de conectar este método al evento "OnValueChanged" del Slider en el Editor
    public void OnTurnSliderChanged(float value)
    {
        int rounded = Mathf.RoundToInt(value);
        switch (rounded)
        {
            case 1:
                settings.snapTurnAngle = 30;
                break;
            case 2:
                settings.snapTurnAngle = 45;
                break;
            case 3:
                settings.snapTurnAngle = 60;
                break;
            case 4:
                settings.snapTurnAngle = 90;
                break;
            default:
                settings.snapTurnAngle = 45;
                break;
        }

        settingsManager.ApplySettings();
    }
}