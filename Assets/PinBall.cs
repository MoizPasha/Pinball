using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class PinBall : MonoBehaviour
{
    private Rigidbody Ball;
    //public float forceMultiplier = 10f;


    //lanuchpad
    private Rigidbody LaunchPad;
    public float LaunchPad_ReverseSpeed = 6f;
    public float LaunchPad_ForwardSpeed = 80f;
    private float LaunchPad_Room = 2.5f;
    private bool LaunchPad_Pressed= false;
    private bool LaunchPad_Rebound= false;
    private bool LaunchPad_Stop= false;
   
    private Vector3 LaunchPad_Position;

    //handles
    public float Handle_Torque = 400f;
    public float MaxRotation = 70f;
    public float Handle_ReboundTorque = 250f;
    public float HandlePowerFactor= 0.5f;
    //left handle
    private Rigidbody LeftHandle;
    private bool LeftHandle_Pressed = false;
    private bool LeftHandle_Rebound = false;
    private bool LeftHandle_Stop = false;
    private float LeftHandle_Angle;
    private float LeftHandle_MaxAngle;
    //right handle
    private Rigidbody RightHandle;
    private bool RightHandle_Pressed = false;
    private bool RightHandle_Rebound = false;
    private bool RightHandle_Stop = false;
    private float RightHandle_MaxAngle;
    private float RightHandle_Angle;

    //public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        Ball = GameObject.FindGameObjectsWithTag("Ball")[0].GetComponent<Rigidbody>();
        //launchpad
        LaunchPad = GameObject.FindGameObjectsWithTag("LaunchPad")[0].GetComponent<Rigidbody>();
        LaunchPad_Position = LaunchPad.transform.position;
        //lefthandle
        GameObject LH = GameObject.FindGameObjectsWithTag("LeftHandlePivot")[0];
        LeftHandle=LH.GetComponent<Rigidbody>();
        LeftHandle_Angle = LH.transform.eulerAngles.y;
        LeftHandle_MaxAngle = LeftHandle_Angle - MaxRotation;
        //right handle
        GameObject RH = GameObject.FindGameObjectsWithTag("RightHandlePivot")[0];
        RightHandle =RH.GetComponent<Rigidbody>();
        RightHandle_Angle = RH.transform.eulerAngles.y;
        RightHandle_MaxAngle = RightHandle_Angle + MaxRotation;
    }
    private void FixedUpdate()
    {
        //launchpad
        if (LaunchPad_Pressed && !LaunchPad_Stop)
        {
            LaunchPad.transform.position = new Vector3(LaunchPad.transform.position.x, LaunchPad.transform.position.y, LaunchPad.transform.position.z - LaunchPad_ReverseSpeed * Time.deltaTime);
            if (LaunchPad.transform.position.z <= LaunchPad_Position.z - LaunchPad_Room)
            {
                LaunchPad_Stop = true;
            }
        }
        else if (LaunchPad_Rebound)
        {
            LaunchPad_Stop = false;
            if (LaunchPad.position.z >= LaunchPad_Position.z)
            {
                LaunchPad_Rebound = false;
            }
            else
            {
                Vector3 tempVect = new Vector3(0, 0, 1);
                tempVect = tempVect.normalized * LaunchPad_ForwardSpeed * Time.deltaTime;
                if(LaunchPad.position.z + tempVect.z > LaunchPad_Position.z)
                    tempVect = new Vector3(0, 0, LaunchPad_Position.z - LaunchPad.position.z);
                LaunchPad.MovePosition(LaunchPad.position+tempVect);
            }
        }
        //lefthandle
        if (LeftHandle_Pressed)
        { 
            if (!LeftHandle_Stop)
            {
                if (LeftHandle.transform.eulerAngles.y >= LeftHandle_MaxAngle)
                {
                    LeftHandle_Rebound = true;
                    LeftHandle.AddTorque(new Vector3(0, -1, 0) * Handle_Torque * Time.deltaTime, ForceMode.VelocityChange);
                }
                else
                {
                    LeftHandle.angularVelocity = Vector3.zero;
                    LeftHandle.velocity = Vector3.zero;
                    LeftHandle_Stop = true;
                }
            }
        }
        else if(LeftHandle_Rebound)
        {
            if (LeftHandle.transform.eulerAngles.y <= LeftHandle_Angle)
                LeftHandle.AddTorque(new Vector3(0, 1, 0) * Handle_ReboundTorque * Time.deltaTime, ForceMode.Impulse);
            else
            {
                LeftHandle.angularVelocity = Vector3.zero;
                LeftHandle.velocity = Vector3.zero;
                LeftHandle_Rebound = false;
            }
            LeftHandle_Stop = false;
        }
        //righthandle
        if (RightHandle_Pressed )
        {
            if (!RightHandle_Stop)
            {
                if (RightHandle.transform.eulerAngles.y <= RightHandle_MaxAngle)
                {
                    RightHandle_Rebound = true;
                    RightHandle.AddTorque(new Vector3(0, +1, 0) * Handle_Torque * Time.deltaTime, ForceMode.VelocityChange);
                }
                else
                {
                    RightHandle.angularVelocity = Vector3.zero;
                    RightHandle.velocity = Vector3.zero;
                    RightHandle_Stop = true;
                }
            }
        }
        else if (RightHandle_Rebound)
        {
            if (RightHandle.transform.eulerAngles.y >= RightHandle_Angle)
                RightHandle.AddTorque(new Vector3(0, -1, 0) * Handle_ReboundTorque * Time.deltaTime, ForceMode.Impulse);
            else
            {
                RightHandle.angularVelocity = Vector3.zero;
                RightHandle.velocity = Vector3.zero;
                RightHandle_Rebound = false;
            }
            RightHandle_Stop = false;

        }
    }
    //handleforce
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "LeftHandle")
        {

            float distance = Vector3.Distance(Ball.transform.position, LeftHandle.transform.position);
            float magnitude = LeftHandle.angularVelocity.magnitude * (HandlePowerFactor * distance);
            float angle = (360 - LeftHandle.transform.eulerAngles.y) * (Mathf.PI / 180); ;
            Vector3 velocity = new Vector3(magnitude * Mathf.Cos(angle), 0, magnitude * Mathf.Sin(angle));

            //print("LeftHandle" +" "+angle*(180/Mathf.PI)+" "+magnitude+" "+ velocity);
            if (LeftHandle.angularVelocity.y <= 0)
            {
                Ball.AddForce(velocity, ForceMode.VelocityChange);
            }
            else
            {
                Ball.AddForce(0.1f*velocity*-1, ForceMode.VelocityChange);
            }
        }
        else if (collision.gameObject.name == "RightHandle")
        {

            float distance = Vector3.Distance(Ball.transform.position, RightHandle.transform.position);
            float magnitude = RightHandle.angularVelocity.magnitude * (HandlePowerFactor * distance);
            float angle = (180 - RightHandle.transform.eulerAngles.y) * (Mathf.PI / 180);
            Vector3 velocity = new Vector3(magnitude * Mathf.Cos(angle), 0, magnitude * Mathf.Sin(angle));
            if (RightHandle.angularVelocity.y >= 0)
            {
                Ball.AddForce(velocity, ForceMode.VelocityChange);
            }
            else
            {
                Ball.AddForce(0.1f * velocity * -1, ForceMode.VelocityChange);
            }
           
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "LargeNut")
        {
            Vector3.Reflect(Ball.velocity.normalized * 2f, collision.contacts[0].normal);
        }
    }
    //launchpad
    public void OnPointerDown_LaunchPad()
    {
        LaunchPad_Pressed = true;
    }
    public void OnPointerUp_LaunchPad()
    {
        LaunchPad_Pressed = false;
        LaunchPad_Rebound = true;
    }
    //handles
    //left
    public void OnPointerDown_LeftHandle()
    {
        LeftHandle_Pressed = true;
    }
    public void OnPointerUp_LeftHandle()
    {
        LeftHandle_Pressed = false;
 
    }
    //right
    public void OnPointerDown_RightHandle()
    {
        RightHandle_Pressed = true;
    }
    public void OnPointerUp_RightHandle()
    {
        RightHandle_Pressed = false;
      
    }
}
