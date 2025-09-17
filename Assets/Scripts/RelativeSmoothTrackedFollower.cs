using UnityEngine;

/// <summary>
/// Recomputes the relative pose of a tracker with respect to an origin frame,
/// then reapplies that pose relative to a different reference origin (e.g., a smoothed pelvis).
/// Includes exponential smoothing for both position and rotation.
/// Attach this to the GameObject you want to smooth.
/// </summary>
public class RelativeSmoothTrackedFollower : MonoBehaviour
{
    [Tooltip("Raw tracker to follow (e.g., Vive/Quest hand tracker).")]
    public Transform rawTracker;

    [Tooltip("Origin frame used to compute relative pose (e.g., tabletop tracker).")]
    public Transform originFrame;

    [Tooltip("The reference origin to reapply the pose (e.g., smoothed tracker).")]
    public Transform originReference;

    [Header("Smoothing Settings")]
    [Range(0f, 20f)]
    [Tooltip("Higher = snappier, lower = smoother but more delayed.")]
    public float positionSmoothSpeed = 10f;

    [Range(0f, 20f)]
    [Tooltip("Higher = snappier, lower = smoother but more delayed.")]
    public float rotationSmoothSpeed = 10f;

    private void Awake()
    {
        if (rawTracker == null || originFrame == null || originReference == null)
        {
            Debug.LogWarning($"{nameof(RelativeSmoothTrackedFollower)}: Missing one or more references.");
        }
    }
    private void LateUpdate()
    {
        if (rawTracker == null || originFrame == null || originReference == null)
            return;

        // Step 1: relative pose of rawTracker in originFrame's local space
        Vector3 localPos = originFrame.InverseTransformPoint(rawTracker.position);
        Quaternion localRot = Quaternion.Inverse(originFrame.rotation) * rawTracker.rotation;

        // Step 2: reapply relative pose to reference origin
        Vector3 targetPos = originReference.TransformPoint(localPos);
        Quaternion targetRot = originReference.rotation * localRot;

        // Step 3: frame-rate independent smoothing
        float posT = 1f - Mathf.Exp(-positionSmoothSpeed * Time.deltaTime);
        float rotT = 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(transform.position, targetPos, posT);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotT);
    }
}