using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace i5.VirtualAgents.TaskSystem
{
    public class TaskSynchroniser
    {
        private HashSet<IAgentTask> tasks = new HashSet<IAgentTask>();
        private HashSet<IAgentTask> readyTasks = new HashSet<IAgentTask>();

        /// <summary>
        /// All tasks that use the returned function as prepare schedule function will start simultaneously
        /// </summary>
        /// <param name="ownTask"></param> The task that wants to be part of the mutual scheduling
        /// <returns></returns>
        public Func<bool> WaitForOtherTasksMutually(IAgentTask ownTask)
        {
            tasks.Add(ownTask);

            bool MarkAndCheckStatus()
            {
                readyTasks.Add(ownTask);
                return tasks.Count == readyTasks.Count;
            }

            return MarkAndCheckStatus;
        }
    }


}
