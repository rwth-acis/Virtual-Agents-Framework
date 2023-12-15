using i5.VirtualAgents.Utilities;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents
{
	/// <summary>
	/// Implements the functionality of aiming at a target
	/// </summary>
	public abstract class AimAt : MonoBehaviour
	{
		/// <summary>
		/// The transform that should be aimed at
		/// </summary>
		[SerializeField] protected Transform targetTransform;

		/// <summary>
		/// The Transform of the agent childobjects that should directly aim at the target
		/// </summary>
		[SerializeField] protected Transform aimTransform;
		// Axis of the aimTransform that should aim at the target
		protected AimDirection aimDirection = AimDirection.Y;
		/// <summary>
		/// The Transform that is acutally looked at and will follow the target smootly
		/// </summary>
		protected Transform targetFollower;

		protected float currentLookSpeed = 2f;
		protected float increaseLookSpeedBy = 0f;

		protected NavMeshAgent navMeshAgent;

		[SerializeField] protected int iterations = 10;

		[SerializeField] protected float angleLimit = 180.0f;
		// closest distance at which an object will be aimed at
		[SerializeField] protected float distanceLimit = 1.5f;

		// The postion where the targetFollower should be placed when no target is set
		[SerializeField] protected Transform startingTransform;

		[SerializeField] protected HumanBone[] humanBones;
		protected Transform[] boneTransforms;
		public enum AimDirection { Y, X, Z };


		/// <summary>
		/// <see langword="true"/> if the component should destroy itself, when the aiming stops and the aim is back at the starting position
		/// </summary>
		public bool ShouldDestroyItself { get; set; } = true;

		[field: SerializeField]
		public float LookSpeed { get; set; } = 2f;

		[field: Range(0, 1)]
		[field: SerializeField]
		public float Weight { get; set; } = 0.8f;

		// Start is called before the first frame update
		protected virtual void Start()
		{
			navMeshAgent = GetComponent<NavMeshAgent>();
		}

        /// <summary>
        /// Starts the aiming at the target with the given layer and target
        /// </summary>
        /// <param name="target">The transform of the object that should be aimed at</param>
        /// <param name="shouldDestroyItself">If the component should destroy itself after aiming is stopped</param>
        public void SetupAndStart(Transform target, bool shouldDestroyItself = true)
		{
			SetBonePreset();
			this.ShouldDestroyItself = shouldDestroyItself;
			SetTargetTransform(target);
		}

		/// <summary>
		/// Removes the targetTransform, which results in the aim to return to the starting position, if shouldDestroyItself is set to true, the component will be destroyed after the aim is back at the starting position
		/// </summary>
		public void Stop()
		{
			this.targetTransform = null;
		}

		// LateUpdate is called once per frame, after Update
		protected void LateUpdate()
		{
			TemporarilyIncreaseLookSpeed(navMeshAgent.velocity.magnitude);

			if (targetFollower != null)
			{
				UpdateTargetFollower();

				Vector3 targetPosition = CalculateWhereToLook();

				for (int i = 0; i < iterations; i++)
				{
					for (int b = 0; b < humanBones.Length; b++)
					{
						Transform bone = boneTransforms[b];
						float boneWeight = humanBones[b].weight * Weight;
						AimAtTarget(bone, targetPosition, boneWeight);
					}
				}
			}
		}

		// Calculates where to aim at based on the target and the angle and distance limit
		protected Vector3 CalculateWhereToLook()
		{

			Vector3 targetDirection = targetFollower.position - aimTransform.position;
			Vector3 aimDirection = GetAimDirectionVector();
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

		protected void UpdateTargetFollower()
		{
			Vector3 targetPosition;

			// If targetTransform was not removed in Stop()
			if (targetTransform != null)
			{
				targetPosition = targetTransform.position;
				increaseLookSpeedBy = 1;
			}
			else
			{
				// Return to the starting posiont
				targetPosition = startingTransform.position;


				if (Vector3.Distance(targetFollower.position, targetPosition) >= 0.05f)
				{
					// increase LookSpeed over time to finish up the movement
					increaseLookSpeedBy = Math.Min(10, increaseLookSpeedBy + 0.7f);
					Weight = Math.Max(0, Weight - 0.01f);
				}
				else
				{
					// When target position of the standard look is reached destroy this component
					Weight = 0f;
					if (ShouldDestroyItself)
					{
						Destroy(targetFollower.gameObject);
						Destroy(this);
					}
				}

			}

			// Smooth transition to target position
			targetFollower.transform.position = Vector3.Lerp(targetFollower.transform.position, targetPosition, Time.deltaTime * (currentLookSpeed * increaseLookSpeedBy));
		}


		protected void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
		{
			Vector3 aimDirection = GetAimDirectionVector();
			Vector3 targetDirection = targetPosition - aimTransform.position;
			Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
			Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
			bone.rotation = blendedRotation * bone.rotation;
		}

		protected Vector3 GetAimDirectionVector()
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
			// If there is no targetFollower, create one
			if (targetFollower == null)
			{
				targetFollower = new GameObject().transform;
				targetFollower.gameObject.name = "TargetFollower";
				DebugDrawTransformSphere targetVisualizer = targetFollower.gameObject.AddComponent<DebugDrawTransformSphere>();
				targetVisualizer.color = Color.red;
				targetVisualizer.radius = 0.50f;

				// Set starting position of targetFollower 1 unit along the current aiming direction getAimDirectionVektor() * 1f +
				this.startingTransform = new GameObject().transform;
				this.startingTransform.gameObject.name = "StartingPositon";
				this.startingTransform.position = aimTransform.position + (GetAimDirectionVector() * 1f);
				this.startingTransform.parent = this.transform;
				this.targetFollower.position = startingTransform.position;
			}

			this.targetTransform = targetTransform;
		}
		public void TemporarilyIncreaseLookSpeed(float increase)
		{
			this.currentLookSpeed = LookSpeed + increase;
		}

		/// <summary>
		/// Instead of using a bone preset, the bones can be selected and weighted manually
		/// </summary>
		/// <param name="humanBones">The bones and weights that should be moved to accomplish the aiming</param>
		/// <param name="aimDirection">The direction going out of the aimTransform that should directly point at the target</param>
		/// <param name="aimTransform">The last point of the bones that should directly point at the target</param>
		/// <param name="angleLimit">The limit at which pointing will be stopped, i.e. 90f to only aim when target is somewhere in front of the agent</param>
		public void UseNewBoneset(HumanBone[] humanBones, AimDirection aimDirection, Transform aimTransform, float angleLimit)
		{
			this.humanBones = humanBones;
			this.aimDirection = aimDirection;
			this.aimTransform = aimTransform;
			this.angleLimit = angleLimit;

		}
		/// <summary>
		/// To set up the aiming at a specific body part, a preset of bones and weights and related settings can be selected
		/// </summary>
		/// <param name="layer">Which bonepreset should be selected based on the layer of the human body</param>
		public abstract void SetBonePreset();


		protected void GetBoneTransformsFromAnimatior(HumanBodyBones aimingTip)
		{
			Animator animator = GetComponent<Animator>();
			boneTransforms = new Transform[humanBones.Length];
			for (int i = 0; i < humanBones.Length; i++)
			{
				boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
			}
            aimTransform = animator.GetBoneTransform(aimingTip);

        }

		protected void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			if (startingTransform)
			{
				Gizmos.DrawWireSphere(startingTransform.position, 0.25f);
				Gizmos.DrawLine(aimTransform.position, startingTransform.position);
			}
		}
	}
}