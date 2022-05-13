using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents
{
    public class SelectorNode : ICompositeNode
    {
        public List<INode> children { get; set; }
        public NodeState state { get; set; }
        Agent executingAgent;
        public void Execute(Agent executingAgent)
        {
            state = NodeState.Running;
            this.executingAgent = executingAgent;
        }
        int current = 0;

        public void Stop()
        {
        }

        public NodeState Update()
        {
            NodeState currentNodestate = children[current].FullUpdate(executingAgent);


            if (currentNodestate == NodeState.Failure)
            {
                current++;
                if (current >= children.Count)
                    return NodeState.Failure; //All nodes failed, report general failure
                else
                    return NodeState.Running; 
            }
            else
            {
                return currentNodestate;
            }
        }
    }
}
