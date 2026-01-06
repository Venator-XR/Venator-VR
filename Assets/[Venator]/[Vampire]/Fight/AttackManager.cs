using System.Collections;
using UnityEngine;

/// <summary>
/// Manages vampire attack patterns during combat.
/// </summary>
public class AttackManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int swarmCount = 10;
    [SerializeField] private float timeBetweenShots = 0.2f;
    [SerializeField] private float postAttackDelay = 1f;
    [SerializeField] [Range(0f, 1f)] private float attackProbability = 0.7f;

    private bool _isAttacking;

    /// <summary>
    /// Gets whether the vampire is currently executing an attack.
    /// </summary>
    public bool IsAttacking => _isAttacking;

    /// <summary>
    /// Gets or sets the number of projectiles in a swarm attack.
    /// </summary>
    public int SwarmCount
    {
        get => swarmCount;
        set => swarmCount = Mathf.Max(1, value);
    }

    /// <summary>
    /// Gets or sets 
    /// </summary>
    public float TimeBetweenShots
    {
        get => timeBetweenShots;
        set => timeBetweenShots = value;
    }

    /// <summary>
    /// Gets or sets the number of projectiles in a swarm attack.
    /// </summary>
    public float AttackProbability
    {
        get => attackProbability;
        set => attackProbability = value;
    }

    /// <summary>
    /// Initiates a swarm attack if not already attacking.
    /// </summary>
    public void ExecuteSwarmAttack()
    {
        if (_isAttacking)
        {
            Debug.LogWarning("Attack already in progress!");
            return;
        }

        StartCoroutine(SwarmRoutine());
    }

    /// <summary>
    /// Decides whether to attack based on probability, then executes if chosen.
    /// </summary>
    public bool TryExecuteAttack()
    {
        if (_isAttacking)
            return false;

        if (Random.value <= attackProbability)
        {
            ExecuteSwarmAttack();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Coroutine that executes the swarm attack sequence.
    /// </summary>
    private IEnumerator SwarmRoutine()
    {
        _isAttacking = true;
        Debug.Log($"Launching swarm attack with {swarmCount} projectiles!");

        for (int i = 0; i < swarmCount; i++)
        {
            // TODO: Instantiate bat projectile here
            // Example: Instantiate(batProjectilePrefab, transform.position, Quaternion.identity);
            // IMPORTANT: instantiate prefabs in random position between three options (forward left right)
            Debug.Log($"Bat projectile {i + 1}/{swarmCount} launched!");
            
            yield return new WaitForSeconds(timeBetweenShots);
        }

        // Post-attack cooldown
        yield return new WaitForSeconds(postAttackDelay);
        
        _isAttacking = false;
        Debug.Log("Swarm attack completed.");
    }

    /// <summary>
    /// Stops any ongoing attack.
    /// </summary>
    public void StopAttack()
    {
        if (_isAttacking)
        {
            StopAllCoroutines();
            _isAttacking = false;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}