using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Event which is raised once the agent's state changes
        /// </summary>
        public event Action OnStateChanged;
        /// <summary>
        /// Event which is raised once the agent has finished the current task
        /// </summary>
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
                case TaskState.idle:
                    RequestNextTask(); // request new tasks
                    break;
                case TaskState.waitForTaskReadyToBegin: // wait until the task is ready to start
                    if (CheckTaskReadyness(CurrentTask.ReadyToStart))
                        StartCurrentTask();
                    break;
                case TaskState.waitForTaskReadyToEnd: // wait until the task is ready to end
                    if (CheckTaskReadyness(CurrentTask.ReadyToEnd))
                        EndCurrentTask();
                    break;
                case TaskState.busy:
                    NodeState taskState = CurrentTask.Update();
                    if (taskState == NodeState.Success || taskState == NodeState.Failure) // perform frame-to-frame updates required by the current task
                    {
                        TaskFinished();
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
            IAgentTask nextTask = queue.RequestNextTask();
            if (nextTask == null)
            {
                // The queue is empty, thus change the agent's current state to idle
                CurrentState = TaskState.idle;
            }
            else
            {
                // save the current task,
                CurrentTask = nextTask;

                if (CheckTaskReadyness(CurrentTask.ReadyToStart))
                {
                    StartCurrentTask();
                }
                else
                {
                    //The current task isn't ready yet, wait until it signals that it is
                    currentState = TaskState.waitForTaskReadyToBegin;
                }
            }
        }

        /// <summary>
        /// Helper function to be called when a task has been executed.
        /// Set agent's state to idle
        /// </summary>
        private void TaskFinished()
        {

            if (CheckTaskReadyness(CurrentTask.ReadyToEnd))
            {
                EndCurrentTask();
            }
            else
            {
                //Task isn't ready yet to be ended, wait until it signals that it is
                currentState = TaskState.waitForTaskReadyToEnd;
            }
        }

        //Exceute task and change agent state to busy
        private void StartCurrentTask()
        {
            // change the agent's current state to busy,
            CurrentState = TaskState.busy;

            // execute the next task,
            CurrentTask.Execute(ExecutingAgent);
        }

        //Invoe OnTaskFinish and set agent state to idel
        private void EndCurrentTask()
        {
            // change the agent's current state to idle,
            CurrentState = TaskState.idle;
            CurrentTask.Stop();
            RequestNextTask();
        }

        //Is the task ready to be scheduled or finished?
        private bool CheckTaskReadyness(List<Func<bool>> isReady)
        {
            return isReady == null || isReady.Count == 0 || //Does the current task implement no prepare/cleanup functions? If it doesn't, it is ready for scheduling/finish
                 isReady.Aggregate((result, item) => () => result() && item())(); //If it does, does every prepare/cleanup function report that it has finished?
        }

    }
}
