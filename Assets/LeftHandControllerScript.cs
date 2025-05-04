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
    
    private Vector3 baseForward;

    void Start()
    {
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

        // Calculate acceleration factor (0 to 1)
        float accelerationFactor = Mathf.Clamp01(elapsedTime / accelerationTime);

        Vector3 launchDirection = GetLaunchDirection();
        float currentSpeed = rocketForce * accelerationFactor;

        rb.velocity = launchDirection * currentSpeed;

        // Smoothly rotate the hand toward the launch direction
        // New: Rotational deviation spin
        if (handTransform != null && aimDirectionReference != null)
        {
            // Step 1: Define the base forward (could be world forward or some locked direction)

            // Step 2: Get the deviation from the real hand
            Vector3 currentDirection = GetLaunchDirection();

            // Calculate signed angle difference around a reference axis (usually up)
            float angleOffset = Vector3.SignedAngle(baseForward, currentDirection, Vector3.up);

            // Use the angle offset to create a spin direction
            float spinSpeed = angleOffset * rotationSmoothSpeed;

            // Apply rotation continuously around up axis
            handTransform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            }

    }

    Vector3 GetLaunchDirection()
    {
        return (aimDirectionReference ? aimDirectionReference.forward : transform.forward).normalized;
    }
}