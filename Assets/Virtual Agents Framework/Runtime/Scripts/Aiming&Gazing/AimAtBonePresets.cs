using UnityEngine;
using static i5.VirtualAgents.AimAt;

namespace i5.VirtualAgents
{
    public interface IAimAtBonePresets
    {
        // Start is called before the first frame update
        void ApplyPresets(Transform agentTransform, out HumanBone[] humanBones, out AimAt.AimDirection aimDirection, out Transform aimTransform, out float angleLimit);
    }
    public class RightArmPreset : IAimAtBonePresets
    {
        public void ApplyPresets(Transform agentTransform, out HumanBone[] humanBones, out AimAt.AimDirection aimDirection, out Transform aimTransform, out float angleLimit)
        {
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
            aimDirection = AimDirection.Y;
            angleLimit = 180f;
            aimTransform = agentTransform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Shoulder.R/UpperArm.R/LowerArm.R/Hand.R/Palm1.R/IndexProximal.R/IndexIntermediate.R/IndexDistal.R/IndexDistal.R_end");

        }
    }
    public class LeftArmPreset : IAimAtBonePresets
    {
        public void ApplyPresets(Transform agentTransform, out HumanBone[] humanBones, out AimAt.AimDirection aimDirection, out Transform aimTransform, out float angleLimit)
        {
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

            aimDirection = AimDirection.Y;
            angleLimit = 180f;
            aimTransform = agentTransform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Shoulder.L/UpperArm.L/LowerArm.L/Hand.L/Palm1.L/IndexProximal.L/IndexIntermediate.L/IndexDistal.L/IndexDistal.L_end");

        }
    }

    public class RightLegPreset : IAimAtBonePresets
    {
        public void ApplyPresets(Transform agentTransform, out HumanBone[] humanBones, out AimAt.AimDirection aimDirection, out Transform aimTransform, out float angleLimit)
        {
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

            aimDirection = AimDirection.Y;
            angleLimit = 180f;
            aimTransform = agentTransform.Find("MeshDeformRig/Hips/UpperLeg.R/LowerLeg.R/Foot.R/Toes.R/Toes.R_end");

        }
    }
    public class LeftLegPreset : IAimAtBonePresets
    {
        public void ApplyPresets(Transform agentTransform, out HumanBone[] humanBones, out AimAt.AimDirection aimDirection, out Transform aimTransform, out float angleLimit)
        {
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
            aimDirection = AimDirection.Y;
            angleLimit = 180f;
            aimTransform = agentTransform.Find("MeshDeformRig/Hips/UpperLeg.L/LowerLeg.L/Foot.L/Toes.L/Toes.L_end");

        }
    }

    public class HeadPreset : IAimAtBonePresets
    {
        public void ApplyPresets(Transform agentTransform, out HumanBone[] humanBones, out AimAt.AimDirection aimDirection, out Transform aimTransform, out float angleLimit)
        {
            humanBones = new HumanBone[3];
            humanBones[0] = new HumanBone
            {
                bone = HumanBodyBones.Head,
                weight = 0.75f
            };
            humanBones[1] = new HumanBone
            {
                bone = HumanBodyBones.Neck,
                weight = 0.25f
            };
            humanBones[2] = new HumanBone
            {
                bone = HumanBodyBones.UpperChest,
                weight = 0.25f
            };

            angleLimit = 100f;
            aimDirection = AimDirection.Z;

            aimTransform = agentTransform.Find("MeshDeformRig/Hips/Spine/Chest/UpperChest/Neck/Head");

        }
    }
    public class BaseLayerPreset : IAimAtBonePresets
    {
        public void ApplyPresets(Transform agentTransform, out HumanBone[] humanBones, out AimAt.AimDirection aimDirection, out Transform aimTransform, out float angleLimit)
        {
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

            angleLimit = 90.0f;
            aimDirection = AimDirection.Z;

            aimTransform = agentTransform.Find("MeshDeformRig/Hips/Spine/Chest");
        }
    }
}
