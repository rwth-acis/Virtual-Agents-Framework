using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace i5.VirtualAgents
{
	/// <summary>
	/// Defines a gaze behaviour that looks at gaze targets in the scene dynamiycally
	/// </summary>
	public class AdaptiveGaze : MonoBehaviour
	{
		private class AdaptiveGazeTargetInfo
		{
			public AdaptiveGazeTarget lookAtTarget = null;
			public float distance = 0;
			public float importance = 0;
			public float timeLookedAt = 0;
			public float novelty = 0;
			public float calcValueOfInterest = 0;
			public bool isCurrentlyNearby = false;
		}

		[SerializeField] private float detectionRadius = 10f;
		[SerializeField] private int maxNumberOfTargetsInRange = 50;

		[SerializeField] private float detectionIntervalWhenIdle = 3f;
		[SerializeField] private float detectionIntervalWhenWalking = 0.5f;
		private float detectionInterval = 3f;



		// Define the chances for target selection, e.g. chance for the second most interesting target in the list to be looked at
		[Range(0f, 1f)]
		[SerializeField] private float chanceHighestRankedTarget = 0.5f;
		[Range(0f, 1f)]
		[SerializeField] private float chanceSecondHighestTarget = 0.1f;
		[Range(0f, 1f)]
		[SerializeField] private float chanceThirdHighestTarget = 0.1f;
		[Range(0f, 1f)]
		[SerializeField] private float chanceRandomTarget = 0.05f;
		[Range(0f, 1f)]
		[SerializeField] private float chanceIdleTarget = 0.25f;

		[SerializeField] private float lookSpeed = 2f;

		AdaptiveGazeTargetInfo currentTargetOfInterest;

		[SerializeField] private LayerMask seeLayers;
		[SerializeField] private LayerMask occlusionLayers = default;

		private List<AdaptiveGazeTargetInfo> nearbyLookAtTargets = new List<AdaptiveGazeTargetInfo>();
		private float timer = 0f;
		[Range(0f, 1f)]
		private float maxWeight = 0.8f;

		private AimAt aimScript;

		// initialization of the script
		private void Start()
		{
			aimScript = this.gameObject.AddComponent<AimAt>();

			aimScript.SetBonePreset("Head");
			aimScript.ShouldDestroyItself = false;
			aimScript.LookSpeed = lookSpeed;

			// Normalize chances
			float sum = chanceHighestRankedTarget + chanceSecondHighestTarget + chanceThirdHighestTarget + chanceRandomTarget + chanceIdleTarget;
			chanceHighestRankedTarget /= sum;
			chanceSecondHighestTarget /= sum;
			chanceThirdHighestTarget /= sum;
			chanceRandomTarget /= sum;
			chanceIdleTarget /= sum;
			chanceSecondHighestTarget += chanceHighestRankedTarget;
			chanceThirdHighestTarget += chanceSecondHighestTarget;
			chanceRandomTarget += chanceThirdHighestTarget;
			chanceIdleTarget += chanceRandomTarget;
			if (chanceIdleTarget != 1f)
			{
				Debug.LogWarning("Normalisation of gaze chances went wrong");
			}
		}

		public void Activate()
		{
			if (aimScript != null)
				aimScript.enabled = true;

			this.enabled = true;
		}

		public void Deactivate()
		{
			if (aimScript != null)
				aimScript.enabled = false;

			this.enabled = false;
		}

		// If changes are made to the lookSpeed in the inspector, update the aim script
		private void OnValidate()
		{
			if (aimScript != null)
				aimScript.LookSpeed = lookSpeed;
		}

		private void Update()
		{
			// Check if the agent is walking
			AdjustIntervalBasedOnWalkingSpeed();

			// Every second check which targets are nearby and invoke the function to calculate the most interesting target
			CheckWhichTargetsAreNearbyAndVisible();

			// Position of the currently looked at target is updated every frame in case it moves 
			UpdatePositionOfTarget();
		}

		private void AdjustIntervalBasedOnWalkingSpeed()
		{
			if (GetComponent<UnityEngine.AI.NavMeshAgent>()?.velocity.magnitude > 0.1f)
			{
				detectionInterval = detectionIntervalWhenWalking;
			}
			else
			{
				detectionInterval = detectionIntervalWhenIdle;
			}
		}

		private void UpdatePositionOfTarget()
		{
			if (currentTargetOfInterest != null)
			{
				aimScript.SetTargetTransform(currentTargetOfInterest.lookAtTarget.transform);
			}
			else
			{
				aimScript.Stop();
			}
		}

		private void CheckWhichTargetsAreNearbyAndVisible()
		{
			timer += Time.deltaTime;
			if (timer < detectionInterval)
			{
				return;
			}
			timer = 0f;

			// Old nearby targets are marked as not nearby
			foreach (AdaptiveGazeTargetInfo targetInfo in nearbyLookAtTargets)
			{
				targetInfo.isCurrentlyNearby = false;
			}


			// Check for nearby targets
			Collider[] colliders = new Collider[maxNumberOfTargetsInRange];
			// center is calculated so that corner of the bounding cube is at the position of the agent
			Vector3 center = transform.position + transform.forward * Mathf.Sqrt(2 * detectionRadius * detectionRadius);

			Vector3 halfExtents = new Vector3(detectionRadius, 2, detectionRadius);
			Quaternion rotation = transform.rotation * Quaternion.Euler(0, 45, 0);

			int count = Physics.OverlapBoxNonAlloc(center, halfExtents, colliders, rotation, seeLayers);
			// Use Window > Analysis > Physics Debug > Queries to see the detection radius, decrease detection Interval to see the cube on every frame
			if (count == maxNumberOfTargetsInRange)
			{
				Debug.LogWarning("Max number of targets in range reached. Increase the max number of targets in range or decrease the detection radius");
			}

			for (int i = 0; i < count; i++)
			{
				AdaptiveGazeTarget target = colliders[i].GetComponent<AdaptiveGazeTarget>();
				// Check that the object has an PossibleLookAtTarget component and that it is not picked up
				if (target == null || !target.canCurrentlyBeLookedAt)
				{
					continue;
				}
				// Check if the target is in sight
				if (!IsInSight(target.gameObject))
				{
					continue;
				}
				// If the target is not in the array yet, add it
				if (nearbyLookAtTargets.Find(x => x.lookAtTarget == target) == null)
				{
					AdaptiveGazeTargetInfo targetInfo = new AdaptiveGazeTargetInfo
					{
						lookAtTarget = target,
						distance = Vector3.Distance(transform.position, target.transform.position),
						importance = target.importance,
						timeLookedAt = 0f,
						novelty = 0f,
						calcValueOfInterest = 0f,
						isCurrentlyNearby = true
					};
					nearbyLookAtTargets.Add(targetInfo);
				}
				else
				{
					// If the target is already in the array, update info
					AdaptiveGazeTargetInfo targetInfo = nearbyLookAtTargets.Find(x => x.lookAtTarget == target);
					targetInfo.distance = Vector3.Distance(transform.position, target.transform.position);

					// If importance of the target increased, reset time looked at
					if (targetInfo.importance < target.importance)
					{
						targetInfo.novelty += (10 / detectionInterval);
					}
					targetInfo.importance = target.importance;
					targetInfo.isCurrentlyNearby = true;
					// Decrease time looked at by the detection interval
					targetInfo.timeLookedAt = Mathf.Max(0f, targetInfo.timeLookedAt - detectionInterval);
				}
			}

			// Remove targets that are not in the detection radius anymore
			foreach (AdaptiveGazeTargetInfo targetInfo in nearbyLookAtTargets.ToList())
			{
				if (targetInfo.isCurrentlyNearby == false)
				{
					nearbyLookAtTargets.Remove(targetInfo);
				}
			}
			// Calculate the most interesting target and select one by chance from the list
			CalculateInterestInTargetAndSelectOne();
		}


		public void CalculateInterestInTargetAndSelectOne()
		{
			foreach (AdaptiveGazeTargetInfo targetInfo in nearbyLookAtTargets)
			{
				targetInfo.calcValueOfInterest = targetInfo.importance - targetInfo.distance - targetInfo.timeLookedAt + targetInfo.novelty;
			}
			nearbyLookAtTargets.Sort((x, y) => y.calcValueOfInterest.CompareTo(x.calcValueOfInterest));

			AdaptiveGazeTargetInfo newTargetOfInterest = SelectFromListWithProbability();

			if (newTargetOfInterest != null)
			{
				aimScript.Weight = maxWeight;
				// Increase time looked at by the detection interval
				newTargetOfInterest.timeLookedAt += detectionInterval * 2;
				// Decrease novelty over time
				newTargetOfInterest.novelty = Mathf.Max(0f, newTargetOfInterest.novelty - (1 / detectionInterval));
				// Increase novalty if the target changed
				if (currentTargetOfInterest != newTargetOfInterest)
				{
					newTargetOfInterest.novelty += (5 / detectionInterval);
				}
			}

			currentTargetOfInterest = newTargetOfInterest;
		}

		public bool IsInSight(GameObject obj)
		{
			Vector3 origin = transform.position + transform.forward;
			origin.y += 1.5f;
			Vector3 dest = obj.transform.position;
			if (Physics.Linecast(origin, dest, occlusionLayers))
			{
				// Debug.DrawLine(origin, dest, Color.red, 2f);
				return false;
			}
			// Debug.DrawLine(origin, dest, Color.green, 2f);
			return true;
		}

		private AdaptiveGazeTargetInfo SelectFromListWithProbability()
		{
			if (nearbyLookAtTargets.Count == 0)
			{
				// No objects available
				return null;
			}
			else
			{
				double randomValue = Random.value;

				if (randomValue <= chanceHighestRankedTarget)
				{
					// Select the first target
					return nearbyLookAtTargets[0];
				}
				else if (chanceHighestRankedTarget < randomValue && randomValue <= chanceSecondHighestTarget)
				{
					// Select the second target or first target when second target is not available
					if (nearbyLookAtTargets.Count > 1)
						return nearbyLookAtTargets[1];
					else
						return nearbyLookAtTargets[0];
				}
				else if (chanceSecondHighestTarget < randomValue && randomValue <= chanceThirdHighestTarget)
				{
					// Select the third target or first target when second target is not available
					if (nearbyLookAtTargets.Count > 2)
						return nearbyLookAtTargets[2];
					else
						return nearbyLookAtTargets[0];
				}
				else if (chanceThirdHighestTarget < randomValue && randomValue <= chanceRandomTarget)
				{
					// Select a random target
					int randomIndex = Random.Range(0, nearbyLookAtTargets.Count);
					return nearbyLookAtTargets[randomIndex];
				}
				else if (chanceRandomTarget < randomValue && randomValue <= chanceIdleTarget)
				{
					// Select no target and idle
					return null;
				}
				return null;
			}
		}
	}
}
