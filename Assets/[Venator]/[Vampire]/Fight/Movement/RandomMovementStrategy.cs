using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Selects waypoints randomly, avoiding immediate repetition.
/// </summary>
public class RandomMovementStrategy : IMovementStrategy
{
    private bool _lastWasLeftSide = false;

    public Transform GetNextTarget(IReadOnlyList<Transform> waypoints, Transform currentWaypoint)
    {

        if (waypoints == null)
            throw new ArgumentNullException(nameof(waypoints), "Waypoints collection cannot be null!");

        if (waypoints.Count == 0)
            throw new ArgumentException("Waypoints collection cannot be empty!", nameof(waypoints));

        if (waypoints.Count == 1)
            return waypoints[0];

        Transform nextWaypoint;
        // 20% chance of center waypoint
        if (currentWaypoint != waypoints[0] && Random.value < 0.20f)
        {
            return waypoints[0];
        }
        
        // other waypoints
        do
        {
            int index;

            // invert side so vampire goes side to side
            _lastWasLeftSide = !_lastWasLeftSide;
            if (_lastWasLeftSide)
            {
                // Left side (2, 4, 6...)
                index = Random.Range(1, waypoints.Count / 2) * 2;
            }
            else
            {
                // Right side (3, 5, 7...)
                index = (Random.Range(2, waypoints.Count / 2) * 2) + 1;
            }

            index = Mathf.Clamp(index, 0, waypoints.Count - 1);
            nextWaypoint = waypoints[index];
        }
        while (nextWaypoint == currentWaypoint);

        return nextWaypoint;
    }
}