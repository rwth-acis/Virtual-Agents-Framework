using i5.VirtualAgents.AgentTasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AdaptiveGazeSampleController : SampleScheduleController
    {
        /// <summary>
        /// List of waypoints which the agent should visit in order.
        /// </summary>
        [Tooltip("List of waypoints which the agent should visit in order.")]
        [SerializeField] private List<GameObject> waypoints;

        /// <summary>
        /// The target object which the agent should look at.
        /// </summary>
        [Tooltip("The target object which the agent should look at.")]
        [SerializeField] private GameObject target;

        /// <summary>
        /// The time in seconds that the agent should overwrite the adaptive gaze at the beginning.
        /// </summary>
        [Tooltip("The time in seconds that the agent should overwrite the adaptive gaze at the beginning.")]
        [SerializeField] private int aimAtTime = 100;

        /// <summary>
        /// If true, the adaptive gaze will be started and stopped using TaskActions.
        /// </summary>
        [Tooltip("If true, the adaptive gaze will be started and stopped using TaskActions.")]
        [SerializeField] private bool useTaskActionsForAdaptiveGaze;

        /// <summary>
        /// If true, the adaptive gaze will be overwritten with the aim head animation.
        /// </summary>
        [Tooltip("If true, the adaptive gaze will be overwritten with the aim head animation.")]
        [SerializeField] private bool overwriteAdaptiveGazeWithAimHead = false;

        private int gazeTime = 5;
        protected override void Start()
        {
            List<AgentBaseTask> agentTasks = new List<AgentBaseTask>();
            base.Start();
            // add walking tasks for each waypoint
            // here, we use the TaskActions shortcut, but we could also just create a new
            // AgentMovementTask and schedule it using agent.ScheduleTask.
            for (int i = 0; i < waypoints.Count; i++)
            {
                AgentBaseTask task = taskSystem.Tasks.GoTo(waypoints[i], default);
                agentTasks.Add(task);
            }
            if (overwriteAdaptiveGazeWithAimHead)
            {
                AgentBaseTask pointingHead = taskSystem.Tasks.PlayAnimation("NoAnimation", aimAtTime, "", 0, "Head", target);
            }
            if (useTaskActionsForAdaptiveGaze)
            {
                //First stop the adaptive gaze if it is running from the beginning
                taskSystem.Tasks.ActivateOrDeactivateAdaptiveGaze(false);
                //Start the adaptive gaze after the first waypoint is reached
                AgentBaseTask[] adaptiveGazeTask = taskSystem.Tasks.StartAdaptiveGazeForTime(gazeTime);

                //The first task in the array is the start task that waits for the first waypoint to be reached
                adaptiveGazeTask[0].WaitFor(agentTasks[0]);
                Debug.Log("Adaptive gaze will be started when first waypoint is reached and then run for " + gazeTime + " seconds.");

                //The second task in the array is the task that stops the adaptive gaze after the gazeTime has passed, which we wait for in the following coroutine
                StartCoroutine(WaitBeforeStartingAgainIn(gazeTime, adaptiveGazeTask[1]));
            }
        }
        IEnumerator WaitBeforeStartingAgainIn(float waittime, AgentBaseTask task)
        {

            while (!task.IsFinished)
            {
                yield return null;
            }
            Debug.Log("Adaptive gaze has run for " + gazeTime + " Seconds, and is now deactivated. It will be activated in " + waittime + " Seconds again."); ;
            yield return new WaitForSeconds(waittime);
            Debug.Log("Adaptive gaze activated for enternity.");
            taskSystem.Tasks.ActivateOrDeactivateAdaptiveGaze(true);
        }
    }
}