using UnityEngine;
using UnityEngine.Events;

public class ShakeDetector : MonoBehaviour
{
    [Header("Shake Configuration")]
    [SerializeField] private float shakeThreshold = 2.0f;
    [SerializeField] private float minShakeDuration = 1f;
    [SerializeField] private float shakeCooldown = 1.0f;
    [SerializeField] private bool showDebugLogs = true;

    [Header("Event")]
    public UnityEvent OnShakeDetected;

    // Local variables
    private Vector3 lastPosition;
    private float lastShakeTime;
    private float currentShakeTime = 0f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate speed
        float speed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // Detection
        if (speed > shakeThreshold)
        {
            currentShakeTime += Time.deltaTime;

            if (currentShakeTime > minShakeDuration && Time.time > lastShakeTime + shakeCooldown)
            {
                TriggerShake();
            }
        }
        else
        {
            // Reset if not shaking
            currentShakeTime = 0f;
        }
    }

    private void TriggerShake()
    {
        if (showDebugLogs) Debug.Log($"Shake detected on {gameObject.name}!");

        OnShakeDetected.Invoke();

        lastShakeTime = Time.time;
        currentShakeTime = 0f;
    }
}