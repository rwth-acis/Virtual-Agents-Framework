using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents
{
    public interface ICompositeNode : INode
    {
        List<INode> children { get; set; }
    }
}
