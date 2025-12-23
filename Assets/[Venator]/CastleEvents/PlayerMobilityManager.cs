using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class PlayerMobilityManager : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] GameObject player;
    [SerializeField] private DynamicMoveProvider moveProvider;
    [SerializeField] private TeleportationProvider teleportProvider;
    [SerializeField] private SnapTurnProvider snapTurnProvider;
    [SerializeField] private ContinuousTurnProvider contTurnProvider;

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
        player.transform.position = destination.position;
        player.transform.rotation = destination.rotation;
    }
}