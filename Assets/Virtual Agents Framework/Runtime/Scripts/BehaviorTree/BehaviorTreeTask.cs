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

        public override void StartExecution(Agent executingAgent)
        {
            base.StartExecution(executingAgent);
            root = tree.GetExecutableTree();
        }

        public override TaskState EvaluateTaskState()
        {
            return root.Tick(executingAgent);
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
