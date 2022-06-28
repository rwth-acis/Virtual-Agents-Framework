using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    public interface ICompositeNode : ITask
    {
        List<ITask> children {get; set;}
    }
}
