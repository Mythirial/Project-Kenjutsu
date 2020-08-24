using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class VRMap
    {
        public Transform vrTarget;
        public Transform rigTarget;
        public Vector3 trackingPositionOffset;
        public Vector3 trackingRotationOffset;

        public void Map()
        {
            rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
            rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
        }
    }

    public class FullbodyVRRig : MonoBehaviour
    {
        [Header("Tracking Settings")] 
        public float turnSmoothness = 3f;
        public VRMap head;
        public VRMap leftHand;
        public VRMap rightHand;

        [Header("Head Settings")]
        public Transform headConstraint;
        private Vector3 _headBodyOffset;

        // Start is called before the first frame update
        private void Start()
        {
            _headBodyOffset = transform.position - headConstraint.position;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            transform.position = headConstraint.position + _headBodyOffset;
            transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, Time.fixedDeltaTime* turnSmoothness);

            head.Map();
            rightHand.Map();
            leftHand.Map();
        }
    }
}
