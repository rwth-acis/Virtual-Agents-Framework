using System;

namespace VirtualAgentsFramework.TaskSystem
{
    /// <summary>
    /// Common methods and attributes for all AgentTasks
    /// </summary>
    public interface IAgentTask
    {
        // Get the agent's data, prepare for and start task execution
        void Execute(Agent agent);
        // Perform frame-to-frame task execution
        void Update();
        // Fire when the task is finished to let the agent know
        event Action OnTaskFinished;
    }
}