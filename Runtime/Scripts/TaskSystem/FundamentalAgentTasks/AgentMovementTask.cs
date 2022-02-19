using i5.Toolkit.Core.Utilities;
using System;
using UnityEngine;
using UnityEngine.AI;
using VirtualAgentsFramework.TaskSystem;

namespace VirtualAgentsFramework
{
    namespace AgentTasks
    {
        /// <summary>
        /// Defines movement tasks for walking and running
        /// Uses the NavMeshAgent component
        /// </summary>
        public class AgentMovementTask : IAgentTask
        {
            protected NavMeshAgent navMeshAgent;

            private const float minDistance = 0.01f;

            /// <summary>
            /// Destination coordinates of the movement task
            /// </summary>
            public Vector3 Destination { get; protected set; }

            /// <summary>
            /// The target movement speed of the agent
            /// If negative, the default value set in the NavMeshAgent is taken
            /// </summary>
            public float TargetSpeed { get; protected set; }

            public event Action OnTaskFinished;

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
            /// Create an AgentMovementTask using a destination Transform
            /// </summary>
            /// <param name="destinationTransform">The transform to which the agent should walk</param>
            /// <param name="targetSpeed">The target speed of the agent, e.g. to set running or walking; if not set, the default value in the NavMeshAgent is taken</param>
            public AgentMovementTask(Transform destinationTransform, float targetSpeed = -1)
                : this(destinationTransform.position, targetSpeed)
            {
            }

            /// <summary>
            /// Starts the movement task
            /// </summary>
            /// <param name="agent">The agent which should execute the movement task</param>
            public void Execute(Agent agent)
            {
                navMeshAgent = agent.GetComponent<NavMeshAgent>();

                // only proceed on agents with a NavMeshAgent
                if (navMeshAgent == null)
                {
                    i5Debug.LogError($"The agent {agent.name} does not have a NavMeshAgent component. " +
                        $"Therefore, it cannot move. Skipping this task.",
                        this);

                    OnTaskFinished?.Invoke();

                    return;
                }

                StartMovement();
            }

            /// <summary>
            /// Update the animations as long as the agent still moves.
            /// If the NavMeshAgent did not move this agent for standingFrames frames, the task is finished
            /// </summary>
            public void Update()
            {
                if (navMeshAgent.remainingDistance < minDistance)
                {
                    StopMovement();
                }
            }

            private void StartMovement()
            {
                navMeshAgent.SetDestination(Destination);
                if (TargetSpeed > 0)
                {
                    navMeshAgent.speed = TargetSpeed;
                }
                navMeshAgent.enabled = true;
                navMeshAgent.updatePosition = true;
                navMeshAgent.updateRotation = true;
            }

            private void StopMovement()
            {
                OnTaskFinished?.Invoke();
            }
        }
    }
}
