using i5.VirtualAgents.AgentTasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// Executes one of its child tasks at random
    /// </summary>
    public class RandomNode : CompositeNode, ISerializable
    {
        private ITask randomTask;
        List<float> probabilities = new List<float>();

        public override void StartExecution(Agent executingAgent)
        {
            base.StartExecution(executingAgent);

            CorrectProbabilitiesForDifferentChildAmount();
            NormalizeProbabilities();
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);

            if (probabilities.Count == 0)
            {
                randomTask = Children[UnityEngine.Random.Range(0, Children.Count)];
            }
            else
            {
                float[] cumulativeProbabilities = new float[probabilities.Count];
                cumulativeProbabilities[0] = probabilities[0];
                // Calculate cumulative probabilities
                for (int i = 1; i < probabilities.Count; i++)
                {
                    cumulativeProbabilities[i] = cumulativeProbabilities[i - 1] + probabilities[i];
                }

                float random = UnityEngine.Random.Range(0, 1000) / 1000f;
                for (int i = 0; i < probabilities.Count; i++)
                {
                    if (random < cumulativeProbabilities[i])
                    {
                        randomTask = Children[i];
                        break;
                    }
                }

            }
        }
        // Normalize the probabilities so they add up to 1
        private void NormalizeProbabilities()
        {
            float sum = 0;
            foreach (float prob in probabilities)
            {
                sum += prob;
            }
            for (int i = 0; i < probabilities.Count; i++)
            {
                probabilities[i] = probabilities[i] / sum;
            }
        }
        // Add or remov probabilties if the amount of children changes
        private void CorrectProbabilitiesForDifferentChildAmount()
        {
            if(probabilities.Count == 0)
            {
                return;
            }
            if(probabilities.Count == Children.Count)
            {
                return;
            }
            while(probabilities.Count < Children.Count)
            {
                probabilities.Add(probabilities[0]);
            }
            while(probabilities.Count > Children.Count)
            {
                probabilities.RemoveAt(probabilities.Count - 1);
            }


        }

        

        public override TaskState EvaluateTaskState()
        {
            return randomTask.Tick(executingAgent);
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
