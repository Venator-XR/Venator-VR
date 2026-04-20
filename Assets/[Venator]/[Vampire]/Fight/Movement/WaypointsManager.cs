using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the collection of waypoints for vampire movement.
/// </summary>
public class WaypointsManager : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;

    public IReadOnlyList<Transform> Waypoints => waypoints;

    private void OnValidate()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("WaypointsManager has no waypoints assigned!");
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null) return;

        Gizmos.color = Color.yellow;
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawWireSphere(waypoint.position, 0.5f);
            }
        }
    }
}