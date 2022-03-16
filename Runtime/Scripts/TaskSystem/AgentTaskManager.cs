using System;
using System.Collections.Generic;

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

        private TaskBundel currentTaskBundel;

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
        public TaskEntry CurrentTask { get; private set; }

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
                case TaskState.waitForBundleBegin: // wait until all tasks from the current task bundle are ready for exceution
                    break;
                case TaskState.waitForBundleEnd: // wait until all tasks from the current task bundle are finished
                    break;
                case TaskState.busy:
                    CurrentTask.task.Update(); // perform frame-to-frame updates required by the current task
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskBundel"></param>
        /// <param name="priority"></param>
        public void ScheduleTaskBundel(TaskBundel taskBundel)
        {
            queue.AddTask(taskBundel.Tasks[this], taskBundel.priority, taskBundel);
        }

        // get the next task from the queue and adapts the states accordingly
        private void RequestNextTask()
        {
            TaskEntry nextTask = queue.RequestNextTask();
            if (nextTask.task == null)
            {
                // The queue is empty, thus change the agent's current state to idle
                CurrentState = TaskState.idle;
            }
            else
            {
                // save the current task,
                CurrentTask = nextTask;
                

                //Check if the next task belongs to a task bundel

                if (CurrentTask.taskBundel != null)
                {
                    CurrentState = TaskState.waitForBundleBegin;

                    CurrentTask.taskBundel.TaskManagerReady(this);
                    currentTaskBundel = CurrentTask.taskBundel;
                    return;
                }

                // subscribe to the task's OnTaskFinished event to set the agent's state to idle after task execution
                CurrentTask.task.OnTaskFinished += TaskFinished;

                // The queue is not empty, thus...
                // change the agent's current state to busy,
                CurrentState = TaskState.busy;
                
                // execute the next task,
                nextTask.task.Execute(ExecutingAgent);
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
            CurrentTask.task.OnTaskFinished -= TaskFinished;
            OnTaskFinished?.Invoke();
        }

        /// <summary>
        /// Begins the execution of the current task bundle by setting the state to busy and invoking the current tasks excecute method
        /// </summary>
        public void StartBundle()
        {
            if (currentState == TaskState.waitForBundleBegin)
            {
                CurrentState = TaskState.busy;
                CurrentTask.task.OnTaskFinished += WaitForBundle;
                CurrentTask.task.Execute(ExecutingAgent);
            }
        }

        // Lets the taskmanager idle until all tasks from the current task bundle are finished as well
        private void WaitForBundle()
        {
            CurrentState = TaskState.waitForBundleEnd;
            CurrentTask.task.OnTaskFinished -= WaitForBundle;
            currentTaskBundel.TaskManagerFinished(this);
        }

        /// <summary>
        /// Ends the execution of the current task bundle by setting the state to idle and invoking the current tasks OnTaskFinished event
        /// </summary>
        public void EndBundle()
        {
            if (currentState == TaskState.waitForBundleEnd)
            {
                CurrentState = TaskState.idle;
                OnTaskFinished?.Invoke();
            }
        }
    }
}
