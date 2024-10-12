using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Defines a gaze behaviour that looks at gaze targets in the scene dynamically
    /// </summary>
    public class AdaptiveGaze : MonoBehaviour
    {
        private class AdaptiveGazeTargetInfo
        {
            /// <summary>
            /// The gaze target that is looked at
            /// </summary>
            public AdaptiveGazeTarget lookAtTarget = null;

            /// <summary>
            /// Distance to the target
            /// </summary>
            public float distance = 0;

            /// <summary>
            /// The importance of the item for the agent. The higher the value, the more liekly it is the agent to look at it. Increases during runtime resets novelty for the agent
            /// </summary>
            public float importance = 0;

            /// <summary>
            /// The time the agent looked at the target
            /// </summary>
            public float timeLookedAt = 0;

            /// <summary>
            /// The novelty of the target
            /// </summary>
            public float novelty = 0;

            /// <summary>
            /// This value is calculated based on the importance, distance, time looked at and novelty
            /// </summary>
            public float calcValueOfInterest = 0;

            /// <summary>
            /// Specifies if the target is currently nearby
            /// </summary>
            public bool isCurrentlyNearby = false;
        }
        /// <summary>
        /// When this transform is set, the gaze target is overwritten and the agent looks at this transform constantly without interruption (within the boundaries specified by the AimAt Script, e.g. distanceLimit and angleLimit)
        /// </summary>
        [Tooltip("When this transform is set, the gaze target is overwritten and the agent looks at this transform constantly without interruption (within the boundaries specified by the AimAt Script, e.g. distanceLimit and angleLimit)")]
        public Transform OverwriteGazeTarget = null;

        /// <summary>
        /// The radius in which the agent looks for gaze targets
        /// </summary>
        [Tooltip("The radius in which the agent looks for gaze targets")]
        [SerializeField] private float detectionRadius = 10f;

        /// <summary>
        /// The maximum number of targets in range that are considered for the gaze
        /// </summary>
        [Tooltip("The maximum number of targets in range that are considered for the gaze")]
        [SerializeField] private int maxNumberOfTargetsInRange = 50;

        /// <summary>
        /// The interval in seconds in which the agent looks for new targets when not moving
        /// </summary>
        [Tooltip("The interval in seconds in which the agent looks for new targets when not moving")]
        [SerializeField] private float detectionIntervalWhenIdle = 3f;

        /// <summary>
        /// The interval in seconds in which the agent looks for new targets when moving
        /// </summary>
        [Tooltip("The interval in seconds in which the agent looks for new targets when moving")]
        [SerializeField] private float detectionIntervalWhenWalking = 0.5f;

        private float detectionInterval = 3f;

        // Define the chances for target selection, e.g. chance for the second most interesting target in the list to be looked at
        /// <summary>
        /// The chance that the agent looks at the highest ranked target based on the algorithm, see documentation
        /// </summary>
        [Tooltip("The chance that the agent looks at the highest ranked target based on the algorithm, see documentation")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceHighestRankedTarget = 0.5f;

        /// <summary>
        /// The chance that the agent looks at the second-highest ranked target based on the algorithm, see documentation
        /// </summary>
        [Tooltip("The chance that the agent looks at the second-highest ranked target based on the algorithm, see documentation")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceSecondHighestTarget = 0.1f;

        /// <summary>
        /// The chance that the agent looks at the third highest ranked target based on the algorithm, see documentation
        /// </summary>
        [Tooltip("The chance that the agent looks at the third highest ranked target based on the algorithm, see documentation")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceThirdHighestTarget = 0.1f;

        /// <summary>
        /// The chance that the agent looks at a random target
        /// </summary>
        [Tooltip("The chance that the agent looks at a random target")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceRandomTarget = 0.05f;

        /// <summary>
        /// The chance that the agent does not look at any target and looks ahead
        /// </summary>
        [Tooltip("The chance that the agent does not look at any target and looks ahead")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceIdleTarget = 0.25f;

        /// <summary>
        /// The speed at which the agent looks and switches between targets
        /// </summary>
        [Tooltip("The speed at which the agent looks and switches between targets")]
        [SerializeField] private float lookSpeed = 2f;

        private AdaptiveGazeTargetInfo currentTargetOfInterest;

        /// <summary>
        /// The object layers that are checked for gaze targets, default is everything for the sake of simplicity.
        /// It is recommended to set this to a more specific layer mask to improve performance.
        /// </summary>
        [Tooltip("The object layers that are checked for gaze targets. It is recommended to set this to a more specific layer mask to improve performance.")]
        [SerializeField] public LayerMask seeLayers = -1;

        /// <summary>	
        /// The layers that can block the view between a gaze target and the agent
        /// The default is the default layer mask
        /// </summary>
        [Tooltip("The layers that can block the view between a gaze target and the agent")]
        [SerializeField] public LayerMask occlusionLayers = 0;

        private List<AdaptiveGazeTargetInfo> nearbyLookAtTargets = new List<AdaptiveGazeTargetInfo>();
        private float timer = 0f;
        [Range(0f, 1f)]
        private float maxWeight = 0.8f;

        private AimAt aimScript;

        // initialization of the script
        private void Start()
        {
            // Check if the seeLayers are set to Everything (-1)
            if (seeLayers.value == -1)
            {
                // Check if scene is not one of the sample scenes
                if (!SceneManager.GetActiveScene().name.ToLower().Contains("sample"))
                {
                    Debug.LogWarning("The seeLayers of AdaptiveGaze component of the agent are still set to Everything. This might cause performance issues. Please set the seeLayers to a more specific layer mask or deactivate the AdaptiveGaze component. See AdaptiveGaze in the documentation manuel.");
                }
            }
            aimScript = this.gameObject.AddComponent<HeadPreset>();

            aimScript.SetBonePreset();
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
        /// <summary>
        /// This function has to be called to start the adaptive gaze, after has been stopped with the deactivate function
        /// </summary>
        public void Activate()
        {
            if (aimScript != null)
                aimScript.enabled = true;

            this.enabled = true;
        }

        /// <summary>
        /// This function has to be called to stop the adaptive gaze
        /// </summary>
        public void Deactivate()
        {
            if (aimScript != null)
                aimScript.Stop();

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
            if (OverwriteGazeTarget != null)
            {
                aimScript.SetTargetTransform(OverwriteGazeTarget);
            }
            else if (currentTargetOfInterest != null)
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

            // Remove targets that are no longer within the detection radius
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


        private void CalculateInterestInTargetAndSelectOne()
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
                // Increase novelty if the target changed
                if (currentTargetOfInterest != newTargetOfInterest)
                {
                    newTargetOfInterest.novelty += (5 / detectionInterval);
                }
            }

            currentTargetOfInterest = newTargetOfInterest;
        }

        private bool IsInSight(GameObject obj)
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
