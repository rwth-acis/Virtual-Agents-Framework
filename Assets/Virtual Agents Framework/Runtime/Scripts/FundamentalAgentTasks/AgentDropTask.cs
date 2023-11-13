using i5.Toolkit.Core.Utilities;
using UnityEngine;
using static MeshSockets;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Defines pick up tasks for picking up objects that are near to the agent
    /// Uses the NavMeshAgent component
    /// </summary>
    public class AgentDropTask : AgentBaseTask, ISerializable
    {


        /// <summary>
        /// Object that should be picked up
        /// </summary>
        public GameObject DropObject { get; protected set; }

        public AgentDropTask()
        {
        }

        /// <summary>
        /// Create an AgentDropTask using the object that is currently carried and should be dropped, if no object is given, the agent will drop all objects that are currently carried
        /// </summary>
        /// <param name="dropObject">The object that the agent should drop</param>
        public AgentDropTask(GameObject dropObject = null)
        {
            DropObject = dropObject;
        }

        /// <summary>
        /// Starts the drop task
        /// </summary>
        /// <param name="agent">The agent which should execute the drop task</param>
        public override void StartExecution(Agent agent)
        {
            base.StartExecution(agent);
            if (DropObject == null)
            {
                DropAllItems(agent, agent.transform);
            }
            else
            {
                if (!DropObject.TryGetComponent<Item>(out var item))
                {
                    State = TaskState.Failure;
                    i5Debug.LogError($"The drop object {DropObject.name} does not have a Item component. " +
                        $"Therefore, it cannot be dropped. Skipping this task.",
                        this);
                }
                DropOneItem(agent, item);
            }

            FinishTask();
        }


        public void DropAllItems(Agent agent, Transform currentTransform)
        {
            // Check if the current transform has the "ItemComponent"
            if (currentTransform.TryGetComponent<Item>(out var item))
            {
                DropOneItem(agent, item);
            }

            // Recursively check each child
            foreach (Transform child in currentTransform)
            {
                DropAllItems(agent, child);
            }
        }
        public void DropOneItem(Agent agent, Item item)
        {
            MeshSockets meshSockets = agent.GetComponent<MeshSockets>();
            meshSockets.Detach(item);
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Drop Object", DropObject);
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            DropObject = serializer.GetSerializedGameobjects("Drop Object");
        }


    }
}