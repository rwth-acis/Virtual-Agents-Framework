using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public enum TaskState
    {
        Waiting, //Task created, but never executed
        Running, //Task currently running
        Failure,
        Success
    }

    /// <summary>
    /// Task that can be executed by ITaskSystems. Needs to be updated with FullUpdate() in order to perform work.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Called by the executing agent on running tasks
        /// Performs frame-to-frame task execution updates
        /// This is e.g. useful for tracking movements towards a target and determinig when the agent has reached the target
        /// </summary>
        TaskState Update();

        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        void Execute(Agent executingAgent);

        /// <summary>
        /// Called when the task succeedes or fails
        /// </summary>
        void Stop();

        TaskState State { get; set; }

        /// <summary>
        /// Can be used to fail the task outside of its Update method
        /// </summary>
        void PreemptivelyFailTask()
        {
            State = TaskState.Failure;
            Stop();
        }

        /// <summary>
        /// Can be used to let the task succseed outside of its Update method
        /// </summary>
        void PreemptivelySuccedTask()
        {
            State = TaskState.Success;
            Stop();
        }


        /// <summary>
        /// Updates the State and automatically invokes Execute() on first update and Stop() when task succeeds/fails.
        /// </summary>
        /// <param name="excutingAgent"></param>
        /// <returns></returns>
        TaskState FullUpdate(Agent excutingAgent)
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
    }
}
