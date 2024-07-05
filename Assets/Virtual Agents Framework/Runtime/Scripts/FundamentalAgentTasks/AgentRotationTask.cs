using System.Collections;
using UnityEngine;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Defines rotation tasks for rotating the agent to a specific direction.
    /// The direction can be given as a target, coordinates or angle.
    /// </summary>
    public class AgentRotationTask : AgentBaseTask, ISerializable
    {
        /// <summary>
        /// Coordinates of the rotation task
        /// </summary>
        public Vector3 Coordinates { get; protected set; }


        /// <summary>
        /// Create an AgentRotationTask using a target object to turn towards
        /// </summary>
        /// <param name="target">Target object of the rotation task</param>
        public AgentRotationTask(GameObject target)
        {
            Coordinates = target.transform.position;
        }

        /// <summary>
        /// Create an AgentRotationTask using the destination coordinates
        /// </summary>
        /// <param name="coordinates">Coordinates of the rotation task</param>
        public AgentRotationTask(Vector3 coordinates)
        {
            Coordinates = coordinates;
        }

        /// <summary>
        /// Create an AgentRotationTask using the angle that the agent should rotate to
        /// </summary>
        /// <param name="angle">The angle to rotate to, in degrees</param>
        public AgentRotationTask(float angle)
        {
            Coordinates = new Vector3(0, angle, 0);
        }

        /// <summary>
        /// Start the rotation
        /// Called by the agent
        /// </summary>
        /// <param name="agent">The agent which executes this task</param>
        public override void StartExecution(Agent agent)
        {
            base.StartExecution(agent);
            Debug.Log("Rotation started");
            Quaternion coordinatesRotation = Quaternion.Euler(Coordinates);
            agent.StartCoroutine(Rotate(agent.transform, coordinatesRotation, 20f));
        }

        private IEnumerator Rotate(Transform transform, Quaternion targetRotation, float rotationSpeed)
        {
            float time = 0;
            while (time <= 1f)
            {
                time += Time.deltaTime/rotationSpeed; //to control the speed of rotation
                // Rotate the agent a step closer to the target
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, time);
                // Wait for the next frame
                yield return null;
            }
            if (transform.rotation.eulerAngles == targetRotation.eulerAngles)
            {
                FinishTask();
                Debug.Log("Rotation finished");
            }
            else
            {
                FinishTaskAsFailed();
            }
        }

        /// <summary>
        /// Finish the task
        /// </summary>
        public override void StopExecution()
        {
            //TODO: Implement
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            //TODO: Implement
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            //TODO: Implement
        }
    }
}