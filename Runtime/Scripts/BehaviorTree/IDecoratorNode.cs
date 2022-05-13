using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public interface IDecoratorNode : INode
    {
        INode child { get; set; }
    }
}
