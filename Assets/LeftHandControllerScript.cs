using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandControllerScript : MonoBehaviour
{
    [Header("Rocket Hand Settings")]
    public float rocketForce = 50f;
    public float accelerationTime = 2f; // Time to reach full speed

    private Rigidbody rb;
    private float elapsedTime = 0f;

    [Header("Direction Settings")]
    public Transform handTransform;
    public Transform aimDirectionReference;
    public float rotationSmoothSpeed = 5f;
    public float rotationSensitivity = 0.01f;
    
    private Vector3 baseForward;
    private Quaternion previousAimRotation;
    private float accumulatedYaw = 0f;

    void Start()
    {
        if (aimDirectionReference != null)
            previousAimRotation = aimDirectionReference.rotation;
        baseForward = GetLaunchDirection();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on Left Hand!");
        }

        elapsedTime = 0f; // Reset time when launched

        Debug.Log("Rocket hand launched!");
    }

void Update()
{
    elapsedTime += Time.deltaTime;

    // Calculate acceleration
    float accelerationFactor = Mathf.Clamp01(elapsedTime / accelerationTime);
    float currentSpeed = rocketForce * accelerationFactor;

    // Get launch direction (based on current yaw)
    Vector3 launchDirection = Quaternion.Euler(0, accumulatedYaw, 0) * Vector3.forward;
    Debug.Log("Yaw: " + accumulatedYaw);
    rb.velocity = launchDirection * currentSpeed;

    // Update yaw based on real hand rotation delta
    if (aimDirectionReference != null)
    {
        Quaternion currentRotation = aimDirectionReference.rotation;

        // Get delta rotation between frames
        Quaternion delta = currentRotation * Quaternion.Inverse(previousAimRotation);
        //previousAimRotation = currentRotation;

        // Convert delta rotation to yaw angle
        delta.ToAngleAxis(out float angle, out Vector3 axis);
        if (Vector3.Dot(axis, Vector3.up) < 0) angle = -angle; // Ensure correct direction
        float yawChange = angle;

        // Optional: sensitivity factor
        yawChange *= rotationSensitivity; // reduce sensitivity

        accumulatedYaw += yawChange;
        if(accumulatedYaw < 0.1){
            accumulatedYaw = 0;
        }
    }

    // Rotate rocket hand to face the new direction
    if (handTransform != null)
    {
        Quaternion targetRotation = Quaternion.LookRotation(launchDirection, Vector3.up);
        handTransform.rotation = Quaternion.Slerp(
            handTransform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}


    Vector3 GetLaunchDirection()
    {
        return (aimDirectionReference ? aimDirectionReference.forward : transform.forward).normalized;
    }
}