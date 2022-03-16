using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;
using System;

namespace i5.VirtualAgents
{
    /// <summary>
    /// A bundle of tasks that shall be executed in parralel.
    /// The task excecution will only start once all corresponding taskmanagers are ready to excecute their tasks and they will block their excecution layer until all tasks from this bundle are finished.
    /// </summary>
    public class TaskBundel
    {
        /// <summary>
        /// The taskmanagers and there correponding tasks that shall be excecuted as part of this bundle
        /// </summary>
        public Dictionary<AgentTaskManager, IAgentTask> Tasks {get; private set;}
        public int priority { get; private set; }
        //Taskmanagers that are ready to execute 
        private HashSet<AgentTaskManager> readyTaskManagers;
        //Taskmanagers that finsihed their execution
        private HashSet<AgentTaskManager> finishedTaskManagers;

        public TaskBundel(int priority = 0)
        {
            Tasks = new Dictionary<AgentTaskManager, IAgentTask>();
            readyTaskManagers = new HashSet<AgentTaskManager>();
            finishedTaskManagers = new HashSet<AgentTaskManager>();
        }

        /// <summary>
        /// Add taskmanager and the task it shall execute in this bundle
        /// </summary>
        /// <param name="taskManager"></param>
        /// <param name="task"></param>
        public void AddTask(AgentTaskManager taskManager, IAgentTask task)
        {
            Tasks.Add(taskManager, task);
        }

        /// <summary>
        /// Schedules this bundel in all added taskmanagers
        /// </summary>
        public void ScheduleTaskBundel()
        {
            foreach (var taskManager in Tasks.Keys)
            {
                taskManager.ScheduleTaskBundel(this);
            }
        }

        /// <summary>
        /// Marks a tasksmanager as ready to excecute its task from this bundel. If it was the last manager to be set on ready, OnAllTasksReady is invoked.
        /// </summary>
        /// <param name="agentTaskManager"></param> The taskmanager that is ready to execute its task from this bundel
        public void TaskManagerReady(AgentTaskManager agentTaskManager)
        {
            if (Tasks.ContainsKey(agentTaskManager))
            {
                readyTaskManagers.Add(agentTaskManager);
            }
            if (readyTaskManagers.Count == Tasks.Count)
            {
                foreach (var taskManager in Tasks)
                {
                    taskManager.Key.StartBundle();
                }
            }
        }

        /// <summary>
        /// Marks a tasksmanager that a taskmamanger finished its task from this bundel. If it was the last manager to be marked, OnAllTasksFinsihed is invoked.
        /// </summary>
        /// <param name="agentTaskManager"></param> The taskmanager that finished its task from this bundel
        public void TaskManagerFinished(AgentTaskManager agentTaskManager)
        {
            if (Tasks.ContainsKey(agentTaskManager))
            {
                finishedTaskManagers.Add(agentTaskManager);
            }
            if (finishedTaskManagers.Count == Tasks.Count)
            {
                foreach (var taskManager in Tasks)
                {
                    taskManager.Key.EndBundle();
                }
            }
        }
    }
}
