using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRealistic : MonoBehaviour
{
    [Header("Motor")]
    [SerializeField] private List<AxleInfo> axleInfos = null;
    [SerializeField] private float maxMotorTorque = 1000;

    [Header("Steering")]
    [SerializeField] private float maxSteeringAngle = 50;

    public void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }

}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}