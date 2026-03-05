using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Default implementation of the ICompositeNode interface
    /// </summary>
    public abstract class CompositeNode : BaseTask, ICompositeNode
    {
        private List<ITask> children = new List<ITask>();
        public List<ITask> Children { get { return children; } set { children = value; } }

        /// <summary>
        /// Resets entire subtree
        /// </summary>
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
