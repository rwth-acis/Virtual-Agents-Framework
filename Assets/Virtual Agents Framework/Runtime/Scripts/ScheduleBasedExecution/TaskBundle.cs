using System.Collections;
using System.Collections.Generic;
using i5.VirtualAgents.AgentTasks;
using UnityEngine;

namespace i5.VirtualAgents
{
    /// <summary>
    /// A task which consists of multiple subtasks.
    /// It allows for checking of preconditions and then executing a sequence of tasks.
    /// </summary>
    public class TaskBundle : ITask
    {
        TaskBundle()
        {
            TaskState State = TaskState.Waiting;
            taskQueue = new List<IAgentTask>();
        }

        TaskBundle(List<IAgentTask> tasks)
        {
            TaskState State = TaskState.Waiting;
            taskQueue = tasks;
        }

        TaskBundle(List<IAgentTask> tasks, List<bool> preconditions)
        {
            TaskState State = TaskState.Waiting;
            taskQueue = tasks;
            this.preconditions = preconditions;
        }
        public TaskState State { get; set; }

        /// <summary>
        /// List of tasks to be part of the bundle
        /// </summary>
        public List<IAgentTask> taskQueue { get; set; }

        /// <summary>
        /// List of conditions to be met before execution of tasks
        /// </summary>
        public List<bool> preconditions { get; set; }

        public TaskState EvaluateTaskState()
        {
            return State;
        }

        /// <summary>
        /// Check for preconditions and start the execution of all subtasks in sequence
        /// </summary>
        /// <param name="executingAgent"></param>
        public void StartExecution(Agent executingAgent)
        {
            State = TaskState.Running;
            if (checkPreconditions())
            {
                executeTasks(executingAgent);
            }

            State = TaskState.Success;
            StopExecution();
        }

        /// <summary>
        /// Execute all tasks in the task queue. If a task fails, the whole bundle fails. Note, that checking of preconditions is not part of this method.
        /// </summary>
        /// <param name="executingAgent"></param>
        private void executeTasks(Agent executingAgent)
        {
            foreach (IAgentTask task in taskQueue)
            {
                TaskState currentState = task.EvaluateTaskState();
                while (currentState != TaskState.Success)
                {
                    task.Tick(executingAgent);
                    if (currentState == TaskState.Failure)
                    {
                        State = TaskState.Failure;
                        StopExecution();
                        return;
                    }

                    currentState = task.EvaluateTaskState();
                }

            }
        }

        /// <summary>
        /// Check if all preconditions are met.
        /// </summary>
        /// <returns> True if all preconditions evaluate to true, otherwise false. </returns>
        private bool checkPreconditions()
        {
            foreach (bool condition in preconditions)
            {
                if (condition == false)
                {
                    return false;
                }
            }

            return true;
        }

        public void StopExecution() { }

        public TaskState Tick(Agent excutingAgent)
        {
            if(State == TaskState.Waiting)
            {
                StartExecution(excutingAgent);
            }

            if (State == TaskState.Success || State == TaskState.Failure)
            {
                StopExecution();
            }
            return State;
        }
    }
}