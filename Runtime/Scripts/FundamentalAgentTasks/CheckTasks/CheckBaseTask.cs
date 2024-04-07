namespace i5.VirtualAgents.AgentTasks
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

        public override void StartExecution(Agent executingAgent)
        {
            base.StartExecution(executingAgent);
            switch (Check())
            {
                case TaskState.Success:
                    StopAsSucceeded();
                    break;
                case TaskState.Failure:
                    StopAsFailed();
                    break;
            }
        }

        public override TaskState EvaluateTaskState()
        {
            // Check is still pending, repeate it.
            return Check();
        }
    }
}
