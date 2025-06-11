using UnityEngine;

/// <summary>
/// Computes the relative pose of a tracker with respect to an origin tracker,
/// then applies that pose relative to a different reference origin (usually a smoothed origin).
/// </summary>
public class RelativeSmoothTrackedObject : MonoBehaviour
{
    public Transform rawTracker;        // The Vive tracker to follow (e.g., hand)
    public Transform originFrame;     // The origin frame to compute relative pose from (e.g., raw pelvis)
    public Transform referenceOrigin;   // The smoothed or fixed origin to re-apply the relative pose

    public float positionSmoothSpeed = 10f;
    public float rotationSmoothSpeed = 10f;

    void Update()
    {
        if (rawTracker == null || originFrame == null || referenceOrigin == null)
            return;

        // Step 1: compute the relative position and rotation of the rawTracker in the originTracker's frame
        Vector3 localPosition = originFrame.InverseTransformPoint(rawTracker.position);
        Quaternion localRotation = Quaternion.Inverse(originFrame.rotation) * rawTracker.rotation;

        // Step 2: re-apply that relative pose to a reference frame (e.g., smoothed pelvis)
        Vector3 worldTargetPosition = referenceOrigin.TransformPoint(localPosition);
        Quaternion worldTargetRotation = referenceOrigin.rotation * localRotation;

        // Step 3: apply smoothed motion
        transform.position = Vector3.Lerp(transform.position, worldTargetPosition, Time.deltaTime * positionSmoothSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, worldTargetRotation, Time.deltaTime * rotationSmoothSpeed);
    }
}
