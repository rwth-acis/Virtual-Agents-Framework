using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace i5.VirtualAgents.TaskSystem.AgentTasks
{
    public class CheckBaseTask : AgentBaseTask
    {

        /// <summary>
        /// Check that is performed on execute. If check succeeds/fails, the task succeds/fails.
        /// If the check can't be completed in one frame, it can return TaskState.Running or TaskState.Waiting to be repeated on the next frame.
        /// </summary>
        /// <returns></returns>
        protected virtual TaskState Check()
        {
            return TaskState.Success;
        }

        public override void Execute(Agent executingAgent)
        {
            base.Execute(executingAgent);
            switch (Check())
            {
                case TaskState.Success:
                    PreemptivelySuccedTask();
                    break;
                case TaskState.Failure:
                    PreemptivelyFailTask();
                    break;
            }
        }

        public override TaskState Update()
        {
            // Check is still pending, repeate it.
            return Check();
        }
    }
}
