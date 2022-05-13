using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents
{
    public class SequencerNode : ICompositeNode
    {
        public List<INode> children { get; set; }
        public NodeState state { get; set; }
        int current = 0;
        Agent executingAgent;

        public void Execute(Agent executingAgent)
        {
            this.executingAgent = executingAgent;
        }

        public void Stop()
        {
        }

        public NodeState Update()
        {
            NodeState currentNodestate = children[current].FullUpdate(executingAgent);

            if (currentNodestate == NodeState.Success)
            {
                current++;
                if (current >= children.Count)
                    return NodeState.Success;
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
