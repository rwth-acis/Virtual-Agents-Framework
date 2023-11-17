using i5.Toolkit.Core.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static MeshSockets;

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
        private const float minDistance = 1f;

        /// <summary>
        /// Speed of IK animations
        /// </summary>
        private const float moveSpeed = 2f;


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
        public AgentPickUpTask(GameObject pickupObject, SocketId socketId = SocketId.RightHand)
        {
            PickupObject = pickupObject;
            SocketId = socketId;
        }

        /// <summary>
        /// Starts the pickUp task
        /// </summary>
        /// <param name="agent">The agent which should execute the pickUP task</param>
        public override void StartExecution(Agent agent)
        {
            base.StartExecution(agent);

            if (!PickupObject.TryGetComponent<Item>(out var item))
            {
                State = TaskState.Failure;
                i5Debug.LogError($"The pickup object {PickupObject.name} does not have a Item component. " +
                    $"Therefore, it cannot be picket up. Skipping this task.",
                    this);

                FinishTask();
                return;
            }
            if (!item.canBePickedUp)
            {
                State = TaskState.Failure;
                i5Debug.LogError($"The pickup object {PickupObject.name} does not allow the item to be picked up. canBePickedUp = false " +
                    $"Therefore, it cannot be picket up. Skipping this task.",
                    this);

                FinishTask();
                return;
            }

            float distance = Vector3.Distance(agent.transform.position, PickupObject.transform.position);
            if (distance > minDistance)
            {
                State = TaskState.Failure;
                Debug.LogWarning("Object was not close enough for pickup:" + distance + " > " + minDistance);
                FinishTask();
                return;
            }

            //Get the MeshSockets component of the agent for attaching the object and the IK constraints for the arms pickup animation
            this.meshSockets = agent.GetComponent<MeshSockets>();


            //Active IK as grab animation
            //Start coroutine to increase IK weight over time
            agent.StartCoroutine(IKWeightIncrease(agent, item));


        }
        //Corutine for that increases the weight of the Two Bone IK constraint of Right Arm IK  over time as animation
        public IEnumerator IKWeightIncrease(Agent agent, Item item)
        {
            //select witch IK constraint should be used for the pickup, default is the right arm
            if (this.SocketId == SocketId.LeftHand)
            {
                constraint = meshSockets.twoBoneIKConstraintLeftArm;
            }
            else
            {
                //SocketId == SocketId.LeftHand or SocketId == SocketId.Spine
                constraint = meshSockets.twoBoneIKConstraintRightArm;
            }   
            constraint.data.target.SetPositionAndRotation(constraint.data.tip.position, constraint.data.tip.rotation);
            constraint.weight = 1;
            item.SetIsPickedUp(true);

            while (Vector3.Distance(constraint.data.target.position, item.grapTarget.position) > proximityThreshold)
            {

                //calculate direction from which the grapTarget is approached
                Quaternion direction = Quaternion.LookRotation(item.grapTarget.position - constraint.data.tip.position);
                //Ajust direction so that the hand is rotated corectly: formulation was found by testing
                direction = Quaternion.Euler(direction.eulerAngles.x + ((315- direction.eulerAngles.x)*2), direction.eulerAngles.y -180, direction.eulerAngles.z);
   
                //change position and roation of the target smoothly
                constraint.data.target.position = Vector3.Lerp(constraint.data.target.position, item.grapTarget.position, Time.deltaTime * moveSpeed);
                constraint.data.target.rotation = Quaternion.Lerp(constraint.data.target.rotation, direction, Time.deltaTime * moveSpeed);

                yield return null;

                //Update the target position and rotation of the IK constraint
            }
            // Set the target position and rotation to the exact values
            constraint.data.target.SetPositionAndRotation(item.grapTarget.position, item.grapTarget.rotation);
            PickUpObject(agent, item);
        }

        //Pickup the object and attach it to the agent
        public void PickUpObject(Agent agent, Item item)
        {
            //Add object to mesh socket
            meshSockets.Attach(item, SocketId);

            //Deactivate IK
            constraint.weight = 0;

            FinishTask();
        }


        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Pickup Object", PickupObject);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            PickupObject = serializer.GetSerializedGameobjects("Pickup Object");
        }


    }
}