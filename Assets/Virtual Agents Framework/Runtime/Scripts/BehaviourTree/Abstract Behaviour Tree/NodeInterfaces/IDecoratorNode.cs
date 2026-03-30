using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Nodes that change the execution behaviour of attached nodes
    /// </summary>
    public interface IDecoratorNode : ITask
    {
        ITask Child { get; set; }
    }
}
