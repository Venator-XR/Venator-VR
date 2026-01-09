using UnityEngine;

public class CheckpointLoader : MonoBehaviour
{
    public PlayerMobilityManager mobilityManager; 
    public Transform checkpointPosition;

    void Start()
    {
        if (CheckpointState.SpawnAtCheckpoint)
        {
            Debug.Log("teleporting to checkpoint");
            mobilityManager.TeleportTo(checkpointPosition);
        }
        else
        {
            Debug.Log("Starting from the beginning (normal spawn).");
        }
    }
}