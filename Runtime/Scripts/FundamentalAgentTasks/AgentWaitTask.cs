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

        private Coroutine waitCoroutine;

        private Agent agent;

        public AgentWaitTask() { }

        /// <summary>
        /// Creates a new instance of the wait task
        /// </summary>
        /// <param name="timeInSeconds">The number of secondsd that the agent should wait for</param>
        public AgentWaitTask(float timeInSeconds)
        {
            WaitTimeInSeconds = timeInSeconds;
        }

        /// <summary>
        /// Start the waiting task
        /// Called by the agent
        /// </summary>
        /// <param name="agent">The agent which executes this task</param>
        public override void StartExecution(Agent agent)
        {
            this.agent = agent;
            base.StartExecution(agent);

            if (WaitTimeInSeconds <= 0)
            {
                if (WaitTimeInSeconds == 0)
                {
                    i5Debug.LogWarning("Skipping waiting task as its wait time is set to 0 seconds.", this);
                }
                else
                {
                    i5Debug.LogWarning($"Skipping waiting task as it was provided with a negative time of {WaitTimeInSeconds} seconds given.", this);
                }
                FinishTaskAsFailed();
                return;
            }
            waitCoroutine = agent.StartCoroutine(Wait(WaitTimeInSeconds));
        }

        // wait for the given time and then finish the task
        private IEnumerator Wait(float timeInSeconds)
        {
            yield return new WaitForSeconds(timeInSeconds);
            FinishTask();
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Wait time", WaitTimeInSeconds);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            WaitTimeInSeconds = serializer.GetSerializedFloat("Wait time");
        }

        /// <summary>
        /// Aborts the wait task
        /// </summary>
        public override void Abort()
        {
            if (waitCoroutine != null)
            {
                agent.StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
            State = TaskState.Aborted;
        }
    }
}
