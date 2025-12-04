using System;

/// <summary>
/// Interface for entities that can take damage and die.
/// </summary>
public interface IHealth
{
    /// <summary>
    /// Current health value.
    /// </summary>
    int Current { get; }

    /// <summary>
    /// Current health state.
    /// </summary>
    HealthState State { get; }

    /// <summary>
    /// Returns true if the entity is dead.
    /// </summary>
    bool IsDead { get; }

    /// <summary>
    /// Invoked when damage is applied.
    /// </summary>
    event Action<int> OnDamaged;

    /// <summary>
    /// Invoked when the health state changes.
    /// </summary>
    event Action<HealthState> OnStateChanged;

    /// <summary>
    /// Applies damage to the entity. Damage amount is determined internally.
    /// </summary>
    void ApplyDamage();

    /// <summary>
    /// Instantly kills the entity, setting health to 0.
    /// </summary>
    void Kill();
}
