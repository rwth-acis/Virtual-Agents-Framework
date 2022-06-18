using i5.Toolkit.Core.Utilities;
using System;

namespace i5.VirtualAgents.TaskSystem
{
    public class AgentTaskManager
    {
        /// <summary>
        /// Agent which will execute the scheduled tasks
        /// </summary>
        public Agent ExecutingAgent { get; private set; }

        // task queue of this manager
        private AgentTaskQueue queue = new AgentTaskQueue();

        private TaskState currentState;

        public bool IsActive
        {
            get => currentState != TaskState.inactive;
            set
            {
                if (value)
                {
                    if (CurrentTask != null)
                    {
                        CurrentState = TaskState.busy;
                    }
                    else
                    {
                        CurrentState = TaskState.idle;
                    }
                }
                else
                {
                    CurrentState = TaskState.inactive;
                }
            }
        }

        /// <summary>
        /// Event which is raised once the agent's state changes
        /// </summary>
        public event Action OnStateChanged;

        public delegate void TaskFinishedEvent(AgentTaskManager sender, IAgentTask finishedTask);
        /// <summary>
        /// Event which is raised once the agent has finished the current task
        /// </summary>
        public event TaskFinishedEvent OnTaskFinished;

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
                bool invokeEvent = currentState != value;
                currentState = value;
                if (invokeEvent)
                {
                    OnStateChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Creates a new task manager but does not yet associate an agent with it
        /// If you use this method, you need to call the AssociateAgent method at some point before scheduled tasks can be executed
        /// </summary>
        public AgentTaskManager() : this(null)
        {
            // Make the agent start in the idle state in order to enable requesting new tasks
            // CHANGE_ME to inactive in order to disable requesting new tasks
            currentState = TaskState.idle;
        }

        /// <summary>
        /// Creates a new task manager and associates with an agent
        /// </summary>
        /// <param name="agent">The agent on which scheduled tasks should be executed</param>
        public AgentTaskManager(Agent agent)
        {
            AssociateAgent(agent);
        }

        /// <summary>
        /// Associates an agent with the task manager
        /// Scheduled tasks can only run if an agent was registered with the task manager, either using this method or the constructor which takes an agent as an argument
        /// </summary>
        /// <param name="agent">The agent which should execute the scheduled tasks</param>
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
                case TaskState.waiting:
                case TaskState.idle:
                    RequestNextTask(); // request new tasks
                    break;
                case TaskState.busy:
                    CurrentTask.Update(); // perform frame-to-frame updates required by the current task
                    break;
            }
        }

        /// <summary>
        /// Schedules a task in the queue, sorted by the given priority
        /// </summary>
        /// <param name="task">The task that should be scheduled for execution</param>
        /// <param name="priority">Priority of the task. Tasks with high importance should get a positive value, less important tasks a negative value. Default tasks have a priority of 0.</param>
        public void ScheduleTask(IAgentTask task, int priority = 0)
        {
            queue.AddTask(task, priority);
        }

        // get the next task from the queue and adapts the states accordingly
        private void RequestNextTask()
        {
            IAgentTask nextTask = queue.PeekNextTask();
            if (nextTask == null)
            {
                // The queue is empty, thus change the agent's current state to idle
                CurrentState = TaskState.idle;
            }
            else if (!nextTask.CanStart)
            {
                CurrentState = TaskState.waiting;
            }
            else
            {
                // now actually retrieve the task from the queue
                nextTask = queue.RequestNextTask();
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
        /// Peeks at the next task that the task manager will execute after the current one
        /// </summary>
        /// <returns>Returns the next task to execute, null if no task is upcoming</returns>
        public IAgentTask PeekNextTask()
        {
            return queue.PeekNextTask();
        }

        /// <summary>
        /// Helper function to be called when a task has been executed.
        /// Set agent's state to idle and unsubscribe from the current task's OnTaskFinished event
        /// </summary>
        private void TaskFinished()
        {
            // Unsubscribe from the event
            CurrentTask.OnTaskFinished -= TaskFinished;
            CurrentState = TaskState.idle;
            IAgentTask previousTask = CurrentTask;
            CurrentTask = null;
            OnTaskFinished?.Invoke(this, previousTask);
        }
    }
}
