using i5.VirtualAgents.AgentTasks;
using System;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Executes one of its child tasks at random
    /// </summary>
    public class RandomNode : CompositeNode, ISerializable
    {
        private ITask randomTask;

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            randomTask = Children[UnityEngine.Random.Range(0, Children.Count)];
        }

        public override TaskState Update()
        {
            return randomTask.FullUpdate(executingAgent);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
        }

        public void Serialize(SerializationDataContainer serializer)
        {
        }
    }
}
