using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public enum TaskState
    {
        Waiting, //Node created, but never executed
        Running, //Node 
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
        /// Updates rootState and automattically invokes Execute() on first update and Stop() when task succeeds/fails.
        /// </summary>
        /// <param name="excutingAgent"></param>
        /// <returns></returns>
        TaskState FullUpdate(Agent excutingAgent)
        {
            //On first update, invoke Execute()
            if (rootState == TaskState.Waiting)
            {
                rootState = TaskState.Running;
                Execute(excutingAgent);
            }
            
            //Checking failure here is important in case the task changed its root state on its own
            if (rootState == TaskState.Failure)
            {
                Stop();
                return TaskState.Failure;
            }
            else
            {
                rootState = Update();
            }

            //Invoke stop method when the task state switches from not Sucess/Failure to Sucess/Failure
            if (rootState == TaskState.Success || rootState == TaskState.Failure)
            {
                Stop();
            }

            return rootState;
        }
    }
}
