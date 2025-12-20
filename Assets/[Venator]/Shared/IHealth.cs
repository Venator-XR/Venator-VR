using System;

/// <summary>
/// Common interface for all entities that can take damage and be destroyed.
/// Implemented by player, enemies, and destructible objects.
/// </summary>
public interface IHealth
{
    /// <summary>
    /// Invoked when the entity dies.
    /// </summary>
    event Action OnDeath;

    /// <summary>
    /// Gets whether the entity can currently take damage.
    /// </summary>
    bool IsVulnerable { get; }

    /// <summary>
    /// Applies damage to the entity.
    /// </summary>
    /// <param name="amount">The amount of damage to apply.</param>
    void ApplyDamage(int amount);

    /// <summary>
    /// Instantly kills the entity.
    /// </summary>
    void Kill();
}
