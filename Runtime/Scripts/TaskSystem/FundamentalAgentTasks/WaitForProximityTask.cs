using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    public class WaitForProximityTask : AgentBaseTask, ISerializable
    {
        private GameObject target;
        private float distance;
        private Agent executingAgent;

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            this.executingAgent = executingAgent;
        }

        public override TaskState Update()
        {
            if (Vector3.Distance(executingAgent.transform.position, target.transform.position) < distance)
            {
                return TaskState.Success;
            }
            return TaskState.Running;
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            target = serializer.GetSerializedGameobjects("Target");
            distance = serializer.GetSerializedFloat("Distance");
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Target", target);
            serializer.AddSerializedData("Distance",distance);
        }
    }
}
