using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerMobilityManager : MonoBehaviour
{
    [Header("Player Components")]
    private DynamicMoveProvider moveProvider;
    private TeleportationProvider teleportProvider;
    private SnapTurnProvider snapTurnProvider;
    private ContinuousTurnProvider contTurnProvider;


    void Awake()
    {
        moveProvider = GetComponentInChildren<DynamicMoveProvider>();
        teleportProvider = GetComponentInChildren<TeleportationProvider>();
        snapTurnProvider = GetComponentInChildren<SnapTurnProvider>();
        contTurnProvider = GetComponentInChildren<ContinuousTurnProvider>();
    }
 
    public void SetPlayerMobility(bool canMove, bool canTurn)
    {
        if (moveProvider != null)
            moveProvider.enabled = canMove;

        if (teleportProvider != null)
            teleportProvider.enabled = canMove;

        if (snapTurnProvider != null)
            snapTurnProvider.enabled = canTurn;

        if (contTurnProvider != null)
            contTurnProvider.enabled = canTurn;
    }

    public void ForceTeleport(Transform destination)
{
    XROrigin rig = GetComponent<XROrigin>(); // O usa tu variable serializada xrOrigin
    
    if (rig == null) 
    {
        Debug.LogError("No encuentro XROrigin para forzar el TP");
        return;
    }

    // 1. Mover el Rig (los pies) al destino
    rig.transform.position = destination.position;

    // 2. Girar el Rig para mirar al frente (Match Orientation)
    // Calculamos la rotación ignorando la inclinación vertical de la cabeza
    Vector3 cameraForward = rig.Camera.transform.forward;
    cameraForward.y = 0;
    cameraForward.Normalize();

    Vector3 targetForward = destination.forward;
    targetForward.y = 0;
    targetForward.Normalize();

    float angleDiff = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);
    rig.transform.Rotate(0, angleDiff, 0);

    // 3. Ajuste fino de cabeza (Importante para que el ojo caiga en el punto exacto)
    Vector3 cameraNewPos = rig.Camera.transform.position;
    cameraNewPos.y = destination.position.y; // Proyectar al suelo
    Vector3 offset = destination.position - cameraNewPos;

    rig.transform.position += offset;
    
    Debug.Log("Teletransporte Forzado Completado");
}
}