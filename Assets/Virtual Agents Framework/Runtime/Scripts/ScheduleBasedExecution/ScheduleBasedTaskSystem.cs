using i5.VirtualAgents.ScheduleBasedExecution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.ScheduleBasedExecution
{
    /// <summary>
    /// Executes tasks by scheduling them in a priority queue
    /// </summary>
    public class ScheduleBasedTaskSystem : TaskSystem
    {
        /// <summary>
        /// List of shortcut methods to add common tasks to the agent's task queue
        /// Syntactic sugar. It is also possible to directly enqueue task objects on the agent instead, e.g. for custom tasks
        /// </summary>
        public TaskActions Tasks { get; private set; }

        // One task manager for every animation layer of the corresponding animator is generated
        private Dictionary<string, AgentTaskManager> taskManagers;

        private void Awake()
        {
            Tasks = new TaskActions(this);
            taskManagers = new Dictionary<string, AgentTaskManager>();
            Animator animator = GetComponent<Animator>();
            Agent agent = GetComponent<Agent>();
            // Create a task manager for each animation layer
            for (int i = 0; i < animator.layerCount; i++)
            {
                taskManagers.Add(animator.GetLayerName(i), new AgentTaskManager(agent));
            }
        }

        /// <summary>
        /// Get the agent component of the game object
        /// </summary>
        /// <returns>The agent component</returns>
        public Agent GetAgent()
        {
            return GetComponent<Agent>();
        }

        public override void UpdateTaskSystem()
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
    }
}
