using i5.VirtualAgents.TaskSystem;
using System.Collections.Generic;
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
        //One task manager for every animation layer of the corresponding animator is generated
        private Dictionary<string, AgentTaskManager> taskManagers;

        private TaskSynchronizer taskSynchronizer;

        /// <summary>
        /// The animator component which controls the agent's animations
        /// </summary>
        public Animator Animator { get; private set; }

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
            Tasks = new TaskActions(this);
            Animator = GetComponent<Animator>();
            taskManagers = new Dictionary<string, AgentTaskManager>();
            // Create a task manager for each animation layer
            for (int i = 0; i < Animator.layerCount; i++)
            {
                taskManagers.Add(Animator.GetLayerName(i), new AgentTaskManager(this));
            }
            taskSynchronizer = new TaskSynchronizer(taskManagers);
        }

        /// <summary>
        /// Enable the right mode depending on the agent's status
        /// </summary>
        private void Update()
        {
            foreach (var taskManager in taskManagers.Values)
            {
                taskManager.Update();
            }
        }

        /// <summary>
        /// Schedule a task
        /// </summary>
        /// <param name="task">Task to be scheduled</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value</param>
        public void ScheduleTask(IAgentTask task, int priority = 0, string layer = "Base Layer")
        {
            taskManagers[layer].ScheduleTask(task, priority);
        }

        public void DefineTaskDependency(IAgentTask task, params IAgentTask[] dependencies)
        {
            taskSynchronizer.DefineTaskDependency(task, dependencies);
        }
    }
}
