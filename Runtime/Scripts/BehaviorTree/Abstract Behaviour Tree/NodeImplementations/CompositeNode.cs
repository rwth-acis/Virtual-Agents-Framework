using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    public abstract class CompositeNode : BaseTask, ICompositeNode
    {
        public List<ITask> Children { get; set; }
        protected Agent executingAgent;

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            this.executingAgent = executingAgent;
        }

        public override void Reset()
        {
            base.Reset();
            foreach (var child in Children)
            {
                child.Reset();
            }
        }

    }
}
