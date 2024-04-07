using System;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Inverts tasks end states
    /// </summary>
    public class InverterNode : DecoratorNode, ISerializable
    {

        public override TaskState EvaluateTaskState()
        {
            switch (Child.Tick(executingAgent))
            {
                // When not finished, just pass state through
                case TaskState.Waiting:
                    return TaskState.Waiting;
                case TaskState.Running:
                    return TaskState.Running;

                // When finished, invert state
                case TaskState.Success:
                    return TaskState.Failure;
                case TaskState.Failure:
                    return TaskState.Success;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
        }

        public void Serialize(SerializationDataContainer serializer)
        {
        }

    }
}
