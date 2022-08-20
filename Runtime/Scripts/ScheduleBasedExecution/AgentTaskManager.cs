using i5.Toolkit.Core.Utilities;
using System;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.ScheduleBasedExecution
{
    public class AgentTaskManager
    {
        /// <summary>
        /// Agent which will execute the scheduled tasks
        /// </summary>
        public Agent ExecutingAgent { get; private set; }

        // task queue of this manager
        private AgentTaskQueue queue = new AgentTaskQueue();

        // the stat of the task manager
        private TaskManagerState currentState;

        /// <summary>
        /// Checks whether the task manager is active or has been deactivated
        /// Only returns false if the task manager has explicitly been deactivated
        /// </summary>
        public bool IsActive
        {
            get => currentState != TaskManagerState.inactive;
            set
            {
                if (value)
                {
                    if (CurrentTask != null)
                    {
                        CurrentState = TaskManagerState.busy;
                    }
                    else
                    {
                        CurrentState = TaskManagerState.idle;
                    }
                }
                else
                {
                    CurrentState = TaskManagerState.inactive;
                }
            }
        }

        /// <summary>
        /// Event which is raised once the agent's state changes
        /// </summary>
        public event Action OnStateChanged;

        /// <summary>
        /// Event handler for finished tasks
        /// </summary>
        /// <param name="sender">The task manager on which the task finished</param>
        /// <param name="finishedTask">The task that finished</param>
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
        public TaskManagerState CurrentState
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
            currentState = TaskManagerState.idle;
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
                CurrentState = TaskManagerState.inactive;
            }
            else
            {
                CurrentState = TaskManagerState.idle;
            }
        }

        /// <summary>
        /// Enable the right mode depending on the agent's status
        /// </summary>
        public void Update()
        {
            switch (CurrentState)
            {
                case TaskManagerState.inactive: // do nothing
                    break;
                case TaskManagerState.waiting:
                case TaskManagerState.idle:
                    RequestNextTask(); // request new tasks
                    break;
                case TaskManagerState.busy:
                    TaskState taskState = CurrentTask.FullUpdate(ExecutingAgent); // perform frame-to-frame updates required by the current task
                    if (taskState == TaskState.Success || taskState == TaskState.Failure)
                    {
                        CurrentState = TaskManagerState.idle;
                        OnTaskFinished?.Invoke(this, CurrentTask);
                        CurrentTask = null;
                    }
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
                CurrentState = TaskManagerState.idle;
            }
            else if (!nextTask.CanStart)
            {
                CurrentState = TaskManagerState.waiting;
            }
            else
            {
                // now actually retrieve the task from the queue
                nextTask = queue.RequestNextTask();
                // The queue is not empty, thus...
                // change the agent's current state to busy,
                CurrentState = TaskManagerState.busy;
                // save the current task,
                CurrentTask = nextTask;
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
    }
}
