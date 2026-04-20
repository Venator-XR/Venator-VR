using UnityEngine;
using UnityEngine.Events;

public class ShakeDetector : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Sensitivity for arm movement (Position). Lower = easier to trigger.")]
    [SerializeField] private float linearShakeThreshold = 0.5f;

    [Tooltip("Sensitivity for wrist flicks (Rotation). Lower = easier to trigger.")]
    [SerializeField] private float angularShakeThreshold = 45.0f; // Degrees per second

    [Tooltip("How many back-and-forth movements required to trigger.")]
    [SerializeField] private int requiredReversals = 5;

    [Tooltip("Max time between reversals before the counter resets.")]
    [SerializeField] private float comboTimeWindow = 0.5f;

    [Tooltip("Time to wait after a successful shake event.")]
    [SerializeField] private float shakeCooldown = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    [Header("Event")]
    public UnityEvent OnShakeDetected;

    // State Tracking
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    
    private Vector3 lastLinearVelocity;
    private Vector3 lastAngularVelocity;

    private int reversalCount = 0;
    private float timeSinceLastReversal = 0f;
    private float lastTriggerTime;

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        // 1. Calculate Linear Velocity (Position Change)
        Vector3 currentLinearVelocity = (transform.position - lastPosition) / Time.deltaTime;
        
        // 2. Calculate Angular Velocity (Rotation Change)
        // Get the difference in rotation
        Quaternion deltaRot = transform.rotation * Quaternion.Inverse(lastRotation);
        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        
        // Handle angle wrapping (sometimes it returns 360 instead of 0)
        if (angle > 180f) angle -= 360f;
        
        // Calculate angular velocity vector (Axis * Speed)
        Vector3 currentAngularVelocity = axis.normalized * (angle / Time.deltaTime);

        // Save current state for next frame
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        // 3. Check for Shaking
        DetectShake(currentLinearVelocity, currentAngularVelocity);
    }

    private void DetectShake(Vector3 currentLinVel, Vector3 currentAngVel)
    {
        bool isReversal = false;

        // --- Check Linear Reversal (Arm Shake) ---
        if (currentLinVel.magnitude > linearShakeThreshold)
        {
            // Dot Product < 0 means the direction is opposite to the last frame
            if (Vector3.Dot(currentLinVel.normalized, lastLinearVelocity.normalized) < -0.5f)
            {
                isReversal = true;
            }
        }

        // --- Check Angular Reversal (Wrist Flick) ---
        // Only check if we didn't already find a linear reversal this frame
        if (!isReversal && currentAngVel.magnitude > angularShakeThreshold)
        {
            if (Vector3.Dot(currentAngVel.normalized, lastAngularVelocity.normalized) < -0.5f)
            {
                isReversal = true;
            }
        }

        // --- Process Logic ---
        if (isReversal)
        {
            reversalCount++;
            timeSinceLastReversal = 0f;
            
            if (showDebug) Debug.Log($"VR Shake Combo: {reversalCount}");
        }
        else
        {
            timeSinceLastReversal += Time.deltaTime;
        }

        // --- Event Trigger ---
        if (reversalCount >= requiredReversals)
        {
            if (Time.time > lastTriggerTime + shakeCooldown)
            {
                TriggerShake();
            }
            reversalCount = 0; // Reset combo
        }

        // --- Reset if too slow ---
        if (timeSinceLastReversal > comboTimeWindow)
        {
            reversalCount = 0;
        }

        // Store for next frame comparison
        lastLinearVelocity = currentLinVel;
        lastAngularVelocity = currentAngVel;
    }

    private void TriggerShake()
    {
        if (showDebug) Debug.Log("<color=cyan>VR SHAKE DETECTED!</color>");
        lastTriggerTime = Time.time;
        OnShakeDetected.Invoke();
    }
}