using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Agent's functionality mainly includes managing their task queue,
    /// responding to task execution statuses and changing one's state accordingly
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))] // Responsible for the proxy object's movement
    [RequireComponent(typeof(AgentAnimationUpdater))] // Responsible for the avatar's movement
    public class Agent : MonoBehaviour
    {
        public ITaskSystem TaskSystem { get; private set; }

        /// <summary>
        /// The animator component which controls the agent's animations
        /// </summary>
        public Animator Animator { get; private set; }

        /// <summary>
        /// Initialize the agent
        /// </summary>
        private void Awake()
        {
            Animator = GetComponent<Animator>();
            TaskSystem = GetComponent<TaskSystem.TaskSystem>();
        }

        /// <summary>
        /// Update the task system
        /// </summary>
        private void Update()
        {
            TaskSystem.UpdateTaskSystem();
        }
    }
}
