using i5.VirtualAgents.AgentTasks;
using System;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Repeats its child until it succeeds
    /// </summary>
    public class RepeatUntilSuccessNode : DecoratorNode, ISerializable
    {
        // The maximum number of times, the task is repeated. If 0, the task will be repeated indefinitely, until it succeeds
        private int RepeatLimit;
        private int repeatCount;

        public override void StartExecution(Agent executingAgent)
        {
            base.StartExecution(executingAgent);
            repeatCount = 0;
        }

        public override TaskState EvaluateTaskState()
        {
            switch (Child.Tick(executingAgent))
            {
                case TaskState.Waiting:
                    return TaskState.Waiting;
                case TaskState.Running:
                    return TaskState.Running;
                case TaskState.Success:
                    return TaskState.Success;
                case TaskState.Failure:
                    // The task failed, restart it and try again, as long as the limit allows it.
                    if (RepeatLimit == 0 || repeatCount < RepeatLimit)
                    {
                        Child.Reset();
                        repeatCount++;
                        return TaskState.Running;
                    }
                    else
                    {
                        return TaskState.Failure;
                    }
                default:
                    throw new NotImplementedException();
                    
            }
              
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            RepeatLimit = serializer.GetSerializedInt("RepeatLimit");
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("RepeatLimit", RepeatLimit);
        }
    }
}
