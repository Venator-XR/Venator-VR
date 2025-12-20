using System.Collections;
using UnityEngine;

/// <summary>
/// Main AI brain controlling the vampire's combat behavior through a simple FSM.
/// Phases: Bat Form (Invulnerable Movement) -> Human Form (Vulnerable Attack/Wait)
/// </summary>
public class VampireFightBrain : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private WaypointsManager waypointManager;
    [SerializeField] private VampireFightMovementManager movement;
    [SerializeField] private ShapeshiftManager shapeshifter;
    [SerializeField] private AttackManager attackManager;
    
    [Header("Combat Settings")]
    [SerializeField] private float waitTimeAfterLanding = 0.5f;
    [SerializeField] private float vulnerableWindowDuration = 2.0f;
    [SerializeField] private float initialDelay = 2f;
    
    private VampireHealth _health;
    private IMovementStrategy _movementStrategy;
    private Transform _currentWaypoint;

    public bool IsInBatForm => shapeshifter != null && shapeshifter.currentForm == ShapeState.Bat;

    private void Awake()
    {
        _health = GetComponent<VampireHealth>();
        _movementStrategy = new RandomMovementStrategy();
        
        if (waypointManager == null || movement == null || shapeshifter == null || attackManager == null)
        {
            Debug.LogError("Missing critical references in VampireFightBrain!");
            enabled = false;
        }

        if (_health == null)
        {
            Debug.LogError("VampireFightBrain requires VampireHealth component!");
            enabled = false;
        }
    }

    private void Start()
    {
        StartCoroutine(CombatLoop());
    }

    private IEnumerator CombatLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (_health != null)
        {
            // === PHASE 1: BAT FORM - INVULNERABLE MOVEMENT ===
            TransformToBat();
            
            Transform nextPoint = _movementStrategy.GetNextTarget(waypointManager.Waypoints, _currentWaypoint);
            if (nextPoint == null) yield break;
            
            yield return StartCoroutine(movement.MoveToCoroutine(nextPoint));
            _currentWaypoint = nextPoint;

            // === PHASE 2: LANDING TRANSITION ===
            TransformToHuman();
            yield return new WaitForSeconds(waitTimeAfterLanding);

            // === PHASE 3: VULNERABLE WINDOW ===
            MakeVulnerable();
            
            // Decide whether to attack or just wait
            bool didAttack = attackManager.TryExecuteAttack();
            if (didAttack)
            {
                // Wait until attack finishes
                yield return new WaitUntil(() => !attackManager.IsAttacking);
            }
            else
            {
                // Pure vulnerability window if no attack
                yield return new WaitForSeconds(vulnerableWindowDuration);
            }

            yield return null;
        }

        Debug.Log("Vampire combat loop ended.");
    }

    private void TransformToBat()
    {
        if (shapeshifter.currentForm != ShapeState.Bat)
            shapeshifter.Shapeshift();
        
        if (_health != null)
            _health.SetVulnerability(false);
    }

    private void TransformToHuman()
    {
        if (shapeshifter.currentForm != ShapeState.Vampire)
            shapeshifter.Shapeshift();
    }

    private void MakeVulnerable()
    {
        if (_health != null)
            _health.SetVulnerability(true);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}