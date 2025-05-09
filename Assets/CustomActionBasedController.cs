using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomActionBasedController : ActionBasedController
{
    public float k = 0;
    private Vector3 controlerPosition = Vector3.zero;
    private Vector3 forwardDirection = Vector3.zero;

    protected override void UpdateTrackingInput(XRControllerState controllerState)
    {
        base.UpdateTrackingInput(controllerState);
        
        // Move k units in the direction the controller is facing
        forwardDirection = controllerState.rotation * Vector3.forward;
        controlerPosition = controllerState.position; 
        controllerState.position +=  (forwardDirection * k);
    }

    public void SetK(float newK){
        k = newK;
    }
    public Vector3 GetControlerPosition(){
        return controlerPosition;
    }
    public Vector3 GetForwardDirection()
    {
        return forwardDirection;
    }
}
