using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Nodes that execute other nodes
    /// </summary>
    public interface ICompositeNode : ITask
    {
        List<ITask> Children {get; set;}
    }
}
