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

    void Update()
    {
        if (controller == null || joystickInput == null || retractButton == null)
            return;

        // Check if the retract button is pressed
        

        if (isRetracting)
        {
            // Move k toward 0
            controller.k = Mathf.MoveTowards(controller.k, 0f, retractSpeed * Time.deltaTime);
            if(controller.k == 0){
                isRetracting = false;
            }
        }
        else
        {
            isRetracting = retractButton.action.ReadValue<float>() > 0.5f;
            // Normal joystick movement
            Vector2 joystickValue = joystickInput.action.ReadValue<Vector2>();


            controller.k += joystickValue.y * speed * Time.deltaTime;
            controller.k = Mathf.Clamp(controller.k, 0f, 5f);
        }
        SendRayToHand();
    }
    void SendRayToHand()
    {
        Vector3 origin = offset.position;
        Vector3 direction = (handTransform.position - origin).normalized;
        Debug.DrawRay(origin, direction * Vector3.Distance(origin, handTransform.position), Color.red);
        Debug.Log("spot origin: " + origin);
        // Raycast from controller to hand
        if (Physics.Raycast(origin, direction, out RaycastHit hit, Vector3.Distance(origin, handTransform.position)))
        {
            Debug.Log("Ray hit: " + hit.collider.name);
        }

        // Debug visualization (visible in Scene View)
        //Debug.DrawRay(origin, direction * Vector3.Distance(origin, handTransform.position), Color.red);
    }
}
