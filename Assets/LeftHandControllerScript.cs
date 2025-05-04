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
    public float yawDeadZone = 2f;
    public float pitchDeadZone = 2f; // degrees
    
    private Vector3 baseForward;
    private Quaternion previousAimRotation;
    private float accumulatedYaw = 0f;
    private float accumulatedPitch = 0f;
    

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
    Vector3 launchDirection = Quaternion.Euler(accumulatedPitch, accumulatedYaw, 0) * Vector3.forward;

    Debug.Log("Yaw: " + accumulatedYaw);
    rb.velocity = launchDirection * currentSpeed;

    // Update yaw based on real hand rotation delta
    if (aimDirectionReference != null)
    {
        // Save current rotation
        Quaternion currentRotation = aimDirectionReference.rotation;

        // Get delta rotation
        Quaternion delta = currentRotation * Quaternion.Inverse(previousAimRotation);

        // Convert delta to local Euler angles
        Vector3 deltaEuler = delta.eulerAngles;

        // Normalize to [-180, 180]
        deltaEuler.x = Mathf.DeltaAngle(0, deltaEuler.x);
        deltaEuler.y = Mathf.DeltaAngle(0, deltaEuler.y);

        // Apply dead zone and sensitivity to yaw
        float yawChange = Mathf.Abs(deltaEuler.y) < yawDeadZone ? 0f :
            (deltaEuler.y > 0 ? deltaEuler.y - yawDeadZone : deltaEuler.y + yawDeadZone);
        yawChange *= rotationSensitivity;
        accumulatedYaw += yawChange;

        // Apply dead zone and sensitivity to pitch
        float pitchChange = Mathf.Abs(deltaEuler.x) < pitchDeadZone ? 0f :
            (deltaEuler.x > 0 ? deltaEuler.x - pitchDeadZone : deltaEuler.x + pitchDeadZone);
        pitchChange *= rotationSensitivity;
        accumulatedPitch += pitchChange;

        // Clamp pitch if you want to avoid going vertical/upside down:
        accumulatedPitch = Mathf.Clamp(accumulatedPitch, -60f, 60f);

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