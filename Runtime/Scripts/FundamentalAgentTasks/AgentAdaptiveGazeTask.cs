using System.Collections;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Starts adaptive gaze on the agent for a given time and then stops it. This task is meant to be used when the adavtive gaze should be used in a sequence of tasks.
    /// </summary>
    public class AgentAdaptiveGazeTask : AgentBaseTask, ISerializable
    {
        private float playTime;

        AdaptiveGaze adaptiveGazeScript;
        public AgentAdaptiveGazeTask() { }

        public AgentAdaptiveGazeTask(float playTime)
        {
            this.playTime = playTime;
        }

        /// <summary>
        /// Starts the execution of the task; starts the animation
        /// </summary>
        /// <param name="agent">The agent on which the task is executed</param>
        public override void StartExecution(Agent agent)
        {
            //Check if there is an and adaptiveGaze component, if not add one to the agent
            if (!agent.TryGetComponent<AdaptiveGaze>(out adaptiveGazeScript))
            {
                Debug.Log("No AdaptiveGaze component found, adding one. It is recommended to add one to the agent in the inspector.");
                adaptiveGazeScript = agent.gameObject.AddComponent<AdaptiveGaze>();
            }
            else
            {
                adaptiveGazeScript = agent.GetComponent<AdaptiveGaze>();
            }

            adaptiveGazeScript.Activate();
            agent.StartCoroutine(Wait(playTime));
        }

        /// <summary>
        /// Stops the animation
        /// </summary>
        public override void StopExecution()
        {
            adaptiveGazeScript.Deactivate();
        }

        // wait for the given time and then finish the task
        private IEnumerator Wait(float timeInSeconds)
        {
            yield return new WaitForSeconds(timeInSeconds);
            FinishTask();
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Play Time", playTime);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            playTime = serializer.GetSerializedFloat("Play Time");
        }

    }
}