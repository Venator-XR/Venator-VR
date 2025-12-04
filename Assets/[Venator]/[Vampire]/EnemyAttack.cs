using UnityEngine;

/// <summary>
/// Example enemy attack component that damages entities implementing IHealth.
/// Uses interface-based interaction for loose coupling.
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;

    private float lastAttackTime;

    private void OnTriggerEnter(Collider other)
    {
        // Cooldown check to prevent multiple hits in quick succession
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        // Get IHealth interface from the colliding object
        var target = other.GetComponent<IHealth>();
        
        if (target != null && !target.IsDead)
        {
            target.ApplyDamage();
            lastAttackTime = Time.time;
            
            Debug.Log($"EnemyAttack: Attacked {other.gameObject.name}");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Optional: Continuous damage while in contact
        // Uncomment if you want repeated damage over time
        /*
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        var target = other.GetComponent<IHealth>();
        
        if (target != null && !target.IsDead)
        {
            target.ApplyDamage();
            lastAttackTime = Time.time;
        }
        */
    }
}
