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
                dropAllItems(agent, agent.transform);
                Debug.Log("Objects were droped");
            }
            else
            {
                Item item = DropObject.GetComponent<Item>();
                if (item == null)
                {
                    State = TaskState.Failure;
                    i5Debug.LogError($"The drop object {DropObject.name} does not have a Item component. " +
                        $"Therefore, it cannot be dropped. Skipping this task.",
                        this);
                }
                dropOneItem(agent, item);
                Debug.Log("Object was droped");
            }

            FinishTask();
        }


        public void dropAllItems(Agent agent, Transform currentTransform)
        {
            // Check if the current transform has the "ItemComponent"
            Item item = currentTransform.GetComponent<Item>();

            if (item != null)
            {
                dropOneItem(agent, item);
            }

            // Recursively check each child
            foreach (Transform child in currentTransform)
            {
                dropAllItems(agent, child);
            }
        }
        public void dropOneItem(Agent agent, Item item)
        {
            item.transform.SetParent(null, true);
            item.setIsPickedUp(false);
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