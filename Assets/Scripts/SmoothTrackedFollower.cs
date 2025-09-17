using UnityEngine;

/// <summary>
/// Smoothly follows a target Transform (e.g., a VR tracker) with independent smoothing for position and rotation.
/// Uses exponential smoothing for consistent results across frame rates.
/// Attach this to the GameObject you want to smooth.
/// </summary>
public class SmoothTrackedFollower : MonoBehaviour
{
    [Tooltip("Raw tracker to follow (e.g., Vive/Quest tracker).")]
    public Transform rawTracker;

    [Header("Smoothing Settings")]
    [Range(0f, 20f)]
    [Tooltip("Higher = snappier, lower = smoother but more delayed.")]
    public float positionSmoothSpeed = 10f;

    [Range(0f, 20f)]
    [Tooltip("Higher = snappier, lower = smoother but more delayed.")]
    public float rotationSmoothSpeed = 10f;

    private void Awake()
    {
        if (rawTracker == null)
            Debug.LogWarning($"{nameof(SmoothTrackedFollower)}: No target assigned.");
    }

    private void LateUpdate()
    {
        // Make sure a tracker has been assigned before trying to smooth
        if (rawTracker == null) return;

        // Compute frame-rate–independent smoothing factor
        float positionSmoothing = 1f - Mathf.Exp(-positionSmoothSpeed * Time.deltaTime);
        float rotationSmoothing = 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime);

        // Smooth position
        transform.position = Vector3.Lerp(
            transform.position,
            rawTracker.position,
            positionSmoothing
        );

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rawTracker.rotation,
            rotationSmoothing
        );
    }
}
