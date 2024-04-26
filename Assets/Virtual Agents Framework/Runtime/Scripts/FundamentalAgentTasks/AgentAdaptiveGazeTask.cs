using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Starts or stops adaptive gaze on the agent and marks the task as completet afterwards. 
    /// </summary>
    public class AgentAdaptiveGazeTask : AgentBaseTask, ISerializable
    {

        private readonly bool shouldStartOrStop;

        AdaptiveGaze adaptiveGazeScript;
        public AgentAdaptiveGazeTask() { }
        /// <summary>
        /// Constructor for the adaptive gaze task
        /// </summary>
        /// <param name="shouldStartOrStop">If true, will start adaptive Gaze. If false will stop adaptive gaze</param>
        public AgentAdaptiveGazeTask(bool shouldStartOrStop)
        {
            this.shouldStartOrStop = shouldStartOrStop;
        }

        /// <summary>
        /// Starts the execution of the task; starts or stops the adaptive gaze
        /// </summary>
        /// <param name="agent">The agent on which the task is executed</param>
        public override void StartExecution(Agent agent)
        {   
            //Check if there is an and adaptiveGaze component, if not add one to the agent
            if (!agent.TryGetComponent<AdaptiveGaze>(out adaptiveGazeScript))
            {
                if (shouldStartOrStop == true)
                {
                    Debug.Log("No AdaptiveGaze component found, adding one. It is recommended to add one to the agent in the inspector.");
                    adaptiveGazeScript = agent.gameObject.AddComponent<AdaptiveGaze>();
                }

            }
            else
            {
                adaptiveGazeScript = agent.GetComponent<AdaptiveGaze>();
            }
            //Start or stop the adaptive gaze
            if (shouldStartOrStop == true)
            {
                adaptiveGazeScript.Activate();
            }
            else
            {
                adaptiveGazeScript.Deactivate();
            }
            FinishTask();
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            //TODO: add shouldStartOrStop when bool types are supported
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            //TODO: add shouldStartOrStop when bool types are supported
        }
    }
}