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
        NodeState Update();
        void Execute(Agent exceutingAgent);
        void Stop();
    }
}
