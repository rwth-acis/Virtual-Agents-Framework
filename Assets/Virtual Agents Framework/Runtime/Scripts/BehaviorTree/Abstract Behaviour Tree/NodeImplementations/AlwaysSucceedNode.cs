using System;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Converts the task endstate to Success
    /// </summary>
    public class AlwaysSucceedNode : DecoratorNode, ISerializable
    {
        public override TaskState Update()
        {
            TaskState childState = Child.FullUpdate(executingAgent);
            if (childState == TaskState.Failure)
            {
                return TaskState.Success;
            }
            else
            {
                return childState;
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
