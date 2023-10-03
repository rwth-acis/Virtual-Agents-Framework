using i5.Toolkit.Core.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using System.Collections;
using System.Collections.Generic;
using static Codice.Client.Common.WebApi.WebApiEndpoints;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Defines pick up tasks for picking up objects that are near to the agent
    /// Uses the NavMeshAgent component
    /// </summary>
    public class AgentPickUpTask : AgentBaseTask, ISerializable
    {
        /// <summary>
        /// Reference to the NavMeshAgent component
        /// </summary>
        protected NavMeshAgent navMeshAgent;

        /// <summary>
        /// Minimum distance of the agent to the target so that the traget can be picked up
        /// </summary>
        private const float minDistance = 1.20f;


        /// <summary>
        /// Object that should be picked up
        /// </summary>
        public GameObject PickupObject { get; protected set; }

        public AgentPickUpTask()
        {
        }

        /// <summary>
        /// Create an AgentPickUpTask using the object that should be picked up
        /// </summary>
        /// <param name="pickupObject">The object that the agent should pick up</param>
        public AgentPickUpTask(GameObject pickupObject)
        {
            PickupObject = pickupObject;
        }

        /// <summary>
        /// Starts the movement task
        /// </summary>
        /// <param name="agent">The agent which should execute the movement task</param>
        public override void StartExecution(Agent agent)
        {
            base.StartExecution(agent);
            Item item = PickupObject.GetComponent<Item>();
            
            if (item == null)
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
            //Active IK as grab animation
            //Start coroutine to increase IK weight over time
            agent.StartCoroutine(IKWeightIncrease(agent,item));

           
        }
        //Corutine for that increases the weight of the Two Bone IK constraint of Right Arm IK  over time as animation
        public IEnumerator IKWeightIncrease(Agent agent, Item item)
        {
            
            var constraint = agent.GetComponentInChildren<TwoBoneIKConstraint>();
            constraint.data.target.position = item.gripTarget.position;
            constraint.data.target.rotation = item.gripTarget.rotation;
            Debug.Log(constraint);
            constraint.weight = 0;
            item.isPickedUp = true;
            while (constraint.weight < 1)
            {
                constraint.weight += 0.02f;
                yield return new WaitForSeconds(0.01f);

                //Update the target position and rotation of the IK constraint
                constraint.data.target.position = item.gripTarget.position;
                constraint.data.target.rotation = item.gripTarget.rotation;
            }
            pickUpObject(agent, item);
        }
        //Pickup the object and attach it to the agent
        public void pickUpObject(Agent agent, Item item)
        {
            //Add object to mesh socket
            MeshSockets meshSockets = agent.GetComponent<MeshSockets>();
            meshSockets.Attach(PickupObject.transform, MeshSockets.SocketId.RightHand);
            
            //Deactivate IK
            var constraint = agent.GetComponentInChildren<TwoBoneIKConstraint>();
            
            constraint.weight = 0;
            
            Debug.Log("Object was picked up");
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