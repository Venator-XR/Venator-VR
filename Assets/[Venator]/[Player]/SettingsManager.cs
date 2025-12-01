using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private PlayerSettingsData settings;

    [Header("References")]
    [SerializeField] private ControllerInputActionManager rightController;
    [SerializeField] private ControllerInputActionManager leftController;
    [SerializeField] private SnapTurnProvider snapTurnProvider;
    [SerializeField] private GameObject vignette;

    void Start()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        // MOVEMENT
        if (settings.useTeleport)
        {
            rightController.smoothMotionEnabled = false;
        } else if (!settings.useTeleport)
        {   
            rightController.smoothMotionEnabled = true;
        }

        // CAMERA TURN
        if(settings.useSnapTurn)
        {
            rightController.smoothTurnEnabled = true;
            snapTurnProvider.turnAmount = settings.snapTurnAngle;
        } else if (!settings.useSnapTurn)
        {
            rightController.smoothTurnEnabled = false;
        }

        // VIGNETTE
        if(settings.useVignette)
        {
            vignette.SetActive(true);
        } else if (!settings.useVignette)
        {
            vignette.SetActive(false);
        }
    }
}
