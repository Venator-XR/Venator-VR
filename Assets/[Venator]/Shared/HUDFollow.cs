using UnityEngine;

public class HUDFollow : MonoBehaviour
{
    [Header("Config")]
    public Transform headCamera;
    public float distance = 1.5f;
    [Tooltip("Lower value = slower follow")]
    public float smoothSpeed = 5f;
    public float heightOffset = -0.3f;

    void LateUpdate()
    {
        if (headCamera == null) return;

        Vector3 forwardFlat = headCamera.forward;
        forwardFlat.y = 0;
        forwardFlat.Normalize();

        if (forwardFlat == Vector3.zero) forwardFlat = headCamera.forward;

        Vector3 targetPosition = headCamera.position + (forwardFlat * distance);
        targetPosition.y = headCamera.position.y + heightOffset;

        // Move smoothly to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Canvas always looking at camera
        Quaternion targetRotation = Quaternion.LookRotation(transform.position - headCamera.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed * 2);
    }
}