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

        /// <summary>
        /// Aborts the current task on the specified layer
        /// </summary>
        /// <param name="layer">Name of the layer on which the task should be aborted</param>
        public void Abort(string layer = "Base Layer")
        {
            taskManagers[layer].Abort();
        }

        /// <summary>
        /// Aborts the current tasks on all layers
        /// </summary>
        public void AbortAllLayers()
        {
            foreach (var layer in taskManagers)
            {
                taskManagers[layer.Key].Abort();
            }
        }

        /// <summary>
        /// Removes a given task from the TaskSystem
        /// </summary>
        /// <param name="task">The task to be removed</param>
        /// <param name="layer">The layer on which the given task resides</param>
        public void RemoveTask(IAgentTask task, string layer = "Base Layer")
        {
            Debug.Log("Remove Task in ScheduleBasedTaskSystem");
            taskManagers[layer].RemoveTask(task);
        }

        /// <summary>
        /// Clears all tasks from the given layer
        /// </summary>
        /// <param name="layer">The layer whose tasks should be cleared, leave empty to clear base layer.</param>
        /// <param name="clearCurrentTask">If true, the current tasks gets aborted and removed as well, otherwise it can still finish.
        /// By default set to true</param>
        public void Clear(string layer = "Base Layer", bool clearCurrentTask = true)
        {
            taskManagers[layer].Clear(clearCurrentTask);
        }

        /// <summary>
        /// Clears all tasks from all layers
        /// </summary>
        /// <param name="clearCurrentTask">If true, the current tasks gets aborted and removed as well, otherwise it can still finish.
        /// By default set to true</param>
        public void ClearAllLayers(bool clearCurrentTask = true)
        {
            foreach (var layer in taskManagers)
            {
                taskManagers[layer.Key].Clear(clearCurrentTask);
            }
        }
    }
}
