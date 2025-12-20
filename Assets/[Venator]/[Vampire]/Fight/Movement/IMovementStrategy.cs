using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Strategy pattern for vampire movement decision-making.
/// </summary>
public interface IMovementStrategy
{
    /// <summary>
    /// Determines the next target waypoint based on the strategy.
    /// </summary>
    /// <param name="waypoints">Available waypoints to choose from.</param>
    /// <param name="currentWaypoint">The current waypoint (can be null).</param>
    /// <returns>The next target waypoint.</returns>
    Transform GetNextTarget(IReadOnlyList<Transform> waypoints, Transform currentWaypoint);
}