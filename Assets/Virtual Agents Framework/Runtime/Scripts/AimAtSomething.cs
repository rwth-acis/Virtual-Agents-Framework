using i5.VirtualAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    [Range(0, 1)]
    public float weight;
}
public class AimAtSomething : MonoBehaviour
{
    /// <summary>
    /// The transform that should be aimed at
    /// </summary>
    public Transform targetTransform;
    /// <summary>
    /// The transform of the agent part that should be used as final raycast
    /// </summary>
    public Transform aimTransform;

    public int iterations = 10;
    [Range(0,1)]
    public float weight = 0.8f;

    public float angleLimit = 180.0f;
    public float currentAngleLimit = 180.0f;
    //clostest distance at which an object will be aimed at
    public float distanceLimit = 1.5f;

    public HumanBone[] humanBones;
    Transform[] boneTransforms;

    //Color of the arrow in unity that should aim at the target
    public string aimDirection = "Green";

    // Start is called before the first frame update and when a new bonePreset is loaded
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < humanBones.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
    }

    Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = targetTransform.position - aimTransform.position;
        Vector3 aimDirection = getAimDirectionVektor();
        float blendOut = 0.0f;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if(targetAngle > currentAngleLimit)
        {
            blendOut += (targetAngle - currentAngleLimit) / 50.0f;
        }

        float targetDistance = targetDirection.magnitude;
        if(targetDistance < distanceLimit) {
            blendOut += distanceLimit - targetDistance;
        }


        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(aimTransform == null || targetTransform == null)
        {
            return;
        }

        Vector3 targetPosition = GetTargetPosition();
        
        for(int i = 0; i < iterations; i++)
        {
            for (int b = 0; b < humanBones.Length; b++) {
                Transform bone = boneTransforms[b];
                float boneWeight = humanBones[b].weight * weight;
                AimAtTarget(bone, targetPosition, boneWeight);
            }
            
        }
        
    }

    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = getAimDirectionVektor();
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }

    private Vector3 getAimDirectionVektor()
    {
        if(this.aimDirection == "Green")
            return aimTransform.up.normalized;
        if(this.aimDirection == "Red")
            return aimTransform.right.normalized;    
        if(this.aimDirection == "Blue")
            return aimTransform.forward;

        return aimTransform.up.normalized;
    }
    
    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }
    public void SetAimTransform(Transform aimTransform)
    {
        this.aimTransform = aimTransform;
    }

    public void SetAimDirection(String arrowColor) {
        this.aimDirection = arrowColor;
    }
    public void setBonePreset(string layer)
    {
        switch (layer)
        {
            case "Right Arm":
                humanBones = new HumanBone[6];
                humanBones[0] = new HumanBone();
                humanBones[0].bone = HumanBodyBones.Spine;
                humanBones[0].weight = 0.056f;

                humanBones[1] = new HumanBone();
                humanBones[1].bone = HumanBodyBones.RightShoulder;
                humanBones[1].weight = 0.183f;

                humanBones[2] = new HumanBone();
                humanBones[2].bone = HumanBodyBones.RightUpperArm;
                humanBones[2].weight = 0.442f;

                humanBones[3] = new HumanBone();
                humanBones[3].bone = HumanBodyBones.RightLowerArm;
                humanBones[3].weight = 0.533f;

                humanBones[4] = new HumanBone();
                humanBones[4].bone = HumanBodyBones.RightHand;
                humanBones[4].weight = 0.239f;

                humanBones[5] = new HumanBone();
                humanBones[5].bone = HumanBodyBones.RightIndexProximal;
                humanBones[5].weight = 0.01f;
                
                SetAimDirection("Green");
                break;
            case "Left Arm":
                humanBones = new HumanBone[6];
                humanBones[0] = new HumanBone();
                humanBones[0].bone = HumanBodyBones.Spine;
                humanBones[0].weight = 0.056f;

                humanBones[1] = new HumanBone();
                humanBones[1].bone = HumanBodyBones.LeftShoulder;
                humanBones[1].weight = 0.183f;

                humanBones[2] = new HumanBone();
                humanBones[2].bone = HumanBodyBones.LeftUpperArm;
                humanBones[2].weight = 0.442f;

                humanBones[3] = new HumanBone();
                humanBones[3].bone = HumanBodyBones.LeftLowerArm;
                humanBones[3].weight = 0.533f;

                humanBones[4] = new HumanBone();
                humanBones[4].bone = HumanBodyBones.LeftHand;
                humanBones[4].weight = 0.239f;

                humanBones[5] = new HumanBone();
                humanBones[5].bone = HumanBodyBones.LeftIndexProximal;
                humanBones[5].weight = 0.01f;

                SetAimDirection("Green");
                break;
            case "Right Leg":
                humanBones = new HumanBone[5];

                humanBones[0] = new HumanBone();
                humanBones[0].bone = HumanBodyBones.Hips;
                humanBones[0].weight = 0;

                humanBones[1] = new HumanBone();
                humanBones[1].bone = HumanBodyBones.RightUpperLeg;
                humanBones[1].weight = 0.5f;

                humanBones[2] = new HumanBone();
                humanBones[2].bone = HumanBodyBones.RightLowerLeg;
                humanBones[2].weight = 1f;

                humanBones[3] = new HumanBone();
                humanBones[3].bone = HumanBodyBones.RightFoot;
                humanBones[3].weight = 0.8f;

                humanBones[4] = new HumanBone();
                humanBones[4].bone = HumanBodyBones.RightToes;
                humanBones[4].weight = 0.9f;

                

                SetAimDirection("Green");
                break;
            case "Left Leg":
                humanBones = new HumanBone[5];

                humanBones[0] = new HumanBone();
                humanBones[0].bone = HumanBodyBones.Hips;
                humanBones[0].weight = 0;

                humanBones[1] = new HumanBone();
                humanBones[1].bone = HumanBodyBones.LeftUpperLeg;
                humanBones[1].weight = 0.5f;

                humanBones[2] = new HumanBone();
                humanBones[2].bone = HumanBodyBones.LeftLowerLeg;
                humanBones[2].weight = 1f;

                humanBones[3] = new HumanBone();
                humanBones[3].bone = HumanBodyBones.LeftFoot;
                humanBones[3].weight = 0.8f;

                humanBones[4] = new HumanBone();
                humanBones[4].bone = HumanBodyBones.LeftToes;
                humanBones[4].weight = 0.9f;
                SetAimDirection("Green");
                break;
            case "Head":
                humanBones = new HumanBone[3];
                humanBones[0] = new HumanBone();
                humanBones[0].bone = HumanBodyBones.Head;
                humanBones[0].weight = 0.9f;
                humanBones[1] = new HumanBone();
                humanBones[1].bone = HumanBodyBones.Neck;
                humanBones[1].weight = 0.5f;
                humanBones[2] = new HumanBone();
                humanBones[2].bone = HumanBodyBones.UpperChest;
                humanBones[2].weight = 0.25f;

                this.angleLimit = 90.0f;
                SetAimDirection("Blue");
                break;
            case "Base Layer":
                humanBones = new HumanBone[3];
                humanBones[0] = new HumanBone();
                humanBones[0].bone = HumanBodyBones.Chest;
                humanBones[0].weight = 0.75f;
                humanBones[1] = new HumanBone();
                humanBones[1].bone = HumanBodyBones.Spine;
                humanBones[1].weight = 0.25f;
                humanBones[2] = new HumanBone();
                humanBones[2].bone = HumanBodyBones.Hips;
                humanBones[2].weight = 0.25f;

                this.angleLimit = 90.0f;
                SetAimDirection("Blue");
                break;
            default:
                Debug.LogWarning("No boneset avaiable for the layer named:" + layer);   
                break;
        }

        Start();
    }
    public IEnumerator fadeStop()
    {
        while (angleLimit >= 5)
        {
            this.angleLimit = angleLimit * 0.95f;
            yield return new WaitForSeconds(0.05f);
        }
 
        this.aimTransform = null;
        this.targetTransform = null;
    }
}
