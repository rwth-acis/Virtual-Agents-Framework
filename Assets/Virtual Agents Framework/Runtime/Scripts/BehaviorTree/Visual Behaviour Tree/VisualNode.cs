using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents.BehaviourTrees.Visual
{
    /// <summary>
    /// Allows a node to be used in the visual behavior tree editor.
    /// </summary>
    [Serializable]
    public class VisualNode : TaskSerializer
    {
        public string Guid;
        public Vector2 Position;
        public string Description;
        public List<VisualNode> Children = new List<VisualNode>();
    }
}
