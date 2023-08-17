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
    public Transform targetTransform;
    public Transform aimTransform;

    public int iterations = 10;
    [Range(0,1)]
    public float weight = 1.0f;

    public float angleLimit = 90.0f;
    public float distanceLimit = 1.5f;

    public HumanBone[] humanBones;
    Transform[] boneTransforms;

    // Start is called before the first frame update
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
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0.0f;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if(targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
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
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }
    public void SetAimTransform(Transform aimTransform)
    {
        this.aimTransform = aimTransform;
    }
    public IEnumerator fadeStop()
    {
        while (weight >= 0.001)
        {
            this.weight = weight * 0.95f;
            yield return new WaitForSeconds(0.05f);
        }
 
        this.aimTransform = null;
        this.targetTransform = null;
    }
}
