using i5.VirtualAgents.TaskSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Agent's functionality mainly includes managing their task queue,
    /// responding to task execution statuses and changing one's state accordingly
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))] // Responsible for the proxy object's movement
    [RequireComponent(typeof(AgentAnimationUpdater))] // Responsible for the avatar's movement
    public class Agent : MonoBehaviour
    {
        [SerializeField] private TaskSystemEnum taskSystemKind;
        public ITaskSystem taskSystem { get; private set; }
        

        /// <summary>
        /// The animator component which controls the agent's animations
        /// </summary>
        public Animator Animator { get; private set; }

        /// <summary>
        /// List of shortcut methods to add common tasks to the agent's task queue
        /// Syntactic sugar. It is also possible to directly enqueue task objects on the agent instead, e.g. for custom tasks
        /// </summary>
        public TaskActions Tasks { get; private set; }

        /// <summary>
        /// Initialize the agent
        /// </summary>
        private void Awake()
        {
            Tasks = new TaskActions(this);
            Animator = GetComponent<Animator>();
            switch (taskSystemKind)
            {
                case TaskSystemEnum.ScheduleBased:
                    taskSystem = new SchedulBasedTaskExecution(this);
                    break;
            }

        }

        public void ScheduleTask(IAgentTask task, int priority = 0, string layer = "Base Layer")
        {
            taskSystem.ScheduleTask(task, priority, layer);
        }

        /// <summary>
        /// Update the task system
        /// </summary>
        private void Update()
        {
            taskSystem.Update();
        }
    }

    enum TaskSystemEnum
    {
        ScheduleBased,
        BehaviorTree
    }
}
