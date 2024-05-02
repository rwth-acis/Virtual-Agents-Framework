using System;
using System.Collections;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents
{
    /// <summary>
    /// A task which consists of multiple subtasks.
    /// It allows for checking of preconditions and then executing a sequence of tasks.
    /// </summary>
    public class TaskBundle : AgentBaseTask
    {
        /// <summary>
        /// Creates an empty TaskBundle
        /// </summary>
        public TaskBundle()
        {
            State = TaskState.Waiting;
            TaskQueue = new List<AgentBaseTask>();
            Preconditions = new List<Func<bool>>();
        }

        /// <summary>
        /// Creates a TaskBundle with a list of subtasks
        /// </summary>
        /// <param name="tasks"></param>
        public TaskBundle(List<AgentBaseTask> tasks)
        {
            State = TaskState.Waiting;
            TaskQueue = tasks;
            Preconditions = new List<Func<bool>>();
        }

        /// <summary>
        /// Creates a TaskBundle with a list of tasks and a list of preconditions
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="preconditions"></param>
        public TaskBundle(List<AgentBaseTask> tasks, List<Func <bool>> preconditions)
        {
            State = TaskState.Waiting;
            TaskQueue = tasks;
            Preconditions = preconditions;
        }

        /// <summary>
        /// List of tasks to be part of the bundle
        /// </summary>
        private List<AgentBaseTask> TaskQueue { get; set; }

        /// <summary>
        /// List of conditions to be met before execution of tasks
        /// </summary>
        private List<Func<bool>> Preconditions { get; set; }

        /// <summary>
        /// Check for preconditions and start the execution of all subtasks in sequence
        /// </summary>
        /// <param name="executingAgent"></param>
        public override void StartExecution(Agent executingAgent)
        {
            State = TaskState.Running;
            if (CheckPreconditions())
            {
                executingAgent.StartCoroutine(ExecuteTasks(executingAgent));
            }
        }

        /// <summary>
        /// Execute all tasks in the task queue. If a task fails, the whole bundle fails. Note, that checking of preconditions is not part of this method.
        /// </summary>
        /// <param name="executingAgent"></param>
        private IEnumerator ExecuteTasks(Agent executingAgent)
        {
            // Iterate over TaskQueue
            for (var i = 0; i < TaskQueue.Count; i++)
            {
                var task = TaskQueue[i];
                if (i > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Each task depends on all previous tasks
                        task.DependsOnTasks.Add(TaskQueue[j]);
                    }
                }
                // Start the task
                task.StartExecution(executingAgent);
                // Mini scheduler to wait for the current task to initialize
                while (task.State != TaskState.Running && task.State != TaskState.Success && task.State != TaskState.Failure)
                {
                    task.Tick(executingAgent);
                    yield return null; // wait for the next frame
                }
                // Wait for the current task to finish
                while (task.State == TaskState.Running)
                {
                    task.Tick(executingAgent);
                    yield return null; // wait for the next frame
                }

                if (task.State == TaskState.Failure)
                {
                    Debug.LogError("Task " + i + " failed");
                    StopAsFailed();
                    yield break;
                }
            }
            StopAsSucceeded();
            yield return null;
        }

        /// <summary>
        /// Check if all preconditions are met.
        /// </summary>
        /// <returns> True if all preconditions evaluate to true, otherwise false. </returns>
        private bool CheckPreconditions()
        {
            bool res = true;
            foreach (Func<bool> condition in Preconditions)
            {
                res = res && condition();
            }
            return res;
        }
    }
}