using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public abstract class BaseTask : ITask
    {
        public TaskState State { get; set; }
        protected Agent executingAgent;

        /// <summary>
        /// Custom description that can be used to overwrite the standard description of the node in the visual Behaviour Tree editor.
        /// </summery>
        public string description = "";

        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determining when the agent has reached the target
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
            this.executingAgent = executingAgent;
        }

        /// <summary>
        /// Called when the task succeeds or fails
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
        /// Can be used to let the task succeed outside of its Update method
        /// </summary>
        public void StopAsSucceeded()
        {
            State = TaskState.Success;
            StopExecution();
        }

        public TaskState Tick(Agent executingAgent)
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
                StartExecution(executingAgent);
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

        /// <summary>
        /// Resets the task to its beginning state
        /// </summary>
        public virtual void Reset()
        {
            if (State == TaskState.Running)
            {
                StopExecution();
            }
            State = TaskState.Waiting;
        }

    }
}
