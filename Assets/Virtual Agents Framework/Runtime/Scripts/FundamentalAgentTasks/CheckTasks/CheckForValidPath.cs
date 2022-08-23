using UnityEngine;
using UnityEngine.AI;
using System;

namespace i5.VirtualAgents.AgentTasks
{
    /// <summary>
    /// Succeeds if there is a NavMesh path from the agent to the target
    /// </summary>
    public class CheckForValidPath : CheckBaseTask, ISerializable
    {
        private GameObject destination;
        private NavMeshAgent navMeshAgent;
        private bool allowPartialPaths;


        public override void Execute(Agent executingAgent)
        {
            navMeshAgent = executingAgent.GetComponent<NavMeshAgent>();
            base.Execute(executingAgent);
        }

        /// <summary>
        /// Checks if there is a NavMesh path from the agent to the target
        /// </summary>
        /// <returns></returns>
        protected override TaskState Check()
        {
            navMeshAgent.isStopped = true;
            if (navMeshAgent.destination != destination.transform.position)
            {
                navMeshAgent.SetDestination(destination.transform.position);
            }

            if (navMeshAgent.pathPending)
            {
                // NavMeshAgent needs more time to generate a path
                return TaskState.Running;
            }

            switch (navMeshAgent.pathStatus)
            {
                case NavMeshPathStatus.PathInvalid:
                    return TaskState.Failure;
                case NavMeshPathStatus.PathPartial:
                    return allowPartialPaths ? TaskState.Success : TaskState.Failure;
                case NavMeshPathStatus.PathComplete:
                    return TaskState.Success;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            destination = serializer.GetSerializedGameobjects("Destination");
            allowPartialPaths = serializer.GetSerializedBool("Allow Partial Paths");
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Destination",destination);
            serializer.AddSerializedData("Allow Partial Paths",allowPartialPaths);
        }
    }
}
