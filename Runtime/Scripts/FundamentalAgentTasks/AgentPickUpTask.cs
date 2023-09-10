using i5.Toolkit.Core.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

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
        private const float minDistance = 1.00f;


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
                i5Debug.LogError($"The puckup object {agent.name} does not have a Item component. " +
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
            //Pickup the object
            PickupObject.transform.SetParent(agent.transform);
            //PickupObject.transform.localPosition = Vector3.zero;
            item.isPickedUp = true;
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