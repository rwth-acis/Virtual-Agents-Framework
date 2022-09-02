using UnityEngine;
namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Succeeds if the target is less than distance units away from the executing agent, otherwise fails
    /// </summary>
    public class CheckForProximityTask : CheckBaseTask, ISerializable
    {
        private GameObject target;
        private float distance;

        /// <summary>
        /// Checks if the target is less than distance units away from the agent
        /// </summary>
        /// <returns></returns>
        protected override TaskState Check()
        {
            return Vector3.Distance(executingAgent.transform.position, target.transform.position) <= distance? TaskState.Success : TaskState.Failure;
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
