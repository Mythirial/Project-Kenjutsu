using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRController leftController;
    public XRController rightController;
    public InputHelpers.Button teleportActivateButton;
    public float activationThreshold = 0.1f;
    
    // Update is called once per frame
    void Update()
    {
        if(leftController)
            leftController.gameObject.SetActive(CheckIfActivated(leftController));

        if (rightController)
            rightController.gameObject.SetActive(CheckIfActivated(rightController));
    }

    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivateButton, out bool isPressed, activationThreshold);
        return isPressed;
    }
}
