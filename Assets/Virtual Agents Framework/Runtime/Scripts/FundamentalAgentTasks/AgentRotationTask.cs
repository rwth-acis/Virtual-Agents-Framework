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

        public override void StartExecution(Agent agent)
        {
            //TODO: Implement
            Debug.Log("Rotation started");
            base.StartExecution(agent);
            float rotationSpeed = 0.2f;
            float step = rotationSpeed * Time.deltaTime;

            agent.transform.Rotate(Coordinates, relativeTo: Space.World);
            Quaternion targetRotation = Quaternion.Euler(Coordinates);

            // Rotate our transform a step closer to the target's.
            agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, step);

            if( agent.transform.rotation.eulerAngles != Coordinates)
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
        /// Checks every frame whether the agent has finished the rotatation
        /// </summary>
        public override TaskState EvaluateTaskState()
        {
            //TODO: Implement
           return TaskState.Running;
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