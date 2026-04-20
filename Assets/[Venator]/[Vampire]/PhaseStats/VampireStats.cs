using UnityEngine;

[CreateAssetMenu(fileName = "VampireStats", menuName = "VampireStats")]
public class VampireStats : ScriptableObject
{
    [Header("Movement")]
    [Range(5, 15)]
    public int moveSpeed;
    [Range(10, 30)]
    public int acceleration;
    [Header("Attack")]
    [Range(3, 10)]
    public int swarmCount;
    [Range(0, 2)]
    public float timeBetweenShots;
    [Range(0, 1)]
    public float attackProbability;
    [Range(0.5f, 3f)]
    public float vulnerableWindowDuration;
}
