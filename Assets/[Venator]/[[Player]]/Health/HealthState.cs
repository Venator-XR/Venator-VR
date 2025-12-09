/// <summary>
/// Represents the health state of the player.
/// </summary>
public enum HealthState
{
    /// <summary>
    /// Player is at full or high health.
    /// </summary>
    Healthy,

    /// <summary>
    /// Player has taken some damage.
    /// </summary>
    Hurt,

    /// <summary>
    /// Player is at low health.
    /// </summary>
    Critical,

    /// <summary>
    /// Player is dead (health = 0).
    /// </summary>
    Dead
}
