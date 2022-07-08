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
    public interface ITask
    {
        TaskState Update();
        void Execute(Agent executingAgent);
        void Stop();

        TaskState rootState { get; set; }

        /// <summary>
        /// Can be used to fail the task outside of its Update method
        /// </summary>
        void PreemptivelyFailTask()
        {
            rootState = TaskState.Failure;
            Stop();
        }

        /// <summary>
        /// Can be used to fail the task outside of its Update method
        /// </summary>
        void PreemptivelySuccedTask()
        {
            rootState = TaskState.Success;
            Stop();
        }


        /// <summary>
        /// Updates rootState and automattically invokes Execute() on first update and Stop() when task succeeds/fails.
        /// </summary>
        /// <param name="excutingAgent"></param>
        /// <returns></returns>
        TaskState FullUpdate(Agent excutingAgent)
        {
            //Is the task already finished?
            if (rootState == TaskState.Success || rootState == TaskState.Failure)
            {
                return rootState; //Don't update the task any further
            }

            //Is the task updated for the first time?
            if (rootState == TaskState.Waiting)
            {
                rootState = TaskState.Running;
                Execute(excutingAgent);
                //Check if the task already finished, in the Execute()
                if (rootState == TaskState.Success || rootState == TaskState.Failure)
                {
                    return rootState;
                }
            }
            
            rootState = Update();


            //Check if the task finished in te last Update
            if (rootState == TaskState.Success || rootState == TaskState.Failure)
            {
                Stop();
            }

            return rootState;
        }
    }
}
