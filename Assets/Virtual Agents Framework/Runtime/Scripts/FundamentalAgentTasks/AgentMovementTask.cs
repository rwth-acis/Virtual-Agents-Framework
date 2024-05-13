using i5.Toolkit.Core.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace i5.VirtualAgents.AgentTasks
{
	/// <summary>
	/// Defines movement tasks for walking and running
	/// Uses the NavMeshAgent component
	/// </summary>
	public class AgentMovementTask : AgentBaseTask, ISerializable
	{
		/// <summary>
		/// Reference to the NavMeshAgent component
		/// </summary>
		protected NavMeshAgent navMeshAgent;

		/// <summary>
		/// Minimum distance of the agent to the target so that the task is considered finished
		/// </summary>
		public float MinDistance { get; set; } = 0.01f;

		/// <summary>
		/// Number of seconds spent on current path
		/// </summary>
		private float timeOnCurrentPath = 0;

		/// <summary>
		/// Determines if the agent should follow the DestinationObject automatically, even when path is noncomplete 
		/// </summary>
		private bool followGameObject;

		/// <summary>
		/// Destination coordinates of the movement task
		/// </summary>
		public Vector3 Destination { get; protected set; }

		/// <summary>
		/// Destination Object of the movement task
		/// </summary>
		public GameObject DestinationObject { get; protected set; }

		/// <summary>
		/// The target movement speed of the agent
		/// If negative, the default value set in the NavMeshAgent is taken
		/// </summary>
		public float TargetSpeed { get; protected set; }

		/// <summary>
		/// Number of seconds after which the path will be recalculated
		/// </summary>
		public float PathUpdateInterval { get; set; } = 1f;

		public AgentMovementTask()
		{
			TargetSpeed = -1;
		}

		/// <summary>
		/// Create an AgentMovementTask using destination coordinates
		/// </summary>
		/// <param name="destinationCoordinates">The position to which the agent should move</param>
		/// <param name="targetSpeed">The target speed of the agent, e.g. to set running or walking; if not set, the default value in the NavMeshAgent is taken</param>
		public AgentMovementTask(Vector3 destinationCoordinates, float targetSpeed = -1)
		{
			Destination = destinationCoordinates;
			TargetSpeed = targetSpeed;
			followGameObject = false;
		}
        /// <summary>
        /// Create an AgentMovementTask using a destination object
        /// </summary>
        /// <param name="destinationObject">The object that the agent should move to or follow</param>
        /// <param name="targetSpeed">The target speed of the agent, e.g. to set running or walking; if not set, the default value in the NavMeshAgent is taken</param>
		/// <param name="followGameObject">Determines if the agent should follow the DestinationObject automatically, even when path is noncomplete</param>
        public AgentMovementTask(GameObject destinationObject, float targetSpeed = -1, bool followGameObject = false)
		{
			DestinationObject = destinationObject;
			TargetSpeed = targetSpeed;
			this.followGameObject = followGameObject;
		}

		/// <summary>
		/// Starts the movement task
		/// </summary>
		/// <param name="agent">The agent which should execute the movement task</param>
		public override void StartExecution(Agent agent)
		{
			base.StartExecution(agent);
			navMeshAgent = agent.GetComponent<NavMeshAgent>();

			// only proceed on agents with a NavMeshAgent
			if (navMeshAgent == null)
			{
				i5Debug.LogError($"The agent {agent.name} does not have a NavMeshAgent component. " +
					$"Therefore, it cannot move. Skipping this task.",
					this);
                FinishTaskAsFailed();
				return;
			}

			StartMovement();
		}

		/// <summary>
		/// Checks every frame whether the agent has reached the target
		/// </summary>
		public override TaskState EvaluateTaskState()
		{
			// we only need to recalculate the path if we are following a GameObject
			if (followGameObject)
			{
				if (timeOnCurrentPath >= PathUpdateInterval)
				{
					UpdateMovement(); //calculates new path if object is not stationary and follow is activated
					timeOnCurrentPath = 0;
				}
				else
				{
					timeOnCurrentPath += Time.deltaTime;
				}
			}

			if (navMeshAgent == null)
				return TaskState.Failure; // No navmesh agent attached
			if (navMeshAgent.pathPending)
				return TaskState.Running; // The navmesh agent is still generating the path, try again on next update
			if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
				return TaskState.Failure; // The navmesh agent couldn't generate a complete and valid path
										  // If the destination is at a stationary location, a partial path will also result in failure. Otherwise the agent will walk as closly to the GameObject as possible
			if (DestinationObject == null || !followGameObject)
			{
				if (navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
				{
					Debug.LogWarning("Path calculation failed because only a partial path could be generated. Use an DestinationObject instead of Destination coordinates and activate follow to allow partial paths.");
					return TaskState.Failure; // The navmesh agent couldn't generate a complete and valid path
				}
			}
			if (navMeshAgent.remainingDistance < MinDistance)
			{
				return TaskState.Success;
			}
			// The agent moves on a valid path and hasn't reached its destination yet. Give all control about movement und rotation to the navmesh agent
            navMeshAgent.isStopped = false;
			return TaskState.Running;


		}

		// Sets the destination on the NavMesh and lets the agent walk on the NavMesh
		private void StartMovement()
		{
			// Give all control about the movement to the navmesh agent
			navMeshAgent.enabled = true;
			navMeshAgent.updatePosition = true;
			navMeshAgent.updateRotation = true;
			if (!navMeshAgent.SetDestination(DestinationObject != null ? DestinationObject.transform.position : Destination))
			{
				FinishTaskAsFailed();
				return;
			}
			navMeshAgent.isStopped = true;
			if (TargetSpeed > 0)
			{
				navMeshAgent.speed = TargetSpeed;
			}
		}

		// Update the destination to the current location of the destination GameObject 
		private void UpdateMovement()
		{
			if (followGameObject && DestinationObject != null)
			{
				// recalculate the path by restarting the movement from the current position
				StartMovement();
			}
		}

        /// <summary>
        /// Finish the task
        /// </summary>
        public override void StopExecution()
        {
            navMeshAgent.isStopped = true;
        }

		public void Serialize(SerializationDataContainer serializer)
		{
			serializer.AddSerializedData("Destination Object", DestinationObject);
			serializer.AddSerializedData("Destination", Destination);
			serializer.AddSerializedData("Target Speed", TargetSpeed);
            serializer.AddSerializedData("Follow GameObject?", followGameObject);

        }

		public void Deserialize(SerializationDataContainer serializer)
		{
            // Replace old names, to update old tree files
            serializer.Replace("TargetSpeed", "Target Speed");
            serializer.Replace("FollowGameObject", "Follow GameObject?");

			// Deserialize the data
            DestinationObject = serializer.GetSerializedGameobjects("Destination Object");
			Destination = serializer.GetSerializedVector("Destination");
			TargetSpeed = serializer.GetSerializedFloat("Target Speed");
            followGameObject = serializer.GetSerializedBool("Follow GameObject?");
		}
	}
}