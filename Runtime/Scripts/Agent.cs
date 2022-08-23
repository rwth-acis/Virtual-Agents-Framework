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
            TaskSystem = GetComponent<ScheduleBasedExecution.TaskSystem>();
            // Since there are multiple TaskSystems, enforcing one with RequireComponent is not advisable.
            // If the parent class TaskSystem is enforced, an agent will automatically get a component that implemenets no own functionallity, that can not easily be deleted and that can be confused with the actual task system.
            // If one of its implementations is enforced, it is harder to use one of the other implemetations.
            if (TaskSystem == null)
            {
                Debug.LogWarning("Agent has no TaskSystem attached. Attach a component that inherits the TaskSystem class.");
            }
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
