using i5.Toolkit.Core.Utilities;
using System.Collections;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Makes the agent wait for a given amount of time
    /// </summary>
    public class AgentWaitTask : AgentBaseTask, ISerializable
    {
        /// <summary>
        /// The number of seconds that the agent should wait for
        /// </summary>
        public float WaitTimeInSeconds { get; set; }

        private Agent agent;

		private float startTime;

        public AgentWaitTask() { }

        /// <summary>
        /// Creates a new instance of the wait task
        /// </summary>
        /// <param name="timeInSeconds">The number of secondsd that the agent should wait for</param>
        public AgentWaitTask(float timeInSeconds)
        {
            WaitTimeInSeconds = timeInSeconds;
        }

        public override void StartExecution(Agent executingAgent)
        {
            base.StartExecution(executingAgent);
            startTime = Time.realtimeSinceStartup;
        }

        public override TaskState EvaluateTaskState()
        {
            if(Time.realtimeSinceStartup - startTime > WaitTimeInSeconds)
			{
				return TaskState.Success;
			}
			else
			{
				return TaskState.Running;
			}
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Wait time", WaitTimeInSeconds);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            WaitTimeInSeconds = serializer.GetSerializedFloat("Wait time");
        }

    }
}
