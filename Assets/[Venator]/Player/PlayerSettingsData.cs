using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "Game/Player Settings")]
public class PlayerSettingsData : ScriptableObject
{
    [Header("Movimiento VR")]
    [Tooltip("Velocidad de movimiento suave")]
    public bool useTeleport = true;
    
    [Tooltip("Ángulo de giro por snap (ej. 30, 45, 90)")]
    public bool useSnapTurn = true;
    public float snapTurnAngle = 45.0f;

    [Header("Comfort")]
    [Range(0, 1)]
    public bool useVignette = true;
}