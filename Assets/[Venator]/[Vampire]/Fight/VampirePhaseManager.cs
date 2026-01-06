using UnityEngine;

public class VampirePhaseManager : MonoBehaviour
{
    // references
    private AttackManager _attackManager;
    private VampireFightBrain _fightBrain;
    private VampireFightMovementManager _movementManager;
    private VampireHealth _health;

    [Header("Health thresholds")]
    [Range(0, 100)]
    [SerializeField] private int hurt;
    [Range(0, 100)]
    [SerializeField] private int critical;

    [Header("Vampire phase stats")]
    [SerializeField] VampireStats[] phases;

    private int _lastPhaseIndex = 0;

    void Awake()
    {
        _attackManager = GetComponent<AttackManager>();
        _fightBrain = GetComponent<VampireFightBrain>();
        _movementManager = GetComponent<VampireFightMovementManager>();
        _health = GetComponent<VampireHealth>();
    }

    void OnEnable()
    {
        _health.OnHealthChanged += HandleHealthChange;
    }

    void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int health)
    {
        int newPhaseIndex = 0;

        if (health <= critical)
        {
            newPhaseIndex = 2;
        }
        else if (health <= hurt)
        {
            newPhaseIndex = 1;
        }
        else
        {
            newPhaseIndex = 0;
        }

        // check, so we don't call the method everytime
        if (newPhaseIndex != _lastPhaseIndex)
        {
            _lastPhaseIndex = newPhaseIndex;

            ChangeStats(phases[newPhaseIndex]);
            Debug.Log("Vampire Phase changed to:" + newPhaseIndex);

            if (newPhaseIndex > 0)
            {
                _fightBrain.TriggerPhaseInterruption(newPhaseIndex);
            }
        }
    }

    private void ChangeStats(VampireStats stats)
    {
        _movementManager.BaseSpeed = stats.moveSpeed;
        _movementManager.Acceleration = stats.acceleration;
        _attackManager.SwarmCount = stats.swarmCount;
        _attackManager.TimeBetweenShots = stats.timeBetweenShots;
        _attackManager.AttackProbability = stats.attackProbability;
        _fightBrain.VulnerableWindowDuration = stats.vulnerableWindowDuration;
    }
}
