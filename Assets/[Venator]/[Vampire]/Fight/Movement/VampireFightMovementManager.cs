using System.Collections;
using UnityEngine;

/// <summary>
/// Handles vampire movement with dynamic speed and flight patterns.
/// </summary>
public class VampireFightMovementManager : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 7f;
    [SerializeField] private AnimationCurve speedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.5f);
    [SerializeField] private float waveIntensity = 0.5f;
    [SerializeField] private float rotationSpeed = 10f;

    /// <summary>
    /// Gets or sets the base movement speed.
    /// </summary>
    public float BaseSpeed
    {
        get => baseSpeed;
        set => baseSpeed = Mathf.Max(0, value);
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

        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / travelTime;
            
            // Linear interpolation with speed curve
            float currentSpeed = speedCurve.Evaluate(normalizedTime) * baseSpeed;
            Vector3 linearPos = Vector3.Lerp(startPos, target.position, normalizedTime);

            // Add sinusoidal wave for bat-like flight pattern
            float wave = Mathf.Sin(normalizedTime * Mathf.PI * 3) * waveIntensity;
            transform.position = linearPos + Vector3.up * wave;

            // Smooth rotation towards movement direction
            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }

        transform.position = target.position;
    }
}