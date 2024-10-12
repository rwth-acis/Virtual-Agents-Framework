using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.ScheduleBasedExecution;
using UnityEngine;

namespace i5.VirtualAgents
{
    /// <summary>
    /// A task which consists of multiple subtasks.
    /// It allows for checking of preconditions and then executing a sequence of tasks.
    /// </summary>
    public class TaskBundle : AgentBaseTask
    {
        // The TaskManager that is used to schedule and manage the tasks in a bundle
        private AgentTaskManager taskManager = new AgentTaskManager();

        /// <summary>
        /// List of conditions to be met before execution of tasks
        /// </summary>
        public List<Func<bool>> Preconditions { get; private set; }

        /// <summary>
        /// Creates an empty TaskBundle
        /// </summary>
        public TaskBundle()
        {
            State = TaskState.Waiting;
            Preconditions = new List<Func<bool>>();
        }

        /// <summary>
        /// Creates a TaskBundle with a list of tasks
        /// </summary>
        /// <param name="tasks"></param>
        public TaskBundle(List <AgentBaseTask> tasks)
        {
            State = TaskState.Waiting;
            AddTasks(tasks);
        }

        /// <summary>
        /// Creates a TaskBundle with a list of tasks and a list of preconditions
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="preconditions"></param>
        public TaskBundle(List <AgentBaseTask> tasks, List<Func <bool>> preconditions)
        {
            State = TaskState.Waiting;
            AddTasks(tasks);
            Preconditions = preconditions;
        }

        /// <summary>
        /// Adds a task to the task queue after initialisation
        /// </summary>
        /// <param name="task">The task to be added to the task queue</param>
        public void AddTask(AgentBaseTask task)
        {
            taskManager.ScheduleTask(task, 0);
        }

        /// <summary>
        /// Adds a list of tasks to the task queue after initialisation
        /// </summary>
        /// <param name="taskList">The task list to be added to the task queue</param>
        public void AddTasks(List<AgentBaseTask> taskList)
        {
            foreach (var task in taskList)
            {
                taskManager.ScheduleTask(task, 0);
            }
        }

        /// <summary>
        /// Adds a precondition to the list of preconditions after initialisation
        /// Note: Preconditions are only checked before the tasks are executed,
        /// a precondition added after the execution has started will not be checked
        /// </summary>
        /// <param name="precondition">The precondition to be added to the list of preconditions</param>
        public void AddPrecondition(Func<bool> precondition)
        {
            if(TaskState.Waiting != State)
            {
                Debug.LogWarning("Preconditions can only be checked before the TaskBundle is started");
            }
            Preconditions.Add(precondition);
        }

        /// <summary>
        /// Check for preconditions and start the execution of all subtasks in sequence
        /// </summary>
        /// <param name="executingAgent"></param>
        public override void StartExecution(Agent executingAgent)
        {
            taskManager.AssociateAgent(executingAgent);
            State = TaskState.Running;
            if (CheckPreconditions())
            {
                taskManager.IsActive = true;
            }
            else
            {
                Debug.Log("Preconditions of TaskBundle not met");
                StopAsFailed();
            }
        }

        /// <summary>
        /// Execute all tasks in the task queue. If a task fails, the whole bundle fails. Note, that checking of preconditions is not part of this method.
        /// </summary>
        /// <param name="executingAgent"></param>
        public override TaskState EvaluateTaskState()
        {
            taskManager.Update();

            if (taskManager.IsActive)
            {
                TaskState state =  taskManager.CheckTaskQueueStates();
                if (state == TaskState.Failure)
                {
                    Debug.LogWarning("Task bundle failed");
                    StopAsFailed();
                }

                if (state == TaskState.Success)
                {
                    StopAsSucceeded();
                }

                if (state == TaskState.Running)
                {
                    State = state;
                }
            }

            return State;
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