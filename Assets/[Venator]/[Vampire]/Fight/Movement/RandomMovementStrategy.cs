using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Selects waypoints randomly, avoiding immediate repetition.
/// </summary>
public class RandomMovementStrategy : IMovementStrategy
{
    public Transform GetNextTarget(IReadOnlyList<Transform> waypoints, Transform currentWaypoint)
    {
        if (waypoints == null)
            throw new ArgumentNullException(nameof(waypoints), "Waypoints collection cannot be null!");
        
        if (waypoints.Count == 0)
            throw new ArgumentException("Waypoints collection cannot be empty!", nameof(waypoints));

        if (waypoints.Count == 1)
            return waypoints[0];

        Transform nextWaypoint;
        do
        {
            nextWaypoint = waypoints[Random.Range(0, waypoints.Count)];
        } 
        while (nextWaypoint == currentWaypoint);

        return nextWaypoint;
    }
}