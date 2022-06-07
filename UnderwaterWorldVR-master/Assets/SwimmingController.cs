using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SwimmingController : MonoBehaviour
{
    [SerializeField] private float swimmingForce;
    [SerializeField] private float resistanceForce;
    [SerializeField] private float deadZone;
    [SerializeField] private Transform trackingSpace;
    [SerializeField] private float speedThreshold;

    private new Rigidbody rigidbody;
    private Vector3 currentDirection;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        bool rightButtonPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        bool leftButtonPressed = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);

        if (rightButtonPressed && leftButtonPressed)
        {
            Vector3 leftHandDirection = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
            Vector3 rightHandDirection = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
            Vector3 localVelocity = leftHandDirection + rightHandDirection;
            localVelocity *= -1f;
            if (localVelocity.sqrMagnitude > deadZone * deadZone)
            {
                AddSwimmingForce(localVelocity);
                if (localVelocity.sqrMagnitude >= speedThreshold)
                {
                    this.tag = "Shark";
                }
                else
                {
                    this.tag = "Untagged";
                }
            }
 
        }
        ApplyReststanceForce();
    }

    private void ApplyReststanceForce()
    {
        if (rigidbody.velocity.sqrMagnitude > 0.01f && currentDirection != Vector3.zero)
        {
            rigidbody.AddForce(-rigidbody.velocity * resistanceForce, ForceMode.Acceleration);
        }
        else
        {
            currentDirection = Vector3.zero;
        }
    }

    private void AddSwimmingForce(Vector3 localVelocity)
    {
        Vector3 worldSpaceVelocity = trackingSpace.TransformDirection(localVelocity);
        rigidbody.AddForce(worldSpaceVelocity * swimmingForce, ForceMode.Acceleration);
        currentDirection = worldSpaceVelocity.normalized;

    }
}
