using i5.VirtualAgents.BehaviourTrees.Visual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Executes a given behaviour tree
    /// </summary>
    public class BehaviorTreeTask : AgentBaseTask, ISerializable
    {
        public BehaviorTreeAsset tree;
        private ITask root;

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            root = tree.GetExecutableTree();
        }

        public override TaskState Update()
        {
            return root.FullUpdate(executingAgent);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            tree = serializer.GetSerializedTrees("Tree");
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Tree", tree);
        }
    }
}
