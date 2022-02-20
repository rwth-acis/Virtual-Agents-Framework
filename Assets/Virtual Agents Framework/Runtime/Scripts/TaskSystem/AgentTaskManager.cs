using System;

namespace i5.VirtualAgents.TaskSystem
{
    public class AgentTaskManager
    {
        public Agent ExecutingAgent { get; private set; }

        // task queue of this manager
        private AgentTaskQueue queue = new AgentTaskQueue();

        private TaskState currentState;

        public event Action OnStateChanged;
        public event Action OnTaskFinished;

        /// <summary>
        /// Agent's current task
        /// </summary>
        public IAgentTask CurrentTask { get; private set; }

        /// <summary>
        /// Agent's current state
        /// </summary>
        public TaskState CurrentState
        {
            get => currentState;
            private set
            {
                currentState = value;
                OnStateChanged?.Invoke();
            }
        }

        public AgentTaskManager() : this(null)
        {
            // Make the agent start in the idle state in order to enable requesting new tasks
            // CHANGE_ME to inactive in order to disable requesting new tasks
            currentState = TaskState.idle;
        }

        public AgentTaskManager(Agent agent)
        {
            AssociateAgent(agent);
        }

        public void AssociateAgent(Agent agent)
        {
            ExecutingAgent = agent;

            if (ExecutingAgent == null)
            {
                CurrentState = TaskState.inactive;
            }
            else
            {
                CurrentState = TaskState.idle;
            }
        }

        /// <summary>
        /// Enable the right mode depending on the agent's status
        /// </summary>
        public void Update()
        {
            switch (CurrentState)
            {
                case TaskState.inactive: // do nothing
                    break;
                case TaskState.idle:
                    RequestNextTask(); // request new tasks
                    break;
                case TaskState.busy:
                    CurrentTask.Update(); // perform frame-to-frame updates required by the current task
                    break;
            }
        }

        public void ForceTask(IAgentTask task)
        {
            queue.AddAtFront(task);
        }

        public void ScheduleTask(IAgentTask task)
        {
            queue.AddAtBack(task);
        }

        private void RequestNextTask()
        {
            IAgentTask nextTask = queue.RequestNextTask();
            if (nextTask == null)
            {
                // The queue is empty, thus change the agent's current state to idle
                CurrentState = TaskState.idle;
            }
            else
            {
                // The queue is not empty, thus...
                // change the agent's current state to busy,
                CurrentState = TaskState.busy;
                // save the current task,
                CurrentTask = nextTask;
                // subscribe to the task's OnTaskFinished event to set the agent's state to idle after task execution
                CurrentTask.OnTaskFinished += TaskFinished;
                // execute the next task,
                nextTask.Execute(ExecutingAgent);
            }
        }

        /// <summary>
        /// Helper function to be called when a task has been executed.
        /// Set agent's state to idle and unsubscribe from the current task's OnTaskFinished event
        /// </summary>
        private void TaskFinished()
        {
            CurrentState = TaskState.idle;
            // Unsubscribe from the event
            CurrentTask.OnTaskFinished -= TaskFinished;
            OnTaskFinished?.Invoke();
        }
    }
}
