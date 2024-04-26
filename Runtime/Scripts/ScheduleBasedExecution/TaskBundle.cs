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
        private MonoBehaviour coroutineHost;
        public TaskBundle(MonoBehaviour coroutineHost)
        {
            this.State = TaskState.Waiting;
            TaskQueue = new List<AgentBaseTask>();
            this.coroutineHost = coroutineHost;
        }

        public TaskBundle(MonoBehaviour coroutineHost, List<AgentBaseTask> tasks)
        {
            this.State = TaskState.Waiting;
            TaskQueue = tasks;
            this.coroutineHost = coroutineHost;
        }

        public TaskBundle(MonoBehaviour coroutineHost, List<AgentBaseTask> tasks, List<Func <bool>> preconditions)
        {
            this.State = TaskState.Waiting;
            TaskQueue = tasks;
            this.Preconditions = preconditions;
            this.coroutineHost = coroutineHost;
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
            Debug.Log("Starting TaskBundle Execution");
            State = TaskState.Running;
            if (coroutineHost == null)
            {
                Debug.LogError("coroutineHost is null");
                return;
            }
            if (CheckPreconditions())
            {
                coroutineHost.StartCoroutine(ExecuteTasks(executingAgent));
            }
        }

        /// <summary>
        /// Execute all tasks in the task queue. If a task fails, the whole bundle fails. Note, that checking of preconditions is not part of this method.
        /// </summary>
        /// <param name="executingAgent"></param>
        private IEnumerator ExecuteTasks(Agent executingAgent)
        {
            Debug.Log("TaskBundle Execution");
            for (var i = 0; i < TaskQueue.Count; i++)
            {
                var task = TaskQueue[i];
                if (i > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        task.DependsOnTasks.Add(TaskQueue[j]);
                    }
                }
                task.StartExecution(executingAgent);
                // Mini scheduler
                while (task.State != TaskState.Running && task.State != TaskState.Success && task.State != TaskState.Failure)
                {
                    task.Tick(executingAgent);
                    Debug.Log("Task " + i + " " + task.State);
                    yield return null; // wait for the next frame
                }
                while (task.State == TaskState.Running)
                {
                    task.Tick(executingAgent);
                    yield return null; // wait for the next frame
                }

                //task.StartExecution(executingAgent);
                //TaskState currentState = task.EvaluateTaskState();
                //Debug.Log("new Task " + task + TaskQueue.Count + currentState);
                if (task is AgentMovementTask)
                {
                    AgentMovementTask movementTask = (AgentMovementTask)task;
                    Debug.Log("new Task " + task + TaskQueue.Count + task.State + movementTask.Destination);
                }

                if (task.State == TaskState.Failure)
                {
                    Debug.Log("Task " + i + " failed");
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
            Debug.Log("Checking Preconditions");
            foreach (Func<bool> condition in Preconditions)
            {
                return condition();
            }

            return true;
        }
    }
}