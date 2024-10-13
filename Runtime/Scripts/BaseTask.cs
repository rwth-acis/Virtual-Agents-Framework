using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public abstract class BaseTask : ITask
    {
        /// <summary>
        /// The state of the task
        /// </summary>
        public TaskState State { get; set; }

        /// <summary>
        /// Event called when the task is started
        /// </summary>
        public event Action OnTaskStarted;

        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determinig when the agent has reached the target
        /// </summary>
        public virtual TaskState EvaluateTaskState()
        {
            return State;
        }

        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        public virtual void StartExecution(Agent executingAgent)
        {
            OnTaskStarted?.Invoke();
        }

        /// <summary>
        /// Called when the task succeedes or fails
        /// </summary>
        public virtual void StopExecution() { }

        /// <summary>
        /// Can be used to fail the task outside of its Update method
        /// </summary>
        public void StopAsFailed()
        {
            State = TaskState.Failure;
            StopExecution();
        }

        /// <summary>
        /// Can be used to abort the task outside of its Update method
        /// </summary>
        public void StopAsAborted()
        {
            State = TaskState.Aborted;
            StopExecution();
        }

        /// <summary>
        /// Can be used to let the task succseed outside of its Update method
        /// </summary>
        public void StopAsSucceeded()
        {
            State = TaskState.Success;
            StopExecution();
        }

        /// <summary>
        /// Updates the State and automatically invokes StartExecution() on first update and StopExeuction() when task succeeds/fails.
        /// </summary>
        /// <param name="excutingAgent"></param>
        /// <returns></returns>
        public TaskState Tick(Agent excutingAgent)
        {
            //Is the task already finished?
            if (State == TaskState.Success || State == TaskState.Failure)
            {
                return State; //Don't update the task any further
            }

            //Is the task updated for the first time?
            if (State == TaskState.Waiting)
            {
                State = TaskState.Running;
                StartExecution(excutingAgent);
                //Check if the task already finished, in the Execute()
                if (State == TaskState.Success || State == TaskState.Failure)
                {
                    return State;
                }
            }

            State = EvaluateTaskState();


            //Check if the task finished in the last Update()
            if (State == TaskState.Success || State == TaskState.Failure)
            {
                StopExecution();
            }

            return State;
        }


    }
}
