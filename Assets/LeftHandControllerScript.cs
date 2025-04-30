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

    void Start()
    {
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
        if (handTransform != null && aimDirectionReference != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(launchDirection);
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