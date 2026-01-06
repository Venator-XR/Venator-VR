using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles vampire movement with dynamic speed and flight patterns.
/// </summary>
public class VampireFightMovementManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] bool navMeshMovmement = true;
    [SerializeField] private float baseSpeed = 7f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float slowdownDistance = 1f;
    [SerializeField] private float arrivalPrecision = 0.1f;

    [Header("Player Reference")]
    [SerializeField] private Transform playerTransform;

    private NavMeshAgent _agent;
    private bool isMoving;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null) Debug.LogError("NavMeshAgent not found");
        else
        {
            _agent.acceleration = acceleration;
            _agent.speed = baseSpeed;
            _agent.angularSpeed = rotationSpeed;
        }
    }

    /// <summary>
    /// Gets or sets the base movement speed.
    /// </summary>
    public float BaseSpeed
    {
        get => baseSpeed;
        set => baseSpeed = Mathf.Max(0, value);
    }

    /// <summary>
    /// Gets or sets the movement acceleration.
    /// </summary>
    public float Acceleration
    {
        get => acceleration;
        set => acceleration = Mathf.Max(0, value);
    }

    void Update()
    {
        // agent braking
        if (navMeshMovmement)
        {
            if (!isMoving) return;

            if (!_agent.pathPending)
            {
                float distance = _agent.remainingDistance;

                if (distance <= slowdownDistance)
                {
                    float newSpeed = Mathf.Lerp(_agent.speed, 0.5f, Time.deltaTime * 5f);

                    _agent.speed = newSpeed;

                    if (distance <= arrivalPrecision)
                    {
                        _agent.isStopped = true;
                        isMoving = false;
                    }
                }
            }

        }
        else return;
    }

    /// <summary>
    /// Moves the vampire to the target position with a curved flight path.
    /// </summary>
    public IEnumerator MoveToCoroutine(Transform target)
    {
        if (target == null)
        {
            Debug.LogError("Target is null!");
            yield break;
        }

        Vector3 startPos = transform.position;
        float distance = Vector3.Distance(startPos, target.position);
        float travelTime = distance / baseSpeed;
        float elapsed = 0;

        if (!navMeshMovmement)
        {
            while (elapsed < travelTime)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / travelTime;

                // movement
                transform.position = Vector3.Lerp(startPos, target.position, normalizedTime);

                // Smooth rotation towards movement direction
                Vector3 direction = (target.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                yield return null;
            }
        }
        else
        {
            isMoving = true;
            MoveTo(target.position);
            while (isMoving) yield return null;
        }

        transform.position = target.position;

        if (playerTransform != null)
        {
            // Calculate direction to player
            Vector3 lookPos = playerTransform.position - transform.position;
            lookPos.y = 0;

            // inmediate rotation
            if (lookPos != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookPos);

        }
        else Debug.LogError("playerTransform not assigned, vampire not rotating torwards player");
    }

    public void MoveTo(Vector3 targetPosition)
    {
        _agent.enabled = true;
        _agent.isStopped = false;

        // reset speed
        _agent.speed = baseSpeed;
        _agent.SetDestination(targetPosition);
    }
}