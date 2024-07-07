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
        /// The rotation as a quaternion which the agent should rotate to
        /// </summary>
        public Quaternion TargetRotation { get; protected set; }


        /// <summary>
        /// Create an AgentRotationTask using a target object to turn towards
        /// </summary>
        /// <param name="target">Target object of the rotation task</param>
        public AgentRotationTask(GameObject target)
        {
            TargetRotation = Quaternion.LookRotation(target.transform.position);
        }

        /// <summary>
        /// Create an AgentRotationTask using the destination coordinates
        /// </summary>
        /// <param name="coordinates">Coordinates of the rotation task</param>
        public AgentRotationTask(Vector3 coordinates)
        {
            TargetRotation = Quaternion.LookRotation(coordinates);
        }

        /// <summary>
        /// Create an AgentRotationTask using the angle that the agent should rotate to
        /// </summary>
        /// <param name="angle">The angle to rotate to, in degrees</param>
        public AgentRotationTask(float angle)
        {
            TargetRotation = Quaternion.Euler(0, angle, 0);
        }

        /// <summary>
        /// Start the rotation
        /// Called by the agent
        /// </summary>
        /// <param name="agent">The agent which executes this task</param>
        public override void StartExecution(Agent agent)
        {
            Animator animator = agent.GetComponent<Animator>();
            base.StartExecution(agent);
            Debug.Log("Rotation started");
            animator.Play("Rotate Right", -1, 0);
            agent.StartCoroutine(Rotate(agent.transform, 10f));
        }

        private IEnumerator Rotate(Transform transform, float rotationSpeed)
        {
            float time = 0;
            while (time <= 1f)
            {
                time += Time.deltaTime/rotationSpeed; //to control the speed of rotation
                // Rotate the agent a step closer to the target
                transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, time);
                // Wait for the next frame
                yield return null;
            }
            if (transform.rotation.eulerAngles == TargetRotation.eulerAngles)
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