using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickKAdjuster : MonoBehaviour
{
    public CustomActionBasedController controller;
    public InputActionReference joystickInput; // Assign in Inspector
    public InputActionReference retractButton; // New retract button
    public Transform handTransform;
    public Transform offset;
    public float speed = 0.1f; // Sensitivity of adjustment
    public float retractSpeed = 2f; // Speed of retraction
    private bool isRetracting = false;
    private float retractTime = 0f; // Track how long we've been retracting
    public float growthRate = 2f;
    public float safeDistanceFromCollider = 2f;
    private bool stop = false;

    void Update()
    {
        if (controller == null || joystickInput == null || retractButton == null)
            return;

        // Check if the retract button is pressed
        

        if (isRetracting)
        {
            retractTime += Time.deltaTime;

            // Exponential speed increase: base * e^(rate * time)
            float exponentialSpeed = retractSpeed * Mathf.Exp(retractTime * growthRate); // 2f = growth rate, tweak it!

            controller.k = Mathf.MoveTowards(controller.k, 0f, exponentialSpeed * Time.deltaTime);
            if (controller.k == 0f)
            {
                isRetracting = false;
                retractTime = 0f; // Reset timer
            }
        }
        else
        {
            
            isRetracting = retractButton.action.ReadValue<float>() > 0.5f;
            // Normal joystick movement
            Vector2 joystickValue = joystickInput.action.ReadValue<Vector2>();
            // Attempted new k value
            float proposedK = controller.k + joystickValue.y * speed * Time.deltaTime;
            proposedK = Mathf.Clamp(proposedK, 0f, 5f);

            // Only do a collision check if trying to increase length
            if (!stop)
            {

                controller.k = proposedK;
            }

        }
        stop = false;
        Vector3 realHandPositionTwo = controller.GetControlerPosition(); // This is the original tracked position

        // 2. Get the virtual/extended hand position (the current position of this GameObject)
        Vector3 virtualHandPosition = handTransform.position;

        // 3. Direction and distance from real to virtual hand
        Vector3 direction = virtualHandPosition - realHandPositionTwo;
        float distance = direction.magnitude;

        // 4. Raycast
        if (Physics.Raycast(realHandPositionTwo, direction.normalized, out RaycastHit hit, distance))
        {
            Debug.Log("Collider detected between real and virtual hand: " + hit.collider.name);
            controller.k = 0;
            // Optionally, do something like stop arm extension or give haptic feedback
        }
        else
        {
            if (Physics.Raycast(realHandPositionTwo, direction.normalized, out RaycastHit hit2, 1000f))
            {
                float safeDistance = hit2.distance - safeDistanceFromCollider; // Keep a small gap
                if (distance > safeDistance)
                {
                    controller.k -= (distance - safeDistance)*0.1f;
                }
                
            }
        }

    }
}
