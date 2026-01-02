using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SettingsCanvas : MonoBehaviour
{
    [Header("Data & Logic")]
    public PlayerSettingsData settings;
    public SettingsManager settingsManager;

    [Header("Player")]
    [SerializeField] NearFarInteractor nearFarInteractor;

    [Header("UI References - Movement")]
    [SerializeField] private Button teleportButton;
    [SerializeField] private Button ContMovButton;

    [Header("UI References - Camera")]
    [SerializeField] private Button snapCamButton;
    [SerializeField] private Button smoothCamButton;
    [SerializeField] Slider turnAmountSlider;

    [Header ("UI References - Comfort")]
    [SerializeField] Toggle vignetteToggle;
    
    [Header("UI Colors")]
    [SerializeField] private Color activeColor = Color.red;
    [SerializeField] private Color inactiveColor = Color.gray;


    private bool isInitializing = false;

    private void Start()
    {
        InitializeUI();
    }

    void OnEnable()
    {
        if(nearFarInteractor == null) Debug.LogError("nearFarInteractor not assigned");
        else nearFarInteractor.enableFarCasting = true;
    }

    void OnDisable()
    {
        if(nearFarInteractor == null) Debug.LogError("nearFarInteractor not assigned");
        else nearFarInteractor.enableFarCasting = false;
    }

    private void InitializeUI()
    {
        isInitializing = true;

        UpdateCameraVisuals(settings.useSnapTurn);
        UpdateMovementVisuals(settings.useTeleport);
        turnAmountSlider.value = settings.snapTurnAngle;
        vignetteToggle.isOn = settings.useVignette;

        isInitializing = false;
    }

    public void SetMovementMode(bool isTeleport)
    {
        if (isInitializing) return;

        settings.useTeleport = isTeleport;
        UpdateMovementVisuals(isTeleport);
        Debug.Log(1);

        settingsManager.ApplySettings();
    }

    private void UpdateMovementVisuals(bool isTeleport)
    {
        if (isTeleport)
        {
            // TELEPORT
            SetButtonState(teleportButton, true);
            SetButtonState(ContMovButton, false);
        }
        else
        {
            // Smooth
            SetButtonState(teleportButton, false);
            SetButtonState(ContMovButton, true);
        }
    }

    public void SetCameraMode(bool isSnap)
    {
        if (isInitializing) return;

        settings.useSnapTurn = isSnap;
        UpdateCameraVisuals(isSnap);
        Debug.Log(1);

        settingsManager.ApplySettings();
    }

    private void UpdateCameraVisuals(bool isSnap)
    {
        if (isSnap)
        {
            // Snap turn
            SetButtonState(snapCamButton, true);
            SetButtonState(smoothCamButton, false);
            turnAmountSlider.interactable = true;
        }
        else
        {
            // Smooth turn
            SetButtonState(snapCamButton, false);
            SetButtonState(smoothCamButton, true);
            turnAmountSlider.interactable = false;
        }
    }

    private void SetButtonState(Button btn, bool isActive)
    {
        var image = btn.GetComponent<Image>();
        
        if (isActive)
        {
            image.color = activeColor;
            btn.GetComponentInChildren<Text>().color = Color.white;
            btn.interactable = false;
        }
        else
        {
            image.color = inactiveColor;
            btn.GetComponentInChildren<Text>().color = Color.black;
            btn.interactable = true;
        }
    }

    public void OnVignetteToggle(bool isOn)
    {
        settings.useVignette = isOn;
        settingsManager.ApplySettings();
    }

    public void OnTurnSliderChanged(float value)
    {
        int rounded = Mathf.RoundToInt(value);
        switch (rounded)
        {
            case 0:
                settings.snapTurnAngle = 30;
                break;
            case 1:
                settings.snapTurnAngle = 45;
                break;
            case 2:
                settings.snapTurnAngle = 60;
                break;
            case 3:
                settings.snapTurnAngle = 90;
                break;
            default:
                settings.snapTurnAngle = 45;
                break;
        }

        settingsManager.ApplySettings();
    }
}