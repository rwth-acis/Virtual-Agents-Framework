using i5.Toolkit.Core.Utilities;
using System;
using System.Collections.Generic;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    /// <summary>
    /// Base class which provides default implementations for the interface methods
    /// Using this class avoids repeatedly implementing empty interface classes if they are not needed
    /// </summary>
    public abstract class AgentBaseTask : IAgentTask
    {
        /// <summary>
        /// List of tasks which need to finish first in order for this task to start
        /// </summary>
        public List<IAgentTask> DependsOnTasks
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicates whether this task is ready to start execution
        /// Checks whether all depending tasks are finished
        /// Can be overridden to add custom conditions in child classes
        /// </summary>
        public virtual bool CanStart
        {
            get
            {
                bool canStart = true;
                foreach(IAgentTask task in DependsOnTasks)
                {
                    canStart &= task.IsFinished;
                }
                return canStart;
            }
        }

        /// <summary>
        /// Indicates whether the task is finished
        /// </summary>
        public bool IsFinished { get; protected set; } = false;

        /// <summary>
        /// Creates a new task
        /// </summary>
        public AgentBaseTask()
        {
            DependsOnTasks = new List<IAgentTask>();
        }

        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        public virtual void Execute(Agent agent) { }

        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determinig when the agent has reached the target
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Event which is invoked once the task is finished
        /// Subscribed to by the agent's task manager so that the next task can be started afterwards
        /// </summary>
        public event Action OnTaskFinished;

        /// <summary>
        /// Marks the task as finished for the executing agent
        /// This will raise the OnTaskFinished event in the base class
        /// Use this method to finish the task
        /// as invoking the event in derived classes is not possible (https://stackoverflow.com/a/31661451)
        /// </summary>
        protected virtual void FinishTask()
        {
            IsFinished = true;
            DependsOnTasks.Clear();
            OnTaskFinished();
        }

        /// <summary>
        /// Indicates that the task has to wait for at least one oter task to finish first
        /// Adds the tasks to the list of dependencies
        /// </summary>
        /// <param name="otherTasks">The other tasks which have to finish before this task can start</param>
        public void WaitFor(params AgentBaseTask[] otherTasks)
        {
            foreach (AgentBaseTask otherTask in otherTasks)
            {
                // detect immediate deadlocks
                // for more complex transitive deadlocks we should consider adding an analyzer in the future
                if (otherTask.DependsOnTasks.Contains(this))
                {
                    i5Debug.LogWarning($"Did not add task {otherTask.ToString()} as dependency to {this.ToString()}" +
                        $" because there is already an inverse dependency. Avoiding deadlock.", this);
                    continue;
                }

                // avoid duplicates
                if (!DependsOnTasks.Contains(otherTask))
                {
                    DependsOnTasks.Add(otherTask);
                }
            }
        }
    }
}