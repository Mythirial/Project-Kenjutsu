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

        public XRRayInteractor leftRayInteractor;
        public XRRayInteractor righRayInteractor;

        public bool EnableLeftTeleportation { get; set; } = true;
        public bool EnableRightTeleportation { get; set; } = true;

    
        // Update is called once per frame
        private void Update()
        {
            Vector3 pos = new Vector3();
            Vector3 norm = new Vector3();
            int index = 0;
            bool validTarget = false;

            if (leftController)
            {
                bool isLeftRayInteractorHovering = leftRayInteractor.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget); 
                leftController.gameObject.SetActive(EnableLeftTeleportation && CheckIfActivated(leftController) && !isLeftRayInteractorHovering);
            }


            if (rightController)
            {
                bool isRightRayInteractorHovering = righRayInteractor.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
                rightController.gameObject.SetActive(EnableRightTeleportation && CheckIfActivated(rightController) && !isRightRayInteractorHovering);
            }
        }

        public bool CheckIfActivated(XRController controller)
        {
            controller.inputDevice.IsPressed(teleportActivateButton, out bool isPressed, activationThreshold);
            return isPressed;
        }
    }
}
