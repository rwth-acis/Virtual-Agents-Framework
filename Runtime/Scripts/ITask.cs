using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    /// <summary>
    /// The state of a task, one out of five possible states: waiting, running, failure, success, aborted
    /// </summary>
    public enum TaskState
    {
        Waiting, // Task created, but never executed
        Running, // Task currently running
        Failure, // Task has finished executing and failed
        Success,  // Task has finished executing and succeeded
        Aborted // Task has been aborted
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
        /// Called when the task succeeds or fails
        /// </summary>
        void StopExecution();

        /// <summary>
        /// Updates the State and automatically invokes StartExecution() on first update and StopExecution() when task succeeds/fails.
        /// </summary>
        /// <param name="executingAgent"></param>
        /// <returns></returns>
        TaskState Tick(Agent executingAgent);
    }
}
