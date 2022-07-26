using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    public abstract class DecoratorNode : BaseTask, IDecoratorNode
    {
        public ITask Child { get; set; }

        public override void Reset()
        {
            base.Reset();
            Child.Reset();
        }
    }
}
