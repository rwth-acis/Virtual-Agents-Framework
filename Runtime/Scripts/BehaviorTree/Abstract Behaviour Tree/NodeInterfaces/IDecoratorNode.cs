using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public interface IDecoratorNode : ITask
    {
        ITask child { get; set; }
    }
}
