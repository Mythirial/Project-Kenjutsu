using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts
{
    public class VRAnimatorController : MonoBehaviour
    {
        [SerializeField] private float _speedThresHold = 0.1f;
        [SerializeField] [Range(0, 1)] private float _smoothing = 1f;

        private Animator _animator;
        private Vector3 _previousPos;
        private FullbodyVRRig _vrRig;

        // Start is called before the first frame update
        private void Start()
        {
            _animator = GetComponent<Animator>();
            _vrRig = GetComponent<FullbodyVRRig>();
            _previousPos = _vrRig.head.vrTarget.position;
        }

        // Update is called once per frame
        void Update()
        {
            //compute the speed
            Vector3 headsetSpeed = (_vrRig.head.vrTarget.position - _previousPos) / Time.deltaTime;
            headsetSpeed.y = 0;

            //local speed
            Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
            _previousPos = _vrRig.head.vrTarget.position;

            //set Animator Values
            float previousDirectionX = _animator.GetFloat("DirectionX");
            float previousDirectionY = _animator.GetFloat("DirectionY");

            _animator.SetBool("IsMoving", headsetLocalSpeed.magnitude > _speedThresHold);
            _animator.SetFloat("DirectionX", math.lerp(previousDirectionX, math.clamp(headsetLocalSpeed.x, -1, 1), _smoothing));
            _animator.SetFloat("DirectionY", math.lerp(previousDirectionY, math.clamp(headsetLocalSpeed.z, -1, 1), _smoothing));
        }
    }
}
