using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace i5.VirtualAgents.TaskSystem
{
    public class TaskSynchronizer
    {
        private Dictionary<IAgentTask, HashSet<IAgentTask>> dependencyMap = new Dictionary<IAgentTask, HashSet<IAgentTask>>();

        private Dictionary<string, AgentTaskManager> taskManagers;

        public TaskSynchronizer(Dictionary<string, AgentTaskManager> taskManagers)
        {
            this.taskManagers = taskManagers;
            foreach (AgentTaskManager taskManager in taskManagers.Values)
            {
                taskManager.OnTaskFinished += OnTaskFinished;
            }
        }

        private void OnTaskFinished(AgentTaskManager sender, IAgentTask finishedTask)
        {
            // remove finished task's dependency list as it is no longer needed
            // however, the finished task can still be a dependency for other tasks
            dependencyMap.Remove(finishedTask);

            CheckTaskManagers();
        }

        private void CheckTaskManagers()
        {
            foreach (AgentTaskManager taskManager in taskManagers.Values)
            {
                // re-evaluate every task manager that is currently on hold
                if (taskManager.CurrentState == TaskState.idle || taskManager.CurrentState == TaskState.inactive)
                {
                    IAgentTask nextTask = taskManager.PeekNextTask();
                    if (nextTask == null)
                    {
                        continue;
                    }
                    taskManager.IsActive = CanRun(nextTask);
                }
            }
        }

        public void DefineTaskDependency(IAgentTask task, IAgentTask[] dependencies)
        {
            if (!dependencyMap.ContainsKey(task))
            {
                dependencyMap.Add(task, new HashSet<IAgentTask>());
            }

            for (int i = 0; i < dependencies.Length; i++)
            {
                dependencyMap[task].Add(dependencies[i]);
            }
        }

        private bool CanRun(IAgentTask task)
        {
            if (dependencyMap.ContainsKey(task))
            {
                bool canRun = true;
                foreach (IAgentTask dependency in dependencyMap[task])
                {
                    canRun &= dependency.IsFinished;
                }
                return canRun;
            }
            else
            {
                return true;
            }
        }
    }
}
