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
		[Tooltip("The transform that should be aimed at")]
		[SerializeField] protected Transform targetTransform;

		/// <summary>
		/// The Transform of the agent child objects that should directly aim at the target, e.g. the tip of a finger 
		/// </summary>
		[Tooltip("The Transform of the agent child objects that should directly aim at the target")]
		[SerializeField] protected Transform aimTransform;

		/// <summary>
		/// Axis of the aimTransform that should aim at the target
		/// </summary>
		protected AimDirection aimDirection = AimDirection.Y;

		/// <summary>
		/// The Position that is actually looked at and which will follow the target smoothly
		/// </summary>
		protected Vector3 targetFollower;
		protected bool isTargetFollowerInitialized = false;

		/// <summary>
		/// The speed at which the agent looks at the target
		/// </summary>
		protected float currentLookSpeed = 2f;

		/// <summary>
		/// To increase the look speed, maximal value is 10
		/// </summary>
		protected float increaseLookSpeedBy = 0f;

		/// <summary>
		/// Reference to the NavMeshAgent component
		/// </summary>
		protected NavMeshAgent navMeshAgent;

		/// <summary>
		/// The number of iterations that the aiming algorithm should run
		/// </summary>
		[Tooltip("The number of iterations that the aiming algorithm should run")]
		[SerializeField] protected int iterations = 10;

		/// <summary>
		/// The angle limit at which the aiming should stop
		/// </summary>
		[Tooltip("The angle limit at which the aiming should stop")]
		[SerializeField] protected float angleLimit = 180.0f;

		/// <summary>
		/// The closest distance at which an object will be aimed at
		/// </summary>
		[Tooltip("The closest distance at which an object will be aimed at")]
		[SerializeField] protected float distanceLimit = 1.5f;

		/// <summary>
		/// The position where the targetFollower should be placed when no target is set
		/// </summary>
		protected Vector3 startingPosition;
		protected bool isStartingPositionInitialized = false;

		/// <summary>
		/// The bones that should be moved to accomplish the aiming
		/// </summary>
		[Tooltip("The bones that should be moved to accomplish the aiming")]
		[SerializeField] protected HumanBone[] humanBones;

		/// <summary>
		/// Array of the transforms of the bones that should be moved
		/// </summary>
		protected Transform[] boneTransforms;

		/// <summary>
		/// The direction of the aimTransform that should aim at the target
		/// </summary>
		public enum AimDirection { Y, X, Z };


		/// <summary>
		/// <see langword="true"/> if the component should destroy itself, when the aiming stops and the aim is back at the starting position
		/// </summary>
		public bool ShouldDestroyItself { get; set; } = true;

		/// <summary>
		/// The speed at which the agent looks at the target
		/// </summary>
		[Tooltip("The speed at which the agent looks at the target")]
		[field: SerializeField]
		public float LookSpeed { get; set; } = 2f;

		/// <summary>
		/// The weight of the aiming
		/// </summary>
		[Tooltip("The weight of the aiming")]
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
			ShouldDestroyItself = shouldDestroyItself;
			SetTargetTransform(target);
		}

		/// <summary>
		/// Removes the targetTransform, which results in the aim to return to the starting position, if shouldDestroyItself is set to true, the component will be destroyed after the aim is back at the starting position
		/// </summary>
		public void Stop()
		{
			targetTransform = null;
		}

		// LateUpdate is called once per frame, after Update
		protected void LateUpdate()
		{
			TemporarilyIncreaseLookSpeed(navMeshAgent.velocity.magnitude);

			if (isTargetFollowerInitialized)
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
			Vector3 targetDirection = targetFollower - aimTransform.position;
			Vector3 aimDirectionVector = GetAimDirectionVector();
			float blendOut = 0.0f;
			float targetAngle = Vector3.Angle(targetDirection, aimDirectionVector);
			if (targetAngle > angleLimit)
			{
				blendOut += (targetAngle - angleLimit) / 50.0f;
			}

			float targetDistance = targetDirection.magnitude;
			if (targetDistance < distanceLimit)
			{
				blendOut += distanceLimit - targetDistance;
			}


			Vector3 direction = Vector3.Slerp(targetDirection, aimDirectionVector, blendOut);
			return aimTransform.position + direction;
		}
		
		protected void UpdateTargetFollower()
		{
			Vector3 targetPosition;

			// If targetTransform was not removed in Stop()
			if (targetTransform)
			{
				targetPosition = targetTransform.position;
				increaseLookSpeedBy = 1;
			}
			else
			{
				// Target position stays where the agent was last looking at
				targetPosition = targetFollower;
				
				// Fade out IK weight
				Weight = Mathf.Lerp(Weight, 0f, Time.deltaTime * 3f);
				targetPosition = aimTransform.position + (GetAimDirectionVector() * 1f);
				if (Weight < 0.001f)
				{
					Weight = 0f;
					
					if (ShouldDestroyItself)
					{
						Destroy(this);
					}
					else
					{
						// Reset target position and target follower
						targetPosition = transform.TransformPoint(startingPosition);
						targetFollower = targetPosition;
					}
					
				}
			}

			// Smooth transition to target position
			targetFollower = Vector3.Lerp(targetFollower, targetPosition, Time.deltaTime * (currentLookSpeed * increaseLookSpeedBy));
		}


		protected void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
		{
			Vector3 aimDirectionVector = GetAimDirectionVector();
			Vector3 targetDirection = targetPosition - aimTransform.position;
			Quaternion aimTowards = Quaternion.FromToRotation(aimDirectionVector, targetDirection);
			Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
			bone.rotation = blendedRotation * bone.rotation;
		}

		protected Vector3 GetAimDirectionVector()
		{
			if (aimDirection == AimDirection.Y)
				return aimTransform.up.normalized;
			if (aimDirection == AimDirection.X)
				return aimTransform.right.normalized;
			if (aimDirection == AimDirection.Z)
				return aimTransform.forward;

			return aimTransform.up.normalized;
		}

		public void SetTargetTransform(Transform targetTransform)
		{
			if (!isTargetFollowerInitialized || !isStartingPositionInitialized)
			{
				// Set starting position of targetFollower 1 unit along the current aiming direction getAimDirectionVector() * 1f
				startingPosition = transform.InverseTransformPoint(aimTransform.position + (GetAimDirectionVector() * 1f));
				targetFollower = transform.TransformPoint(startingPosition);
				isStartingPositionInitialized = true;
				isTargetFollowerInitialized = true;
			}

			this.targetTransform = targetTransform;
		}
		public void TemporarilyIncreaseLookSpeed(float increase)
		{
			currentLookSpeed = LookSpeed + increase;
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
		public abstract void SetBonePreset();


		protected void GetBoneTransformsFromAnimator(HumanBodyBones aimingTip)
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
			if (isStartingPositionInitialized && aimTransform != null)
			{
				Gizmos.DrawWireSphere(transform.TransformPoint(startingPosition), 0.25f);
				Gizmos.DrawLine(aimTransform.position, transform.TransformPoint(startingPosition));
			}
			Gizmos.color = Color.red;
			if (isTargetFollowerInitialized)
			{
				Gizmos.DrawWireSphere(targetFollower, 0.25f);
			}
		}
	}
}