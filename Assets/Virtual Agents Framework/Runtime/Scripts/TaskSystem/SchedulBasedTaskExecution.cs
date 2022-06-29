using i5.VirtualAgents.TaskSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public class SchedulBasedTaskExecution : ITaskSystem
    {
        public SchedulBasedTaskExecution(Agent agent)
        {
            taskManagers = new Dictionary<string, AgentTaskManager>();
            Animator animator = agent.GetComponent<Animator>();
            // Create a task manager for each animation layer
            for (int i = 0; i < animator.layerCount; i++)
            {
                taskManagers.Add(animator.GetLayerName(i), new AgentTaskManager(agent));
            }
        }

        // One task manager for every animation layer of the corresponding animator is generated
        private Dictionary<string, AgentTaskManager> taskManagers;

        public void Update()
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
