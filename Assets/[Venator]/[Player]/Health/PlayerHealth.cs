using UnityEngine;
using System;

/// <summary>
/// Manages player health with state-based transitions and events.
/// Implements IHealth interface for external damage interactions.
/// </summary>
public class PlayerHealth : MonoBehaviour, IHealth
{
    [Header("Health Configuration")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int healthyMin = 3;
    [SerializeField] private int hurtMin = 2;
    [SerializeField] private int criticalMin = 1;
    [SerializeField] private int damagePerHit = 1;

    [Header("Debug")]
    [SerializeField] private int currentHealth;
    [SerializeField] private HealthState currentState;

    /// <summary>
    /// Invoked when damage is applied. Parameter is the damage amount.
    /// </summary>
    public event Action<int> OnDamaged;

    /// <summary>
    /// Invoked when the health state changes. Parameter is the new state.
    /// </summary>
    public event Action<HealthState> OnStateChanged;

    /// <summary>
    /// Current health value.
    /// </summary>
    public int Current => currentHealth;

    /// <summary>
    /// Current health state.
    /// </summary>
    public HealthState State => currentState;

    /// <summary>
    /// Returns true if the player is dead.
    /// </summary>
    public bool IsDead => currentState == HealthState.Dead;

    private void Awake()
    {
        maxHealth = Mathf.Max(0, maxHealth);
        healthyMin = Mathf.Clamp(healthyMin, 1, maxHealth);
        hurtMin = Mathf.Clamp(hurtMin, 0, healthyMin);
        criticalMin = Mathf.Clamp(criticalMin, 0, hurtMin);

        currentHealth = maxHealth;
        UpdateState(CalculateState(currentHealth));
    }

    /// <summary>
    /// Applies damage to the player. Ignored if already dead. Damage amount is determined internally.
    /// </summary>
    public void ApplyDamage()
    {
        if (IsDead)
        {
            Debug.LogWarning("PlayerHealth: Attempted to damage a dead player.");
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damagePerHit);
        OnDamaged?.Invoke(damagePerHit);

        var newState = CalculateState(currentHealth);
        UpdateState(newState);
    }

    /// <summary>
    /// Instantly kills the player, setting health to 0.
    /// </summary>
    public void Kill()
    {
        if (IsDead)
        {
            Debug.LogWarning("PlayerHealth: Player is already dead.");
            return;
        }

        currentHealth = 0;
        OnDamaged?.Invoke(999); // Sentinel value for instant kill
        UpdateState(HealthState.Dead);
    }

    /// <summary>
    /// Calculates the health state based on current health and thresholds.
    /// </summary>
    /// <param name="health">Current health value.</param>
    /// <returns>The corresponding HealthState.</returns>
    private HealthState CalculateState(int health)
    {
        if (health <= 0) return HealthState.Dead;
        if (health >= healthyMin) return HealthState.Healthy;
        if (health >= hurtMin) return HealthState.Hurt;
        if (health >= criticalMin) return HealthState.Critical;
        
        // Fallback for edge cases with unusual threshold configurations
        return HealthState.Critical;
    }

    /// <summary>
    /// Updates the current state and invokes OnStateChanged if the state has changed.
    /// </summary>
    /// <param name="newState">The new health state.</param>
    private void UpdateState(HealthState newState)
    {
        if (newState == currentState) return;

        currentState = newState;
        OnStateChanged?.Invoke(currentState);

        Debug.Log($"PlayerHealth: State changed to {currentState} (Health: {currentHealth})");
    }
}
