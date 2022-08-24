using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.BehaviourTrees.Visual
{
    public class RootNode : DecoratorNode, IRootNode, ISerializable
    {
        public override TaskState Update()
        {
            return Child.FullUpdate(executingAgent);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
        }

        public void Serialize(SerializationDataContainer serializer)
        {
        }
    }
}
