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
        /// Used to determine if the agent should rotate by a specific angle or towards a specific angle.
        /// </summary>
        public bool IsRotationByAngle{ get; protected set; }

        /// <summary>
        /// The angle the agent should rotate by or towards
        /// </summary>
        public float Angle { get; protected set; }


        /// <summary>
        /// Create an AgentRotationTask using a target object to turn towards
        /// </summary>
        /// <param name="target">Target object of the rotation task</param>
        public AgentRotationTask(GameObject target)
        {
            Vector3 position = target.transform.position;
            position.y = 0;
            TargetRotation = Quaternion.LookRotation(position);
            IsRotationByAngle = false;
        }

        /// <summary>
        /// Create an AgentRotationTask using the destination coordinates
        /// </summary>
        /// <param name="coordinates">Coordinates of the rotation task</param>
        public AgentRotationTask(Vector3 coordinates)
        {
            coordinates.y = 0;
            TargetRotation = Quaternion.LookRotation(coordinates);
            IsRotationByAngle = false;
        }

        /// <summary>
        /// Create an AgentRotationTask using the angle that the agent should rotate by.
        /// Positive angle turns right, negative angle turns left.
        /// When isRotationByAngle is set to false, the agents rotation attribute will be set to the angle specified instead.
        /// In this case the agent rotates in the direction that minimises the distance.
        /// </summary>
        /// <param name="angle">The angle to rotate by or towards, in degrees</param>
        /// <param name="isRotationByAngle">True if agent should rotate by "angle" degrees, false if the rotation value of the agent should be set to "angle"</param>
        public AgentRotationTask(float angle, bool isRotationByAngle = true)
        {
            IsRotationByAngle = isRotationByAngle;
            if (!isRotationByAngle)
            {
                TargetRotation = Quaternion.Euler(0, angle, 0);
            }
            else
            {
                Angle = angle;
            }
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
            //For Angle rotation
            if(IsRotationByAngle)
            {
                TargetRotation = agent.transform.rotation * Quaternion.Euler(0, Angle, 0);
            }
            Debug.Log("Rotation started");
            agent.StartCoroutine(Rotate(agent.transform, 10f));
        }

        private IEnumerator Rotate(Transform transform, float rotationSpeed)
        {
            float time = 0;
            while (Vector3.Distance(transform.rotation.eulerAngles, TargetRotation.eulerAngles) > 0.01f)
            {
                time += Time.deltaTime/rotationSpeed; //to control the speed of rotation
                // Rotate the agent a step closer to the target
                transform.rotation = Quaternion.Slerp(transform.rotation, TargetRotation, time);
                //Debug.Log("Rotating...");
                // Wait for the next frame
                yield return null;
            }
            //if (transform.rotation.eulerAngles == TargetRotation.eulerAngles)
            //if (Vector3.Distance(transform.rotation.eulerAngles, TargetRotation.eulerAngles) <= 0.01f)
            if (Quaternion.Angle(transform.rotation, TargetRotation) <= 0.01f)
            {
                FinishTask();
                Debug.Log("Rotation finished");
            }
            else
            {
                FinishTaskAsFailed();
            }
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            //serializer.AddSerializedData("Target Rotation", TargetRotation);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            //TODO: Implement
        }
    }
}