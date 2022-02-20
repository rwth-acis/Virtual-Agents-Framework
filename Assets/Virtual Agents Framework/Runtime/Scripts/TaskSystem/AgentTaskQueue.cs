using System.Collections.Generic;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// Holds an IAgentTask queue
    /// </summary>
    public class AgentTaskQueue
    {
        private LinkedList<IAgentTask> taskQueue;

        /// <summary>
        /// Create an empty IAgentTask queue
        /// </summary>
        public AgentTaskQueue()
        {
            taskQueue = new LinkedList<IAgentTask>();
        }

        /// <summary>
        /// Request the next task from the queue
        /// </summary>
        /// <returns>Next task from the queue or null if the queue is empty</returns>
        public IAgentTask RequestNextTask()
        {
            if (taskQueue.First != null)
            {
                IAgentTask result = taskQueue.First.Value;
                taskQueue.RemoveFirst();
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Add a new task to the queue according to the FIFO principle
        /// </summary>
        /// <param name="task">Any task that implements the IAgentTask interface</param>
        public void AddAtBack(IAgentTask task)
        {
            taskQueue.AddLast(task);
        }

        /// <summary>
        /// Make a task jump the queue instead of scheduling it.
        /// The task is performed as soon as possible and the rest
        /// of the queue remains intact
        /// </summary>
        /// <param name="task">Any task that implements the IAgentTask interface</param>
        public void AddAtFront(IAgentTask task)
        {
            taskQueue.AddFirst(task);
        }
    }
}
