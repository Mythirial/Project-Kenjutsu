using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ContinuousMovement : MonoBehaviour
{
    public XRNode inputNode;
    public LayerMask groundLayer;
    public float additionalHeight = 0.1f;
    public float speed = 1f;
    public float gravity = -9.81f;

    private XRRig _rig;
    private Vector2 _inputAxis;
    private CharacterController _character;
    private float _fallingSpeed;

    // Start is called before the first frame update
    private void Start()
    {
        _character = GetComponent<CharacterController>();
        _rig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputNode);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out _inputAxis);
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        var headYaw = Quaternion.Euler(0, _rig.cameraGameObject.transform.eulerAngles.y, 0);
        var direction = headYaw * new Vector3(_inputAxis.x , 0 , _inputAxis.y);
        _character.Move(direction * Time.fixedDeltaTime * speed);
        
        //gravity
        if (CheckIfGrounded())
            _fallingSpeed = 0;
        else
            _fallingSpeed += gravity * Time.fixedDeltaTime;

        _character.Move(Vector3.up * _fallingSpeed * Time.fixedDeltaTime);
    }

    private void CapsuleFollowHeadset()
    {
        _character.height = _rig.cameraInRigSpaceHeight + additionalHeight;
        var capsuleCenter = transform.InverseTransformPoint(_rig.cameraGameObject.transform.position);
        _character.center = new Vector3(capsuleCenter.x, _character.height/2 + _character.skinWidth , capsuleCenter.z);
    }

    private bool CheckIfGrounded()
    {
        Vector3 rayStart = transform.TransformPoint(_character.center);
        float rayLength = _character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, _character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
