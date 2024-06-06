using i5.Toolkit.Core.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static i5.VirtualAgents.MeshSockets;

namespace i5.VirtualAgents.AgentTasks
{
	/// <summary>
	/// Defines pick up tasks for picking up objects that are near to the agent
	/// Uses the NavMeshAgent component
	/// </summary>
	public class AgentPickUpTask : AgentBaseTask, ISerializable
    {
        /// <summary>
        /// Minimum distance of the agent to the target so that the traget can be picked up
        /// </summary>
        public float minDistanceForPickup = 1f;

        /// <summary>
        /// Speed of IK animations
        /// </summary>
        public float animationSpeed = 2f;


        /// <summary>
        /// How near an IK target has to be to the actual target position to be considered as reached
        /// </summary>
        private float proximityThreshold = 0.05f;

        /// <summary>
        /// Object that should be picked up
        /// </summary>
        public GameObject PickupObject { get; protected set; }

        /// <summary>
        /// Bodypart that the object should attached to
        /// </summary>
        public SocketId SocketId { get; protected set; }

        private MeshSockets meshSockets;
        private TwoBoneIKConstraint constraint;

        public AgentPickUpTask()
        {
        }

        /// <summary>
        /// Create an AgentPickUpTask using the object that should be picked up
        /// </summary>
        /// <param name="pickupObject">The object that the agent should pick up</param>
        /// <param name="socketId">Bodypart that the object should be attached to, standard is the right Hand</param>
        public AgentPickUpTask(GameObject pickupObject, SocketId socketId = SocketId.RightHand, float minDistanceForPickup = 1f, float animationSpeed = 2f)
        {
            PickupObject = pickupObject;
            SocketId = socketId;
            this.minDistanceForPickup = minDistanceForPickup;
            this.animationSpeed = animationSpeed;
        }

        /// <summary>
        /// Starts the pickUp task
        /// </summary>
        /// <param name="agent">The agent which should execute the pickUP task</param>
        public override void StartExecution(Agent agent)
        {
            base.StartExecution(agent);

            if (!PickupObject)
            {
                i5Debug.LogError($"The pickup object is null. " +
                    "Therefore, it cannot be picked up. Skipping this task.",
                    this);
                FinishTaskAsFailed(); 
                return; 
            }
            if (!PickupObject.TryGetComponent<Item>(out var item))
            {
                i5Debug.LogError($"The pickup object {PickupObject.name} does not have a Item component. " +
                    "Therefore, it cannot be picked up. Skipping this task.",
                    this);

                FinishTaskAsFailed();
                return;
            }
            if (!item.CanBePickedUp)
            {
                i5Debug.LogError($"The pickup object {PickupObject.name} does not allow the item to be picked up. canBePickedUp = false " +
                    "Therefore, it cannot be picked up. Skipping this task.",
                    this);

                FinishTaskAsFailed();
                return;
            }

            float distance = Vector3.Distance(agent.transform.position, PickupObject.transform.position);
            if (distance > minDistanceForPickup)
            {
                Debug.LogWarning("Object was not close enough for pickup:" + distance + " > " + minDistanceForPickup);
                FinishTaskAsFailed();
                return;
            }

            // Get the MeshSockets component of the agent for attaching the object and the IK constraints for the arms pickup animation
            this.meshSockets = agent.GetComponent<MeshSockets>();

            // Active IK as grab animation
            // Start coroutine to increase IK weight over time
            agent.StartCoroutine(IKWeightIncrease(agent, item));
        }

        // Coroutine that increases the weight of the Two Bone IK constraint of Right Arm IK over time as animation
        public IEnumerator IKWeightIncrease(Agent agent, Item item)
        {
            // Select witch IK constraint should be used for the pickup, default is the right arm
            if (this.SocketId == SocketId.LeftHand)
            {
                constraint = meshSockets.TwoBoneIKConstraintLeftArm;
            }
            else
            {
                // SocketId == SocketId.LeftHand or SocketId == SocketId.Spine
                constraint = meshSockets.TwoBoneIKConstraintRightArm;
            }
            if(constraint == null)
            {
                Debug.LogError("No TwoBoneIKConstraint found on the meshSockets component ");
                FinishTaskAsFailed();
                yield break;
            }

            if(constraint.data.tip == null || constraint.data.mid == null || constraint.data.root == null)
            {
                // Add correct Root, Mid and Tip to CharacterRig for IK animation
                if (!agent.TryGetComponent<Animator>(out var animator))
                {
                    Debug.LogWarning("Agent has no Animator component.");

                }
                meshSockets.TwoBoneIKConstraintLeftArm.data.root = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                meshSockets.TwoBoneIKConstraintLeftArm.data.mid = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                meshSockets.TwoBoneIKConstraintLeftArm.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftHand);

                meshSockets.TwoBoneIKConstraintRightArm.data.root = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                meshSockets.TwoBoneIKConstraintRightArm.data.mid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                meshSockets.TwoBoneIKConstraintRightArm.data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);
                //TODO: This is a computatioal heavy operation, it would be advisable to not do this during runtime
                RigBuilder rigs = agent.GetComponent<RigBuilder>();
                rigs.Build();
            }
            constraint.data.target.SetPositionAndRotation(constraint.data.tip.position, constraint.data.tip.rotation);
            constraint.weight = 1;
            item.IsPickedUp = true;

            while (Vector3.Distance(constraint.data.target.position, item.GrabTarget.position) > proximityThreshold)
            {

                // Calculate direction from which the grapTarget is approached
                Quaternion direction = Quaternion.LookRotation(item.GrabTarget.position - constraint.data.tip.position);
                // Ajust direction so that the hand is rotated corectly: formulation was found by testing
                direction = Quaternion.Euler(direction.eulerAngles.x + ((315- direction.eulerAngles.x)*2), direction.eulerAngles.y -180, direction.eulerAngles.z);
   
                // Change position and roation of the target smoothly
                constraint.data.target.position = Vector3.Lerp(constraint.data.target.position, item.GrabTarget.position, Time.deltaTime * animationSpeed);
                constraint.data.target.rotation = Quaternion.Lerp(constraint.data.target.rotation, direction, Time.deltaTime * animationSpeed);

                yield return null;

                // Update the target position and rotation of the IK constraint
            }
            // Set the target position and rotation to the exact values
            constraint.data.target.SetPositionAndRotation(item.GrabTarget.position, item.GrabTarget.rotation);
            PickUpObject(agent, item);
        }

        // Pickup the object and attach it to the agent
        public void PickUpObject(Agent agent, Item item)
        {
            // Add object to mesh socket
            meshSockets.Attach(item, SocketId);

            // Deactivate IK
            constraint.weight = 0;

            FinishTask();
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Pickup Object", PickupObject);
            serializer.AddSerializedData("SocketId", (int) SocketId);
            serializer.AddSerializedData("Min Distance For Pickup", minDistanceForPickup);
            serializer.AddSerializedData("Animation Speed", animationSpeed);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            PickupObject = serializer.GetSerializedGameobjects("Pickup Object");
            SocketId = (SocketId) serializer.GetSerializedInt("SocketId");
            minDistanceForPickup = serializer.GetSerializedFloat("Min Distance For Pickup");
            animationSpeed = serializer.GetSerializedFloat("Animation Speed");
        }
    }
}