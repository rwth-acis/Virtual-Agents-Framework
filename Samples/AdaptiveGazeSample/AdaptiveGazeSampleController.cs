using i5.VirtualAgents.AgentTasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
    public class AdaptiveGazeSampleController : SampleScheduleController
    {
        [SerializeField] private List<GameObject> waypoints;

        [SerializeField] private GameObject target;

        [SerializeField] private int aimAtTime = 100;

        [SerializeField] private bool useTaskActionsForAdaptiveGaze;
        [SerializeField] private bool overwriteAdaptiveGazeWithAimHead = false;

        private int gazeTime = 5;
        protected override void Start()
        {
            List<AgentBaseTask> agentTasks = new List<AgentBaseTask>();
            base.Start();
            // add walking tasks for each waypoint
            // here, we use the TaskActions shortcut but we could also just create a new
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
                taskSystem.Tasks.StopAdaptiveGaze();
                //Start the adaptive gaze after the first waypoint is reached
                AgentBaseTask adaptiveGazeTask = taskSystem.Tasks.StartAdaptiveGazeAsTask(gazeTime);
                adaptiveGazeTask.WaitFor(agentTasks[0]);
                Debug.Log("Adaptive gaze will be started when first waypoint is reached and then run for " + gazeTime + " seconds.");

                StartCoroutine(WaitBeforeStartingAgainIn(gazeTime, adaptiveGazeTask));
            }
        }
        IEnumerator WaitBeforeStartingAgainIn(float waittime, AgentBaseTask task)
        {
            while (!task.IsFinished)
            {
                yield return null;
            }
            Debug.Log("Adaptive gaze has run for " + gazeTime + " Seconds, and is now deactivated. It will be activated in "+ waittime + " Seconds again.");;
            yield return new WaitForSeconds(waittime);
            Debug.Log("Adaptive gaze activated for enternity.");
            taskSystem.Tasks.StartAdaptiveGaze();
        }
    }
}