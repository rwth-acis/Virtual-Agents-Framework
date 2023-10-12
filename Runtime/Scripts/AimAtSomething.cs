using System;
using UnityEngine;
using UnityEngine.AI;

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
    /// The Transform of the agent childobjects that should directly aim at the target
    /// </summary>
    public Transform aimTransform;
    /// <summary>
    /// The Transform that is acutally looked at and will follow the target smootly
    /// </summary>
    private Transform targetFollower;

    public float lookSpeed = 2f;
    private float currentLookSpeed = 2f;
    private float increasLookSpeedBy = 0f;

    NavMeshAgent navMeshAgent;

    public int iterations = 10;
    [Range(0, 1)]
    public float weight = 0.8f;

    public float angleLimit = 180.0f;
    //clostest distance at which an object will be aimed at
    public float distanceLimit = 1.5f;

    //The postion where the targetFollower should be placed when no target is set
    public Transform startingTransform;

    public HumanBone[] humanBones;
    Transform[] boneTransforms;
    public enum AimDirection { Y, X, Z };

    //Color of the arrow in unity that should aim at the target
    public AimDirection aimDirection = AimDirection.Y;


    bool shouldDestroyIteselfe = true;

    // Start is called before the first frame update and when a new bonePreset is loaded
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

    }
    // LateUpdate is called once per frame, after Update
    void LateUpdate()
    {
        TemporarlyIncreaseLookSpeed(navMeshAgent.velocity.magnitude);

        if (targetFollower != null)
        {
            UpdateTargetFollower();


            Vector3 targetPosition = CalculateWhereToLook();

            for (int i = 0; i < iterations; i++)
            {
                for (int b = 0; b < humanBones.Length; b++)
                {
                    Transform bone = boneTransforms[b];
                    float boneWeight = humanBones[b].weight * weight;
                    AimAtTarget(bone, targetPosition, boneWeight);
                }

            }
        }

    }

    //Calculates where to aim at based on the target and the angle and distance limit
    Vector3 CalculateWhereToLook()
    {

        Vector3 targetDirection = targetFollower.position - aimTransform.position;
        Vector3 aimDirection = GetAimDirectionVektor();
        float blendOut = 0.0f;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }

        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit)
        {
            blendOut += distanceLimit - targetDistance;
        }


        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }
    void UpdateTargetFollower()
    {

        Vector3 targetPosition;

        //If targetTransform was not removed in Stop()
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            increasLookSpeedBy = 1;
        }
        else
        {
            //Return to the starting posiont
            targetPosition = startingTransform.position;

            //increase LookSpeed over time to finish up the movement
            increasLookSpeedBy = Math.Min(10, increasLookSpeedBy + 0.7f);
            weight = Math.Max(0, weight - 0.01f);

            //When target position of the standard look is reached destroy this component
            if (Vector3.Distance(targetFollower.position, targetPosition) < 0.05f)
            {
                weight = 0f;
                if (shouldDestroyIteselfe)
                {
                    Destroy(targetFollower.gameObject);
                    Destroy(this);
                }

            }

        }

        //Smooth transition to target position
        targetFollower.transform.position = Vector3.Lerp(targetFollower.transform.position, targetPosition, Time.deltaTime * (currentLookSpeed * increasLookSpeedBy));



    }


    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = GetAimDirectionVektor();
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }

    private Vector3 GetAimDirectionVektor()
    {
        if (this.aimDirection == AimDirection.Y)
            return aimTransform.up.normalized;
        if (this.aimDirection == AimDirection.X)
            return aimTransform.right.normalized;
        if (this.aimDirection == AimDirection.Z)
            return aimTransform.forward;

        return aimTransform.up.normalized;
    }

    public void SetTargetTransform(Transform targetTransform)
    {
        if (targetFollower == null)
        {
            targetFollower = new GameObject().transform;
            targetFollower.gameObject.name = "TargetFollower";
            DebugDrawSphere targetFollow = targetFollower.gameObject.AddComponent<DebugDrawSphere>();
            targetFollow.color = Color.red;
            targetFollow.radius = 0.50f;
            //Set starting position of targetFollower 1 unit along the current aiming direction getAimDirectionVektor() * 1f +
            this.startingTransform = new GameObject().transform;
            this.startingTransform.gameObject.name = "StartingPositon";
            this.startingTransform.position = aimTransform.position + (GetAimDirectionVektor() * 1f);
            this.startingTransform.parent = this.transform;

            Transform debug = new GameObject().transform;
            debug.gameObject.name = "Debug";
            debug.position = startingTransform.position;
            debug.gameObject.AddComponent<DebugDrawSphere>();
            this.targetFollower.position = startingTransform.position;
        }

        this.targetTransform = targetTransform;
    }
    public void SetAimTransform(Transform aimTransform)
    {
        this.aimTransform = aimTransform;
    }

    public void SetAimDirection(AimDirection aimDirection)
    {
        this.aimDirection = aimDirection;
    }
    public void SetupAndStart(string layer, Transform target)
    {
        SetBonePreset(layer);
        SetTargetTransform(target);
    }

    public void Stop()
    {
        this.targetTransform = null;
    }

    public void TemporarlyIncreaseLookSpeed(float increase)
    {
        this.currentLookSpeed = lookSpeed + increase;
    }
    public void SetShouldDestroyItself(bool shouldDestroyIteselfe)
    {
        this.shouldDestroyIteselfe = shouldDestroyIteselfe;
    }

    public void SetBonePreset(string layer)
    {
        switch (layer)
        {
            case "Right Arm":
                humanBones = new HumanBone[6];
                humanBones[0] = new HumanBone
                {
                    bone = HumanBodyBones.Spine,
                    weight = 0.056f
                };

                humanBones[1] = new HumanBone
                {
                    bone = HumanBodyBones.RightShoulder,
                    weight = 0.183f
                };

                humanBones[2] = new HumanBone
                {
                    bone = HumanBodyBones.RightUpperArm,
                    weight = 0.442f
                };

                humanBones[3] = new HumanBone
                {
                    bone = HumanBodyBones.RightLowerArm,
                    weight = 0.533f
                };

                humanBones[4] = new HumanBone
                {
                    bone = HumanBodyBones.RightHand,
                    weight = 0.239f
                };

                humanBones[5] = new HumanBone
                {
                    bone = HumanBodyBones.RightIndexProximal,
                    weight = 0.01f
                };
                SetAimDirection(AimDirection.Y);

                SetAimTransform(transform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R/Palm1.R/IndexProximal.R/IndexIntermediate.R/IndexDistal.R/IndexDistal.R_end"));
                break;
            case "Left Arm":
                humanBones = new HumanBone[6];
                humanBones[0] = new HumanBone
                {
                    bone = HumanBodyBones.Spine,
                    weight = 0.056f
                };

                humanBones[1] = new HumanBone
                {
                    bone = HumanBodyBones.LeftShoulder,
                    weight = 0.183f
                };

                humanBones[2] = new HumanBone
                {
                    bone = HumanBodyBones.LeftUpperArm,
                    weight = 0.442f
                };

                humanBones[3] = new HumanBone
                {
                    bone = HumanBodyBones.LeftLowerArm,
                    weight = 0.533f
                };

                humanBones[4] = new HumanBone
                {
                    bone = HumanBodyBones.LeftHand,
                    weight = 0.239f
                };

                humanBones[5] = new HumanBone
                {
                    bone = HumanBodyBones.LeftIndexProximal,
                    weight = 0.01f
                };

                SetAimDirection(AimDirection.Y);

                SetAimTransform(transform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L/Palm1.L/IndexProximal.L/IndexIntermediate.L/IndexDistal.L/IndexDistal.L_end"));
                break;
            case "Right Leg":
                humanBones = new HumanBone[5];

                humanBones[0] = new HumanBone
                {
                    bone = HumanBodyBones.Hips,
                    weight = 0
                };

                humanBones[1] = new HumanBone
                {
                    bone = HumanBodyBones.RightUpperLeg,
                    weight = 0.5f
                };

                humanBones[2] = new HumanBone
                {
                    bone = HumanBodyBones.RightLowerLeg,
                    weight = 1f
                };

                humanBones[3] = new HumanBone
                {
                    bone = HumanBodyBones.RightFoot,
                    weight = 0.8f
                };

                humanBones[4] = new HumanBone
                {
                    bone = HumanBodyBones.RightToes,
                    weight = 0.9f
                };

                SetAimDirection(AimDirection.Y);

                SetAimTransform(transform.Find("MeshDeformRig/Hips/UpperLeg.R/LowerLeg.R/Foot.R/Toes.R/Toes.R_end"));
                break;
            case "Left Leg":
                humanBones = new HumanBone[5];

                humanBones[0] = new HumanBone
                {
                    bone = HumanBodyBones.Hips,
                    weight = 0
                };

                humanBones[1] = new HumanBone
                {
                    bone = HumanBodyBones.LeftUpperLeg,
                    weight = 0.5f
                };

                humanBones[2] = new HumanBone
                {
                    bone = HumanBodyBones.LeftLowerLeg,
                    weight = 1f
                };

                humanBones[3] = new HumanBone
                {
                    bone = HumanBodyBones.LeftFoot,
                    weight = 0.8f
                };

                humanBones[4] = new HumanBone
                {
                    bone = HumanBodyBones.LeftToes,
                    weight = 0.9f
                };
                SetAimDirection(AimDirection.Y);

                SetAimTransform(transform.Find("MeshDeformRig/Hips/UpperLeg.L/LowerLeg.L/Foot.L/Toes.L/Toes.L_end"));
                break;
            case "Head":
                humanBones = new HumanBone[3];
                humanBones[0] = new HumanBone
                {
                    bone = HumanBodyBones.Head,
                    weight = 0.9f
                };
                humanBones[1] = new HumanBone
                {
                    bone = HumanBodyBones.Neck,
                    weight = 0.5f
                };
                humanBones[2] = new HumanBone
                {
                    bone = HumanBodyBones.UpperChest,
                    weight = 0.25f
                };

                this.angleLimit = 100f;
                SetAimDirection(AimDirection.Z);

                SetAimTransform(transform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Neck/Head"));
                break;
            case "Base Layer":
                humanBones = new HumanBone[3];
                humanBones[0] = new HumanBone
                {
                    bone = HumanBodyBones.Chest,
                    weight = 0.75f
                };
                humanBones[1] = new HumanBone
                {
                    bone = HumanBodyBones.Spine,
                    weight = 0.25f
                };
                humanBones[2] = new HumanBone
                {
                    bone = HumanBodyBones.Hips,
                    weight = 0.25f
                };

                this.angleLimit = 90.0f;
                SetAimDirection(AimDirection.Z);

                SetAimTransform(transform.Find("MeshDeformRig/Hips/Spine/Chest"));
                break;
            default:
                Debug.LogWarning("No boneset avaiable for the layer named:" + layer);
                break;
        }

        Animator animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < humanBones.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (startingTransform)
        {
            Gizmos.DrawWireSphere(startingTransform.position, 0.25f);
            Gizmos.DrawLine(aimTransform.position, startingTransform.position);
        }


    }
}
