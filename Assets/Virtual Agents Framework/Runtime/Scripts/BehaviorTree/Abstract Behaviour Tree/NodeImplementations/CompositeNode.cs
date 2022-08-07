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
        public List<ITask> Children { get; set; }
        protected Agent executingAgent;

        /// <summary>
        /// Gets the reference to the agent which will execute this task
        /// Starts the task's execution
        /// </summary>
        /// <param name="agent">The agent which should execute this task</param>
        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            this.executingAgent = executingAgent;
        }

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
