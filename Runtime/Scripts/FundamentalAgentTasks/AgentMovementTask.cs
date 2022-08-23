using i5.Toolkit.Core.Utilities;
using UnityEngine;
using UnityEngine.AI;

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
        private const float minDistance = 0.01f;

        /// <summary>
        /// Destination coordinates of the movement task
        /// </summary>
        public Vector3 Destination { get; protected set; }


        public GameObject DestinationObject;

        /// <summary>
        /// The target movement speed of the agent
        /// If negative, the default value set in the NavMeshAgent is taken
        /// </summary>
        public float TargetSpeed { get; protected set; }

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
        }

        /// <summary>
        /// Starts the movement task
        /// </summary>
        /// <param name="agent">The agent which should execute the movement task</param>
        public override void Execute(Agent agent)
        {
            base.Execute(agent);
            navMeshAgent = agent.GetComponent<NavMeshAgent>();

            // only proceed on agents with a NavMeshAgent
            if (navMeshAgent == null)
            {
                i5Debug.LogError($"The agent {agent.name} does not have a NavMeshAgent component. " +
                    $"Therefore, it cannot move. Skipping this task.",
                    this);

                FailTask();
                return;
            }

            StartMovement();
        }

        /// <summary>
        /// Checks every frame whether the agent has reached the target
        /// </summary>
        public override TaskState Update()
        {
            if (navMeshAgent == null)
                return TaskState.Failure; //No navmesh agent attached
            if (navMeshAgent.pathPending)
                return TaskState.Running; //The navmesh agent is still generating the path, try again on next update
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial || navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return TaskState.Failure; //The navmesh agent couldn't generate a complete and valid path
            if (navMeshAgent.remainingDistance < minDistance)
            {
                return TaskState.Success;
            }

            //The agent moves on a valid path and hasn't reached its destination yet. Give all control about movement und rotation to the navmesh agent
            navMeshAgent.isStopped = false;
            return TaskState.Running;
        }

        // sets the destionation on the NavMesh and lets the agent walk on the NavMesh
        private void StartMovement()
        {
            navMeshAgent.enabled = true;
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;
            if (!navMeshAgent.SetDestination(DestinationObject != null ? DestinationObject.transform.position : Destination))
            {
                FailTask();
                return;
            }
            navMeshAgent.isStopped = true;
            if (TargetSpeed > 0)
            {
                navMeshAgent.speed = TargetSpeed;
            }
        }

        /// <summary>
        /// Finish the task
        /// </summary>
        public override void Stop()
        {
            navMeshAgent.isStopped = true;
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Destination Object", DestinationObject);
            serializer.AddSerializedData("Destination",Destination);
            serializer.AddSerializedData("TargetSpeed",TargetSpeed);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            DestinationObject = serializer.GetSerializedGameobjects("Destination Object");
            Destination = serializer.GetSerializedVector("Destination");
            TargetSpeed = serializer.GetSerializedFloat("TargetSpeed");
        }
    }
}