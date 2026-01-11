using System.Collections;
using UnityEngine;

/// <summary>
/// Main AI brain controlling the vampire's combat behavior through a simple FSM.
/// Phases: Bat Form (Invulnerable Movement) -> Human Form (Vulnerable Attack/Wait)
/// </summary>
public class VampireFightBrain : MonoBehaviour
{
    [Header("Combat configuration")]
    [SerializeField] private float vulnerableWindowDuration = 2.0f;
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float shapeshiftDelay = 1.5f;

    [Header("First Waypoint")]
    [SerializeField] private Transform _currentWaypoint;
    private IMovementStrategy _movementStrategy;

    [Header("References")]
    [SerializeField] private WaypointsManager _waypointsManager;
    [SerializeField] private Animator _animator;
    public GlobalSoundManager globalSoundManager;
    // all classes below have to be inside vampire gameobject, which has VampireFightBrain (this script)
    private VampireHealth _health;
    private VampireFightMovementManager _movement;
    private ShapeshiftManager _shapeshifter;
    private AttackManager _attackManager;
    

    public bool IsInBatForm => _shapeshifter != null && _shapeshifter.currentForm == ShapeState.Bat;

    public float VulnerableWindowDuration
    {
        get => vulnerableWindowDuration;
        set => vulnerableWindowDuration = value;
    }

    private bool init = false;

    private void Awake()
    {
        if (_movementStrategy == null)
        {
            _movementStrategy = new RandomMovementStrategy();
        }

        _health = GetComponent<VampireHealth>();
        _movement = GetComponent<VampireFightMovementManager>();
        _shapeshifter = GetComponent<ShapeshiftManager>();
        _attackManager = GetComponent<AttackManager>();


        if (_waypointsManager == null || _movement == null || _shapeshifter == null || _attackManager == null)
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

    public void TriggerPhaseInterruption(int phaseIndex)
    {
        if (IsInBatForm) return;

        Debug.Log("!!! PHASE CHANGE - STOPPING COROUTINES !!!");

        StopAllCoroutines();
        _attackManager.StopAttack();


        StartCoroutine(PhaseChangeSequence(phaseIndex));
    }

    private IEnumerator PhaseChangeSequence(int phase)
    {
        // invunerable while animation 
        _health.SetVulnerability(false);

        // start animation & SFX
        if (phase == 1)
        {
            _animator.SetTrigger("laugh");
            // audioSource.PlayOneShot(laughSound);
        }
        else if (phase == 2)
        {
            _animator.SetTrigger("scream");
            // audioSource.PlayOneShot(screamSound);
        }

        // wait for animation
        yield return new WaitForSeconds(2.0f);
        Debug.Log("Done! Restarting combat loop...");


        // Restart Coroutine from 0 so it flies away directly
        StartCoroutine(CombatLoop());
    }

    private IEnumerator CombatLoop()
    {
        if (!init) { yield return new WaitForSeconds(initialDelay); init = true; }
        
        // start / next music
        globalSoundManager.PlayNextSequence();

        Debug.Log("ComatLoop()");

        while (_health != null)
        {
            // === PHASE 1: BAT FORM - INVULNERABLE MOVEMENT ===
            TransformToBat();
            yield return new WaitForSeconds(shapeshiftDelay);

            Transform nextPoint = _movementStrategy.GetNextTarget(_waypointsManager.Waypoints, _currentWaypoint);
            if (nextPoint == null) { Debug.LogError("nextPoint null"); yield break; }

            yield return StartCoroutine(_movement.MoveToCoroutine(nextPoint));
            _currentWaypoint = nextPoint;

            // === PHASE 2: LANDING TRANSITION ===
            TransformToHuman();
            yield return new WaitForSeconds(shapeshiftDelay);

            // === PHASE 3: VULNERABLE WINDOW ===
            MakeVulnerable();

            // Decide whether to attack or just wait
            bool didAttack = _attackManager.TryExecuteAttack();
            if (didAttack)
            {
                // Wait until attack finishes
                yield return new WaitUntil(() => !_attackManager.IsAttacking);
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
        if (_shapeshifter.currentForm != ShapeState.Bat)
            _shapeshifter.Shapeshift();

        if (_health != null)
            _health.SetVulnerability(false);
    }

    private void TransformToHuman()
    {
        if (_shapeshifter.currentForm != ShapeState.Vampire)
            _shapeshifter.Shapeshift();
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