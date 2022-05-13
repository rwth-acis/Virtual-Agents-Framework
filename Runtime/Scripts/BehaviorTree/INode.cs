using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public enum NodeState
    {
        Waiting, //Node created, but never executed
        Running, //Node 
        Failure,
        Success
    }

    public interface INode
    {
        NodeState state { get; set; }

        /// <summary>
        /// Manages nodestate and automatically triggers execute when updated fo the first time
        /// </summary>
        /// <param name="exceutingAgent"></param>
        /// <returns></returns>
        NodeState FullUpdate(Agent exceutingAgent)
        {
            if (state == NodeState.Waiting)
            {
                Execute(exceutingAgent);
                state = NodeState.Running;
            }
            
            return Update();
        }

        NodeState Update();
        void Execute(Agent exceutingAgent);
        void Stop();
    }
}
