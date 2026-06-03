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
		[Tooltip("The transform that should be aimed at")]
		[SerializeField] protected Transform targetTransform;

		/// <summary>
		/// The Transform of the agent child objects that should directly aim at the target
		/// </summary>
		[Tooltip("The Transform of the agent child objects that should directly aim at the target")]
		[SerializeField] protected Transform aimTransform;

		/// <summary>
		/// Axis of the aimTransform that should aim at the target
		/// </summary>
		protected AimAxisAlignmentMode aimAxisAlignmentMode = AimAxisAlignmentMode.Forward;

		/// <summary>
		/// The Transform that is actually looked at and will follow the target smoothly
		/// </summary>
		protected Transform targetFollower;

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
		[Tooltip("The position where the targetFollower should be placed when no target is set")]
		[SerializeField] protected Transform startingTransform;

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
		/// Defines the strategy used to automatically calculate the correct local forward axis for an aiming bone.
		/// </summary>
		public enum AimAxisAlignmentMode { 
			/// <summary>
			/// Compares the bone's local axes against the character's root forward direction to find the best match. 
			/// Best suited for independent bones like the head or torso.
			/// </summary>
			Forward, 

			/// <summary>
			/// Compares the bone's local axes against the directional vector of the bone chain (e.g., from the parent bone to the aiming tip). 
			/// Best suited for connected extremities like arms or fingers.
			/// </summary>
			Chain
			
		};
		
		/// <summary>
		/// The cached local axis of the aimTransform that represents "forward". 
		/// Calculated once during setup.
		/// </summary>
		protected Vector3 localAimAxis = Vector3.forward;

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
		/// Setup the aiming script without starting it, e.g. for passive gazing
		/// </summary>
		/// <param name="shouldDestroyItself">If the component should destroy itself after aiming is stopped</param>
		/// <param name="lookSpeed">The speed at which the agent looks at the target</param>
		public void Setup(bool shouldDestroyItself = true, float lookSpeed = 2f)
		{
			SetBonePreset();
			localAimAxis = GetAimDirectionVector();
			this.ShouldDestroyItself = shouldDestroyItself;
			LookSpeed = lookSpeed;
		}

        /// <summary>
        /// Starts the aiming at the target with the given layer and target
        /// </summary>
        /// <param name="target">The transform of the object that should be aimed at</param>
        /// <param name="shouldDestroyItself">If the component should destroy itself after aiming is stopped</param>
        public void SetupAndStart(Transform target, bool shouldDestroyItself = true)
		{
			SetBonePreset();
			localAimAxis = GetAimDirectionVector();
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
			Vector3 aimDirection = GetCachedWorldAimDirection();
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
				// Return to the starting position
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
			Vector3 aimDirection = GetCachedWorldAimDirection();
			Vector3 targetDirection = targetPosition - aimTransform.position;
			Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
			Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
			bone.rotation = blendedRotation * bone.rotation;
		}

		/// <summary>
		/// Calculates the correct aiming vector for this specific bone based on the selected <see cref="AimAxisAlignmentMode"/>.
		/// It calculates the closest local axis by comparing the bone's orientation against either the character's root 
		/// forward direction or the directional vector of the bone chain, depending on the selected mode.
		/// </summary>
		/// <returns>A normalized local-space vector representing the bone's primary aiming axis.</returns>
		protected Vector3 GetAimDirectionVector()
		{
			// Strategy 1: Align with the Agent's Forward Direction (Best for Heads)
			if (this.aimAxisAlignmentMode == AimAxisAlignmentMode.Forward)
			{
				// Pass the agent root's forward vector into our calculation
				return GetClosestAxis(transform.forward);
			}
    
			// Strategy 2: Align with the Bone Chain (Best for Limbs/Fingers)
			if (this.aimAxisAlignmentMode == AimAxisAlignmentMode.Chain)
			{
				Vector3 chainDirection = Vector3.zero;

				// Calculate direction from the last bone in the array to the aimTransform tip
				if (boneTransforms != null && boneTransforms.Length > 0)
				{
					Transform lastBone = boneTransforms[^1];
					chainDirection = aimTransform.position - lastBone.position;
				}
				// Fallback: If no bone chain array exists, use the direct parent
				else if (aimTransform.parent != null)
				{
					chainDirection = aimTransform.position - aimTransform.parent.position;
				}

				if (chainDirection != Vector3.zero)
				{
					return GetClosestAxis(chainDirection);
				}
			}
			Debug.LogWarning("AimAxisAlignmentMode is set to " + aimAxisAlignmentMode + " but no valid direction vector could be calculated. Defaulting to aimTransform.forward.");

			return Vector3.forward;
		}

		/// <summary>
		/// Evaluates all 6 primary local axes of the <see cref="aimTransform"/> and returns the one that most closely aligns 
		/// with the provided target direction.
		/// </summary>
		/// <param name="targetVector">The desired world-space direction the bone is attempting to point towards.</param>
		/// <returns>A normalized local vector representing the closest matching local axis.</returns>
		protected Vector3 GetClosestAxis(Vector3 targetVector)
		{
			if (aimTransform == null) return Vector3.up;
			
			Vector3 normalizedTarget = targetVector.normalized;
			
			Vector3 localTarget = aimTransform.InverseTransformDirection(normalizedTarget);
			
			Vector3[] localAxes =
			{
				Vector3.right,   // X
				Vector3.left,    // -X
				Vector3.up,      // Y
				Vector3.down,    // -Y
				Vector3.forward, // Z
				Vector3.back     // -Z
			};

			Vector3 bestLocalAxis = Vector3.up; // Fallback
			float maxDot = -Mathf.Infinity;

			// Find which local axis most closely aligns with the target direction
			foreach (Vector3 axis in localAxes)
			{
				float dot = Vector3.Dot(localTarget, axis);
				if (dot > maxDot)
				{
					maxDot = dot;
					bestLocalAxis = axis;
				}
			}
			
			return bestLocalAxis;
		}
		/// <summary>
		/// Converts cached aim direction to world space
		/// </summary>
		protected Vector3 GetCachedWorldAimDirection()
		{
			if (aimTransform == null) return Vector3.forward;
    
			// Converts the static local axis (e.g. up) into where it's pointing in the world right now
			return aimTransform.TransformDirection(localAimAxis);
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

				// Set starting position of targetFollower 1 unit along the current aiming direction GetCachedWorldAimDirection()() * 1f
				this.startingTransform = new GameObject().transform;
				this.startingTransform.gameObject.name = "StartingPosition";
				this.startingTransform.position = aimTransform.position + (GetCachedWorldAimDirection() * 1f);
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
		/// <param name="aimAxisAlignmentMode">The mode for aligning the aiming direction</param>
		/// <param name="aimTransform">The last point of the bones that should directly point at the target</param>
		/// <param name="angleLimit">The limit at which pointing will be stopped, i.e. 90f to only aim when target is somewhere in front of the agent</param>
		public void UseNewBoneset(HumanBone[] humanBones, AimAxisAlignmentMode aimAxisAlignmentMode, Transform aimTransform, float angleLimit)
		{
			this.humanBones = humanBones;
			this.aimAxisAlignmentMode = aimAxisAlignmentMode;
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