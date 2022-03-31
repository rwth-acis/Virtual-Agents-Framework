using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace i5.VirtualAgents.TaskSystem
{
    public class TaskSynchroniser
    {
        public HashSet<IAgentTask> tasks = new HashSet<IAgentTask>();
        private HashSet<IAgentTask> readyTasks = new HashSet<IAgentTask>();

        public Func<bool> WaitForOtherTasksBegin(IAgentTask ownTask)
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
