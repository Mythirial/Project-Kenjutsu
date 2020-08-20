using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class ClimbingMovement : MonoBehaviour
    {
        public static XRController climbingHand;

        private CharacterController _character;
        private ContinuousMovement _continuousMovement;


        // Start is called before the first frame update
        void Start()
        {
            _character = GetComponent<CharacterController>();
            _continuousMovement = GetComponent<ContinuousMovement>();
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (climbingHand)
            {
                _continuousMovement.enabled = false;
                Climb();
            }
            else
            {
                _continuousMovement.enabled = true;
            }
        }

        /// <summary>
        /// Climbing Computations
        /// </summary>
        private void Climb()
        {
            InputDevices.GetDeviceAtXRNode(climbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
            _character.Move(transform.rotation * -velocity * Time.fixedDeltaTime);
        }
    }
}
