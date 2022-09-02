using i5.VirtualAgents.AgentTasks;
using System;
using System.Collections.Generic;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Executes one of its child tasks at random
    /// </summary>
    public class RandomNode : CompositeNode, ISerializable
    {
        private ITask randomTask;
        List<float> probabilities = new List<float>();

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            if (probabilities.Count == 0)
            {
                randomTask = Children[UnityEngine.Random.Range(0, Children.Count)];
            }
            else if (probabilities.Count == 1)
            {
                randomTask = Children[0];
            }
            else
            {
                for (int i = 1; i < probabilities.Count; i++)
                {
                    probabilities[i] = probabilities[i] + probabilities[i - 1];
                }
                int random = UnityEngine.Random.Range(1, (int)probabilities[probabilities.Count - 1]+1);
                for (int i = 0; randomTask == null && i < probabilities.Count; i++)
                {
                    if (random <= probabilities[i])
                    {
                        if (i < Children.Count)
                        {
                            randomTask = Children[i];
                        }
                        else
                        {
                            randomTask = Children[Children.Count - 1];
                        }
                    }
                }
            }
        }

        public override TaskState Update()
        {
            return randomTask.FullUpdate(executingAgent);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            probabilities = serializer.GetSerializedListFloat("Probabilities");
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Probabilities", probabilities);
        }
    }
}
