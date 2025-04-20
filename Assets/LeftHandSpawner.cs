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
        if(hasSpawned){
            realHand.transform.position = Vector3.zero;
        }
    }

    private void TrySpawnLeftHand()
    {
        if (spawnedLeftHand != null) return;

        Transform spawnLocation = spawnPoint != null ? spawnPoint : transform;
        realHand.SetActive(false);
        spawnedLeftHand = Instantiate(leftHandPrefab, spawnLocation.position, spawnLocation.rotation);
        spawnedLeftHand.SetActive(true);

        hasSpawned = true;
        spawnTimer = 0f;  // Reset the timer when the hand is spawned
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
        realHand.SetActive(true);
        spawnedLeftHand = null;
        hasSpawned = false;
        spawnTimer = 0f; // Reset the timer when the hand is cleared
    }
}