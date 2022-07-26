using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public abstract class BaseTask : ITask
    {
        public TaskState State { get; set; }

        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determinig when the agent has reached the target
        /// </summary>
        public virtual TaskState Update()
        {
            return State;
        }

        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        public virtual void Execute(Agent executingAgent){ }

        /// <summary>
        /// Called when the task succeedes or fails
        /// </summary>
        public virtual void Stop() { }

        /// <summary>
        /// Can be used to fail the task outside of its Update method
        /// </summary>
        public void PreemptivelyFailTask()
        {
            State = TaskState.Failure;
            Stop();
        }

        /// <summary>
        /// Can be used to let the task succseed outside of its Update method
        /// </summary>
        public void PreemptivelySuccedTask()
        {
            State = TaskState.Success;
            Stop();
        }

        public TaskState FullUpdate(Agent excutingAgent)
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
                Execute(excutingAgent);
                //Check if the task already finished, in the Execute()
                if (State == TaskState.Success || State == TaskState.Failure)
                {
                    return State;
                }
            }

            State = Update();


            //Check if the task finished in the last Update()
            if (State == TaskState.Success || State == TaskState.Failure)
            {
                Stop();
            }

            return State;
        }

        /// <summary>
        /// Resets the task to its beginning state
        /// </summary>
        public virtual void Reset()
        {
            State = TaskState.Waiting;
        }

    }
}
