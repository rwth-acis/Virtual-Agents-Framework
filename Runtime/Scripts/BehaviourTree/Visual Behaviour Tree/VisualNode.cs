using i5.VirtualAgents.AgentTasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees.Visual
{
    /// <summary>
    /// Allows a node to be used in the visual behaviour tree editor.
    /// </summary>
    [Serializable]
    public class VisualNode : TaskSerializer
    {
        public string Guid;
        public Vector2 Position;
        public List<VisualNode> Children = new List<VisualNode>();
        /// <summary>
        /// The corresponding Task in the abstract behaviour tree build in BehaviourTreeAsset
        /// </summary>
        public Dictionary<Agent, ITask> CorrespondingTask { get; set; } = new Dictionary<Agent, ITask>();

        // Used to clear dictionary when not in playmode
        public void ClearCorrespondingTask()
        {
            CorrespondingTask?.Clear();
        }
    }
}
