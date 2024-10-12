using System;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Converts the child end state to Success or successes automatically when there is no child
    /// </summary>
    public class AlwaysSucceedNode : DecoratorNode, ISerializable
    {
        public override TaskState EvaluateTaskState()
        {
            if(Child == null)
            {
                // Always succeed node should also succeed if it has no child
                return TaskState.Success;
            }
            TaskState childState = Child.Tick(executingAgent);
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
