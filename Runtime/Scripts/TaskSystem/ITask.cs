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
        void Execute(Agent exceutingAgent);
        void Stop();

        TaskState rootState { get; set; }

        /// <summary>
        /// When updated for the first time, sets rootState to running and invokes Execute().
        /// </summary>
        /// <param name="excutingAgent"></param>
        /// <returns></returns>
        TaskState FullUpdate(Agent excutingAgent)
        {
            if (rootState == TaskState.Waiting)
            {
                Execute(excutingAgent);
                rootState = TaskState.Running;
            }

            return Update();
        }
    }
}
