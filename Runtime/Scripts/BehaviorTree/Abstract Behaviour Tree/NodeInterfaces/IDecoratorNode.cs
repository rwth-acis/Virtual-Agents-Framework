using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    public interface IDecoratorNode : ITask
    {
        ITask child { get; set; }
    }
}
