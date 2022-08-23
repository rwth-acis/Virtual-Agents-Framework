using i5.VirtualAgents.AgentTasks;
using System.Collections;
using UnityEngine;

namespace i5.VirtualAgents.BehaviourTrees
{
    /// <summary>
    /// If its child doesn't finish in timeOutInSeconds, it fails
    /// </summary>
    public class TimeOutNode : DecoratorNode, ISerializable
    {
        private float timeOutInSeconds;
        private Coroutine timeOutRoutine;


        public override TaskState Update()
        {
            TaskState state = Child.FullUpdate(executingAgent);
            if (state == TaskState.Success || state == TaskState.Failure)
            {
                // Task finished faster than the timeout time, stop routine to prevent the task from being stopped multiple times
                executingAgent.StopCoroutine(timeOutRoutine);
            }
            return state;
        }

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            if (timeOutRoutine != null)
            {
                executingAgent.StopCoroutine(timeOutRoutine);
            }
            timeOutRoutine = executingAgent.StartCoroutine(TimeOut(timeOutInSeconds));
        }

        // Wait for the given time and then fail the task
        private IEnumerator TimeOut(float timeInSeconds)
        {
            yield return new WaitForSeconds(timeInSeconds);
            PreemptivelyFailTask();
        }

        public void Deserialize(SerializationDataContainer serializer)
        {
            timeOutInSeconds = serializer.GetSerializedFloat("Time Out in Seconds");
        }

        public void Serialize(SerializationDataContainer serializer)
        {
            serializer.AddSerializedData("Time Out in Seconds", timeOutInSeconds);
        }
    }
}
