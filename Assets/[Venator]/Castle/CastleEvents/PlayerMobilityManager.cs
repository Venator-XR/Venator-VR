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

    public void TeleportTo(Transform destination)
    {
        if (teleportProvider == null)
        {
            Debug.LogError("PlayerMobilityManager: Falta asignar el TeleportationProvider en el Inspector.");
            return;
        }

        // Creamos la solicitud de teletransporte
        TeleportRequest request = new TeleportRequest()
        {
            // Posición de destino (donde irán los PIES)
            destinationPosition = destination.position,

            // Rotación de destino (hacia donde mirará la CÁMARA)
            destinationRotation = destination.rotation,

            // IMPORTANTE: Esto le dice al sistema "Gira el Rig para que coincida con la flecha azul del destino"
            matchOrientation = MatchOrientation.TargetUpAndForward
        };

        // Encolamos la solicitud. El sistema la ejecutará en el siguiente frame físico.
        teleportProvider.QueueTeleportRequest(request);
        
        Debug.Log($"Teletransportando a {destination.name} usando TeleportRequest");
    }
}