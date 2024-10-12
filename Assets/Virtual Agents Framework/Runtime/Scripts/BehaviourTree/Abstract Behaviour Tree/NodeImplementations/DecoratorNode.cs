using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Default implementation of the IDecoratorNode interface
    /// </summary>
    public abstract class DecoratorNode : BaseTask, IDecoratorNode
    {
        public ITask Child { get; set; }

        /// <summary>
        /// Resets entire subtree
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Child.Reset();
        }
    }
}
