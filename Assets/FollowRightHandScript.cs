using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRightHandScript : MonoBehaviour
{
    public CustomActionBasedController controller;
    public Transform rightHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller != null && rightHand != null)
        {
            rightHand.position = controller.transform.position;
            rightHand.rotation = controller.transform.rotation;
        }
    }


}
