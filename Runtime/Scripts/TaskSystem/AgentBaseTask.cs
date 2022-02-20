using System;

namespace i5.VirtualAgents.TaskSystem
{
    namespace AgentTasks
    {
        /// <summary>
        /// Makes it possible to avoid implementing Update()
        /// and Execute() functions when they are not needed
        /// </summary>
        public abstract class AgentBaseTask : IAgentTask
        {
            // Get the agent's data, prepare for and start task execution
            public virtual void Execute(Agent agent) {}
            // Perform frame-to-frame task execution
            public virtual void Update() {}
            // Fire when the task is finished to let the agent know
            public event Action OnTaskFinished;

            protected virtual void MarkAsFinished() // The event itself cannot be called
                                                // in derived classes (https://stackoverflow.com/a/31661451)
            {
                OnTaskFinished();
            }
        }
    }
}
