using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "Game/Player Settings")]
public class PlayerSettingsData : ScriptableObject
{
    [Header("Movement")]
    public bool useTeleport = true;

    [Header("Camera")]
    public bool useSnapTurn = true;
    public float snapTurnAngle = 45.0f;

    [Header("Comfort")]
    public bool useVignette = true;
}