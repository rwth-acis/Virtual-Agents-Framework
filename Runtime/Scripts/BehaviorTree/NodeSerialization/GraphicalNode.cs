using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Experimental.GraphView;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Allows a node to be used in the visual behavior tree editor
    /// </summary>
    [Serializable]
    public class GraphicalNode : TaskSerializer
    {
        public string guid;
        public Vector2 position;
        public string description;
        public List<GraphicalNode> children = new List<GraphicalNode>();
    }
}
