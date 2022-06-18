using System;
using System.Collections.Generic;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// Holds an IAgentTask queue
    /// </summary>
    public class AgentTaskQueue
    {
        private List<TaskEntry> taskQueue;

        /// <summary>
        /// Create an empty IAgentTask queue
        /// </summary>
        public AgentTaskQueue()
        {
            taskQueue = new List<TaskEntry>();
        }

        /// <summary>
        /// Request the next task from the queue
        /// </summary>
        /// <returns>Next task from the queue or null if the queue is empty</returns>
        public IAgentTask RequestNextTask()
        {
            IAgentTask result = PeekNextTask();
            if (result != null)
            {
                taskQueue.RemoveAt(0);
            }
            return result;
        }

        /// <summary>
        /// Peeks the next task in the queue
        /// </summary>
        /// <returns>Returns the next upcoming task in the queue, null if no more tasks are queued</returns>
        public IAgentTask PeekNextTask()
        {
            if (taskQueue.Count > 0)
            {
                IAgentTask result = taskQueue[0].task;
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Add a new task to the queue according to the FIFO principle but with priority categories
        /// </summary>
        /// <param name="task">Any task that implements the IAgentTask interface</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public void AddTask(IAgentTask task, int priority = 0)
        {
            int insertIndex = taskQueue.Count;
            for (int i = 0; i < taskQueue.Count; i++)
            {
                if (taskQueue[i].priority < priority)
                {
                    insertIndex = i;
                    break;
                }
            }

            taskQueue.Insert(insertIndex,
                new TaskEntry()
                {
                    task = task,
                    priority = priority
                });
        }

        /// <summary>
        /// Contains a task, its corresponding priority and the task bundle it belongs to if it is part of one
        /// </summary>
        private struct TaskEntry
        {
            public IAgentTask task;
            public int priority;
        }
    }


}
