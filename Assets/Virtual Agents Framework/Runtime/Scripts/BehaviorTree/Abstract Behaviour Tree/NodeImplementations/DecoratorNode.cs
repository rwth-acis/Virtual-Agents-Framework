using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    public abstract class DecoratorNode : BaseTask, IDecoratorNode
    {
        public ITask Child { get; set; }
        protected Agent executingAgent;

        public override void Reset()
        {
            base.Reset();
            Child.Reset();
        }

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            this.executingAgent = executingAgent;
        }
    }
}
