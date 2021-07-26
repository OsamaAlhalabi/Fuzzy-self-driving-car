using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carController : MonoBehaviour
{
    public float MotorForce, SteerForce, BrakeForce;
    public WheelCollider FR_L_Wheel, FR_R_Wheel, RE_L_WHeel, RE_R_Wheel;

    public Camera MainCamera;

    public GameObject frontRayCurve;
    public GameObject rightRayCurve;
    public GameObject leftRayCurve;

    private float frontM, rightM, leftM;

    private string frontA, rightA, leftA;

    public float value = 50;

    public float hitDist;
    public float frontHitDist;
    private float lastTurn=0.0f;

    void Calc(Vector3 dist)
    {
        string rulesFired="";
   
        SteerForce = 0.0f;
        float deFuzzyV=30.0f;
        
        frontRayCurve.GetComponent<fuzzyController>().x = dist.x;
        rightRayCurve.GetComponent<fuzzyController>().x = dist.y;
        leftRayCurve.GetComponent<fuzzyController>().x = dist.z;

        this.frontM = frontRayCurve.GetComponent<fuzzyController>().fuzzyValue;
        this.rightM = rightRayCurve.GetComponent<fuzzyController>().fuzzyValue;
        this.leftM = leftRayCurve.GetComponent<fuzzyController>().fuzzyValue;

        this.frontA = frontRayCurve.GetComponent<fuzzyController>().activatedPart;
        this.rightA = rightRayCurve.GetComponent<fuzzyController>().activatedPart;
        this.leftA = leftRayCurve.GetComponent<fuzzyController>().activatedPart;

        if (RE_L_WHeel.rpm > 100)
            RE_L_WHeel.motorTorque = 0;

        if (RE_R_Wheel.rpm > 100)
            RE_R_Wheel.motorTorque = 0;

        value = Mathf.Min(frontM, Mathf.Min(rightM, leftM));

        print("Front ray: " + frontA + " Right ray: " + rightA + " Left ray: " + leftA);

        // fuzzy inference system...
        if(frontA == "far" || frontA =="midFar")
        {
            if(rightA == "far" || rightA == "midFar")
            {
                if(leftA == "far" || leftA == "midFar")
                {
                    RE_R_Wheel.brakeTorque = 0;
                    RE_L_WHeel.brakeTorque = 0;
                    RE_R_Wheel.motorTorque = MotorForce;
                    RE_L_WHeel.motorTorque = MotorForce;
                }
                else if (leftA == "close" || leftA == "midClose" || leftA == "middle")
                {
                    SteerForce += value * deFuzzyV;
                }
            }
            else if (rightA == "middle")
            {
                if(leftA == "close")
                {
                    SteerForce += value * deFuzzyV;
                }
                   
            }
            else if (rightA == "close"|| rightA== "midClose")
            {
                    SteerForce += value * deFuzzyV * -1;
                   
            }


        }
        if(frontA == "close")
        {
            if(rightA == "close" || rightA == "midClose" || rightA == "middle")
            {
                if(leftA == "midClose"||leftA=="close")
                {
                    lastTurn = value * deFuzzyV * -1;
                    RE_R_Wheel.brakeTorque = 1000;
                    RE_L_WHeel.brakeTorque = 1000;
                }
                else
                {
                    lastTurn = value * deFuzzyV * -1;
                    RE_R_Wheel.motorTorque = MotorForce;
                    RE_L_WHeel.motorTorque = MotorForce;
                }
               
            }

            else if (leftA == "close" || leftA == "midClose" || leftA == "middle")
            {
                if (rightA == "midColse"||rightA=="close")
                {
                    lastTurn = value * deFuzzyV;
                    RE_R_Wheel.brakeTorque = 1000;
                    RE_L_WHeel.brakeTorque = 1000;
                }
                else
                {
                    lastTurn = value * deFuzzyV;
                    RE_R_Wheel.motorTorque = MotorForce;
                    RE_L_WHeel.motorTorque = MotorForce;
                }
            }
            else
            {
                SteerForce += value * deFuzzyV;
                RE_R_Wheel.motorTorque = MotorForce;
                RE_L_WHeel.motorTorque = MotorForce;
                RE_R_Wheel.brakeTorque = 100;
                RE_L_WHeel.brakeTorque = 100;
            }

            if (leftA == "close" && rightA == "close")
            {
                RE_R_Wheel.brakeTorque = BrakeForce * 10000;
                RE_L_WHeel.brakeTorque = BrakeForce * 10000;
            }

        }
        if (frontA == "midClose")
        {
            if(leftA =="midClose" || leftA == "close")
            {
                SteerForce += value * deFuzzyV ;
                RE_R_Wheel.motorTorque = MotorForce;
                RE_L_WHeel.motorTorque = MotorForce;
            }
            else if(rightA == "midClose" || rightA == "close")
            {
                SteerForce += value * deFuzzyV *-1;
                RE_R_Wheel.motorTorque = MotorForce;
                RE_L_WHeel.motorTorque = MotorForce;
            }

        }
    
    }

    Vector3 DetectColision(){
        int layerMask = 1 << 8;
        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;
        Vector3 dist=new Vector3(10000.0f,10000.0f,10000.0f);//x:front y:right z:left
        RaycastHit hit;
        Vector3 rayPos = new Vector3(transform.position.x,transform.position.y+1.0f,transform.position.z);
        if (Physics.Raycast(rayPos, transform.TransformDirection(Vector3.forward), out hit, frontHitDist, layerMask))
        {
            Debug.DrawRay(rayPos, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            dist.x = hit.distance/hitDist;
        }
        else
        {
            Debug.DrawRay(rayPos, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        }

        if (Physics.Raycast(rayPos, transform.TransformDirection(new Vector3(1.0f,0.0f,1.4f)), out hit, hitDist, layerMask))
        {
            Debug.DrawRay(rayPos, transform.TransformDirection(new Vector3(1.0f,0.0f,1.4f)) * hit.distance, Color.green);
            dist.y = hit.distance/hitDist;
        }
        else
        {
            Debug.DrawRay(rayPos, transform.TransformDirection(new Vector3(1.0f,0.0f,1.4f)) * 1000, Color.white);
        }

        if (Physics.Raycast(rayPos, transform.TransformDirection(new Vector3(-1.0f,0.0f,1.4f)), out hit, hitDist, layerMask))
        {
            Debug.DrawRay(rayPos, transform.TransformDirection(new Vector3(-1.0f,0.0f,1.4f)) * hit.distance, Color.blue);
            dist.z = hit.distance/hitDist;
        }
        else
        {
            Debug.DrawRay(rayPos, transform.TransformDirection(new Vector3(-1.0f,0.0f,1.4f)) * 1000, Color.white);
        }
        return dist;
    }


    void Start()
    {
        lastTurn=0.0f;
    }
    void Update()
    {
        Vector3 dist = DetectColision();
        Calc(dist);
         Debug.Log("Steer Force: "+SteerForce);
         Debug.Log("last Turn: "+lastTurn);
        
        Vector3 newPostion = new Vector3(this.transform.position.x, this.transform.position.y + 4.0f, this.transform.position.z - 4.0f);
        //MainCamera.transform.position = Vector3.Lerp(newPostion, MainCamera.transform.position, Time.deltaTime * 5);
 
        float v = MotorForce;
        float h = SteerForce;


        RE_R_Wheel.motorTorque = v;
        RE_L_WHeel.motorTorque = v;

        FR_L_Wheel.steerAngle = h;
        FR_R_Wheel.steerAngle = h;

        if (Input.GetKey(KeyCode.Space))
        {
            RE_R_Wheel.brakeTorque = BrakeForce;
            RE_L_WHeel.brakeTorque = BrakeForce;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            RE_R_Wheel.brakeTorque = 0;
            RE_L_WHeel.brakeTorque = 0;
        }

    }
}
