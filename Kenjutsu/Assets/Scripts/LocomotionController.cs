using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class LocomotionController : MonoBehaviour
    {
        public XRController leftController;
        public XRController rightController;
        public InputHelpers.Button teleportActivateButton;
        public float activationThreshold = 0.1f;

        public bool EnableLeftTeleportation { get; set; } = true;
        public bool EnableRightTeleportation { get; set; } = true;

    
        // Update is called once per frame
        private void Update()
        {
            if(leftController)
                leftController.gameObject.SetActive(EnableLeftTeleportation && CheckIfActivated(leftController));

            if (rightController)
                rightController.gameObject.SetActive(EnableRightTeleportation && CheckIfActivated(rightController));
        }

        public bool CheckIfActivated(XRController controller)
        {
            controller.inputDevice.IsPressed(teleportActivateButton, out bool isPressed, activationThreshold);
            return isPressed;
        }
    }
}
