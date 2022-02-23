using i5.Toolkit.Core.Utilities;
using System.Collections;
using UnityEngine;

namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    public class AgentWaitTask : AgentBaseTask
    {
        public float WaitTimeInSeconds { get; set; }

        public AgentWaitTask(float timeInSeconds)
        {
            WaitTimeInSeconds = timeInSeconds;
        }

        /// <summary>
        /// Start the waiting task
        /// Called by the agent
        /// </summary>
        /// <param name="agent">The agent which executes this task</param>
        public override void Execute(Agent agent)
        {
            base.Execute(agent);

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
                FinishTask();
                return;
            }

            agent.StartCoroutine(Wait(WaitTimeInSeconds));
        }

        // wait for the given time and then finish the task
        private IEnumerator Wait(float timeInSeconds)
        {
            yield return new WaitForSeconds(timeInSeconds);
            FinishTask();
        }
    }
}