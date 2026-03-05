using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        /// Used to determine if the agent should rotate by a specific angle
        /// </summary>
        public bool IsRotationByAngle { get; protected set; }

        /// <summary>
        /// Used to determine if the agent should rotate towards a specific angle.
        /// </summary>
        public bool IsRotationTowardsAngle { get; protected set; }

        /// <summary>
        /// The angle the agent should rotate by or towards
        /// </summary>
        public float Angle { get; protected set; }

        /// <summary>
        /// The transform the agent should rotate towards
        /// </summary>
        public Transform TargetTransform { get; protected set; }
        
        /// <summary>
        /// The coordinates the agent should rotate towards
        /// </summary>
        public Vector3? TargetPosition { get; protected set; }

        /// <summary>
        /// The angular speed at which the agent should rotate (degrees per second)
        /// </summary>
        public float AngularSpeed { get; protected set; }

        /// <summary>
        /// The angle difference (in degrees) at which the task is considered finished
        /// </summary>
        public float AngleThresholdDeg = 0.5f;

        /// <summary>
        /// Create an AgentRotationTask using a target object to turn towards, position will be evaluated when task is started
        /// </summary>
        /// <param name="target">Target object of the rotation task</param>
        /// <param name="angularSpeed">Angular speed in degrees per second</param>
        public AgentRotationTask(GameObject target, float angularSpeed = 110f)
        {
            TargetTransform = target.transform;
            IsRotationByAngle = false;
            AngularSpeed = angularSpeed;
            TargetPosition = null;
        }

        /// <summary>
        /// Create an AgentRotationTask using the destination coordinates
        /// </summary>
        /// <param name="coordinates">Coordinates of the rotation task</param>
        /// <param name="angularSpeed">Angular speed in degrees per second</param>
        public AgentRotationTask(Vector3 coordinates, float angularSpeed = 110f)
        {
            TargetPosition = coordinates;
            IsRotationByAngle = false;
            AngularSpeed = angularSpeed;
            TargetTransform = null;
        }

        /// <summary>
        /// Create an AgentRotationTask using the angle that the agent should rotate by.
        /// Positive angle turns right, negative angle turns left.
        /// When isRotationByAngle is set to false, the agents rotation attribute will be set to the angle specified instead.
        /// In this case the agent rotates in the direction that minimises the distance.
        /// </summary>
        /// <param name="angle">The angle to rotate by or towards, in degrees</param>
        /// <param name="isRotationByAngle">True if agent should rotate by "angle" degrees, false if the rotation value of the agent should be set to "angle" (default true)</param>
        /// <param name="angularSpeed">Angular speed in degrees per second</param>
        public AgentRotationTask(float angle, bool isRotationByAngle = true, float angularSpeed = 110f)
        {
            IsRotationByAngle = isRotationByAngle;
            if (!isRotationByAngle)
            {
                TargetRotation = Quaternion.Euler(0, angle, 0);
                IsRotationTowardsAngle = true;
            }
            else
            {
                Angle = angle;
            }
            AngularSpeed = angularSpeed;
        }

        /// <summary>
        /// Start the rotation
        /// Called by the agent
        /// </summary>
        /// <param name="agent">The agent which executes this task</param>
        public override void StartExecution(Agent agent)
        {
            base.StartExecution(agent);
            // Calculate target rotation based on task parameters
            if (!IsRotationTowardsAngle && !IsRotationByAngle)
            {
                Vector3 targetPos = TargetPosition ?? TargetTransform.position;
                Vector3 newTargetPosition = new Vector3(targetPos.x, 0, targetPos.z);
                Vector3 newAgentPosition = new Vector3(agent.transform.position.x, 0, agent.transform.position.z);
                float angle = Vector3.SignedAngle(agent.transform.forward, newTargetPosition - newAgentPosition, Vector3.up);
                TargetRotation = agent.transform.rotation * Quaternion.Euler(0, angle, 0);
            }

            if (IsRotationByAngle)
            {
                TargetRotation = agent.transform.rotation * Quaternion.Euler(0, Angle, 0);
            }

            // Delegate execution to the AgentAnimationUpdater
            if (agent.TryGetComponent(out AgentAnimationUpdater updater))
            {
                // Pass FinishTask as an Action delegate so the task resolves when the Coroutine ends
                agent.StartCoroutine(updater.RotateTowardsTarget(TargetRotation, AngularSpeed, AngleThresholdDeg, FinishTask));
            }
            else
            {
                Debug.LogWarning("AgentAnimationUpdater missing. Snapping to target rotation instantly.");
                agent.transform.rotation = TargetRotation;
                FinishTask();
            }
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Target Rotation", TargetRotation);
            serializer.AddSerializedData("Is Rotation By Angle", IsRotationByAngle);
            serializer.AddSerializedData("Angle", Angle);
            serializer.AddSerializedData("Speed", AngularSpeed);
            if (TargetTransform != null) serializer.AddSerializedData("Target Transform", TargetTransform.gameObject);
            if (TargetPosition.HasValue) serializer.AddSerializedData("Target Position", TargetPosition.Value);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            TargetRotation = serializer.GetSerializedQuaternion("Target Rotation");
            IsRotationByAngle = serializer.GetSerializedBool("Is Rotation By Angle");
            Angle = serializer.GetSerializedFloat("Angle");
            AngularSpeed = serializer.GetSerializedFloat("Speed");
            try { TargetTransform = serializer.GetSerializedGameobjects("Target Transform").transform; }
            catch (System.Collections.Generic.KeyNotFoundException) { TargetTransform = null; }
            try { TargetPosition = serializer.GetSerializedVector("Target Position"); }
            catch (System.Collections.Generic.KeyNotFoundException) { TargetPosition = null; }
        }
    }
}
