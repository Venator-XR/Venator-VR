using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

// Necesario para el botón personalizado
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        if (settings == null) 
        {
            Debug.LogError("¡Falta asignar el PlayerSettingsData!");
            return;
        }

        // MOVEMENT
        if (settings.useTeleport)
        {
            leftController.smoothMotionEnabled = false;
        } 
        else 
        {   
            leftController.smoothMotionEnabled = true;
        }

        // CAMERA TURN
        if(settings.useSnapTurn)
        {
            rightController.smoothTurnEnabled = false; 
            
            if(snapTurnProvider != null) 
                snapTurnProvider.turnAmount = settings.snapTurnAngle;
        } 
        else 
        {
            rightController.smoothTurnEnabled = true;
        }

        // VIGNETTE
        if (vignette != null)
        {
            if(settings.useVignette)
            {
                vignette.SetActive(true);
            } 
            else 
            {
                vignette.SetActive(false);
            }
        }
        
        Debug.Log("Settings applied");
    }
}