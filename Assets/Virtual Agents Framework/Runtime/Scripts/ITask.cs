using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public enum TaskState
    {
        Waiting, // Task created, but never executed
        Running, // Task currently running
        Failure, // Task has finished executing and failed
        Success  // Task has finished executing and succeeded
    }

    /// <summary>
    /// Task that can be executed by ITaskSystems. Needs to be updated with FullUpdate() in order to perform work.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// The current state of the task
        /// </summary>
        TaskState State { get; set; }

        /// <summary>
        /// Evaluates the task's current state
        /// </summary>
        TaskState EvaluateTaskState();

        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        void StartExecution(Agent executingAgent);

        /// <summary>
        /// Called when the task succeedes or fails
        /// </summary>
        void StopExecution();

        /// <summary>
        /// Updates the State and automatically invokes StartExecution() on first update and StopExeuction() when task succeeds/fails.
        /// </summary>
        /// <param name="excutingAgent"></param>
        /// <returns></returns>

        TaskState Update(Agent excutingAgent);

        /// <summary>
        /// Resets the task to its beginning state
        /// </summary>
        void Reset();
    }
}
