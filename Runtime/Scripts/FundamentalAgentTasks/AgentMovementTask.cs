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
        /// Amount of frames after which the path will be recalculated
        /// </summary>
        private const int pathUpdateAfterFrames = 10;
        /// <summary>
        /// Amount of frames that have been spent on current path
        /// </summary>
        private int frameOnCurrentPath = 0;

        /// <summary>
        /// Determines if the agent should follow the DestinationObject automaticlly, even when path is noncomplete 
        /// </summary>
        private bool follow;

        /// <summary>
        /// Destination coordinates of the movement task
        /// </summary>
        public Vector3 Destination { get; protected set; }


        public GameObject DestinationObject { get; protected set; }

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
            follow = false;
        }
        public AgentMovementTask(GameObject destinationObject, float targetSpeed = -1, bool zfollow = false)
        {
            DestinationObject = destinationObject;
            TargetSpeed = targetSpeed;
            follow = zfollow;
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
                State = TaskState.Failure;
                i5Debug.LogError($"The agent {agent.name} does not have a NavMeshAgent component. " +
                    $"Therefore, it cannot move. Skipping this task.",
                    this);

                FinishTask();
                return;
            }

            StartMovement();
        }

        /// <summary>
        /// Checks every frame whether the agent has reached the target
        /// </summary>
        public override TaskState EvaluateTaskState()
        {
            if(frameOnCurrentPath >= pathUpdateAfterFrames)
            {
                UpdateMovement(); //calculates new path
                frameOnCurrentPath = 0;
            }
            else
            {
                frameOnCurrentPath++;
            }

            if (navMeshAgent == null)
                return TaskState.Failure; //No navmesh agent attached
            if (navMeshAgent.pathPending)
                return TaskState.Running; //The navmesh agent is still generating the path, try again on next update
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                return TaskState.Failure; //The navmesh agent couldn't generate a complete and valid path
            //If the destination is at a stationary location, a partial Path will also result in failure. Otherwise the agent will walk as closly to the GameObject as possible
            if (DestinationObject == null || !follow)
            {
                Debug.LogWarning("Path calculation failed because only a partial path could be generated" + follow);
                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
                    return TaskState.Failure; //The navmesh agent couldn't generate a complete and valid path

            }
                
            if (navMeshAgent.remainingDistance < minDistance)
            {
                return TaskState.Success;
            }

            //The agent moves on a valid path and hasn't reached its destination yet
            return TaskState.Running;

            
        }

        // sets the destionation on the NavMesh and lets the agent walk on the NavMesh
        private void StartMovement()
        {
            //Give all control about the movement to the navmesh agent
            navMeshAgent.enabled = true;
            navMeshAgent.updatePosition = true;
            navMeshAgent.updateRotation = true;
            if (!navMeshAgent.SetDestination(DestinationObject != null ? DestinationObject.transform.position : Destination))
            {
                State = TaskState.Failure;
                return;
            }
            if (TargetSpeed > 0)
            {
                navMeshAgent.speed = TargetSpeed;
            }
        }

        //update the destination to the current location of the destination gameobject 
        private void UpdateMovement()
        {
            if(DestinationObject != null)
            {
                StartMovement();
            }
                
        }

        /// <summary>
        /// Finish the task
        /// </summary>
        public override void StopExecution()
        {
            navMeshAgent.enabled = false;
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