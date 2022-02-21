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
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value</param>
        public void ScheduleTask(IAgentTask task, int priority)
        {
            taskManager.ScheduleTask(task, priority);
        }
    }
}
