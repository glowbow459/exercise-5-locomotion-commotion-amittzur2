using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LeftHandSpawner : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference spawnAction; // Button to press (hold or tap)

    [Header("Left Hand Prefab")]
    public GameObject leftHandPrefab;
    public Transform spawnPoint;
    public GameObject realHand;

    [Header("Hand Life Time")]
    public float handLifetime = 5f;  // Time before the hand is automatically destroyed

    [Header("XR Rig")]
    //public Transform xrRig; // Assign your XR Rig or XR Origin here
    //public float followSpeed = 1f; // Smoothing factor
    public GameObject mainBody;
    //private Vector3 originalLocation;
    [Header("Camera Settings")]
    public Camera vrCamera;
    public Transform cameraOriginalParent;  // Set this manually to XR rig or camera offset parent
    private Transform handCameraMount;      // Will be found on the spawned hand

    private GameObject spawnedLeftHand;
    private bool hasSpawned = false;
    private float spawnTimer = 0f;

    void Update()
    {
        if (spawnAction == null) return;

        float value = spawnAction.action.ReadValue<float>();

        if (value > 0.5f && !hasSpawned)
        {
            TrySpawnLeftHand();
        }
        else if (value <= 0.5f && hasSpawned)
        {
            // Destroy the hand when the button is released
            DestroyHand();
        }

        // If the hand is spawned, count down the timer
        if (hasSpawned)
        {
            spawnTimer += Time.deltaTime;

            // If timer exceeds the lifetime, destroy the hand
            if (spawnTimer >= handLifetime)
            {
                DestroyHand();
            }
        }
    }
    void LateUpdate()
    {
        if (hasSpawned)
        {
            // Move real hand out of view (your current code)
            realHand.transform.position = Vector3.zero;

            // Smoothly move XR rig toward the spawned hand
            vrCamera.transform.position = Vector3.Lerp(vrCamera.transform.position, handCameraMount.position, Time.deltaTime * 10f);
            vrCamera.transform.rotation = Quaternion.Slerp(vrCamera.transform.rotation, handCameraMount.rotation, Time.deltaTime * 10f);
        }
    }


    private void TrySpawnLeftHand()
    {
        if (spawnedLeftHand != null) return;

        Transform spawnLocation = spawnPoint != null ? spawnPoint : transform;
        realHand.SetActive(false);
        spawnedLeftHand = Instantiate(leftHandPrefab, spawnLocation.position, spawnLocation.rotation);
        spawnedLeftHand.SetActive(true);

        // Try to find a child object named "CameraMount" on the hand
        handCameraMount = spawnedLeftHand.transform.Find("CameraMount");
        if (handCameraMount != null && vrCamera != null)
        {
            vrCamera.transform.SetParent(handCameraMount);
            vrCamera.transform.localPosition = Vector3.zero;
            vrCamera.transform.localRotation = Quaternion.identity;
        }
        mainBody.SetActive(false);

        hasSpawned = true;
        spawnTimer = 0f;
    }

    private void DestroyHand()
    {
        if (spawnedLeftHand != null)
        {
            Destroy(spawnedLeftHand);
            ClearSpawnedHand();
        }
    }

    public void ClearSpawnedHand()
    {
        if (vrCamera != null && cameraOriginalParent != null)
        {
            vrCamera.transform.SetParent(cameraOriginalParent);
            vrCamera.transform.localPosition = Vector3.zero;
            vrCamera.transform.localRotation = Quaternion.identity;
        }
        mainBody.SetActive(true);

        realHand.SetActive(true);
        spawnedLeftHand = null;
        hasSpawned = false;
        spawnTimer = 0f;
    }

}