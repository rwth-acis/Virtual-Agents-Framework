using System;
using System.Collections.Generic;

namespace i5.VirtualAgents.TaskSystem
{
    /// <summary>
    /// Common methods and attributes for all AgentTasks
    /// </summary>
    public interface IAgentTask : INode
    {
        /// <summary>
        /// Event which is invoked once the task is finished
        /// Subscribed to by the agent's task manager so that the next task can be started afterwards
        /// </summary>
        //event Action OnTaskFinished;

        /// <summary>
        /// Can be used to delay the scheduling of the task. Once scheduled, the taskmanager will call all ReadyToStart functions every frame until all of them signal that the task is ready (i.e. retrun true).
        /// This is e.g. usefull to delay the task until another task started/finished.
        /// If null/empty, taskmanager will asssume the task is ready
        /// </summary>
        List<Func<bool>> ReadyToStart { get; set; }

        /// <summary>
        /// Can be used to delay the end of the task. Once OnTaskFinished was invoked, the taskmanager will call all ReadyToEnd functions every frame until all of them signal that the task is ready (i.e. retrun true).
        /// This is e.g. usefull to artifically lengthen a task to match the end time of another task in order to have them end simultaniously.
        /// If null/empty, taskmanager will asssume the task is ready
        /// </summary>
        List<Func<bool>> ReadyToEnd { get; set; }
    }
}