using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.TaskSystem.AgentTasks;
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
        private AgentTaskManager taskManager;

        /// <summary>
        /// List of shortcut methods to add common tasks to the agent's task queue
        /// Syntactic sugar. It is also possible to directly enqueue task objects on the agent instead, e.g. for custom tasks
        /// </summary>
        public TaskActions Tasks { get; private set; }

        /// <summary>
        /// Initialize the agent
        /// </summary>
        private void Awake()
        {
            taskManager = new AgentTaskManager(this);
            Tasks = new TaskActions(this);
        }

        /// <summary>
        /// Enable the right mode depending on the agent's status
        /// </summary>
        private void Update()
        {
            taskManager.Update();
        }

        /// <summary>
        /// Schedule a task
        /// </summary>
        /// <param name="task">Task to be scheduled</param>
        public void ScheduleTask(IAgentTask task)
        {
            taskManager.ScheduleTask(task);
        }

        /// <summary>
        /// Execute a task as soon as possible
        /// </summary>
        /// <param name="task">Task to be executed</param>
        public void ExecuteTaskASAP(IAgentTask task)
        {
            taskManager.ForceTask(task);
        }

        /// <summary>
        /// Helper function for shortcut queue management functions.
        /// Schedule a task or force its execution depending on the flag
        /// </summary>
        /// <param name="task">Task to be scheduled or forced</param>
        /// <param name="force">Flag: true if the task's execution should be forced, false if the task should be scheduled</param>
        public void ScheduleOrForce(IAgentTask task, bool force)
        {
            if (force == true)
            {
                taskManager.ForceTask(task);
            }
            else
            {
                taskManager.ScheduleTask(task);
            }
        }
    }
}
