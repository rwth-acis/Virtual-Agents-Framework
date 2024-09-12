using i5.VirtualAgents.AgentTasks;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VirtualAgents.Examples
{
	public class AimingSampleController : SampleScheduleController
	{
		/// <summary>
		/// List of waypoints which the agent should visit in order.
		/// </summary>
		[Tooltip("List of waypoints which the agent should visit in order.")]
		[SerializeField] private List<Transform> waypoints;

		/// <summary>
		/// Waypoint which is visited with high priority.
		/// </summary>
		[Tooltip("Waypoint which is visited with high priority.")]
		[SerializeField] private Transform highPrioWaypoint;

		/// <summary>
		/// If true, task shortcuts are used to create the tasks. If false, the tasks are created explicitly.
		/// </summary>
		[Tooltip("If true, task shortcuts are used to create the tasks. If false, the tasks are created explicitly.")]
		[SerializeField] private bool useTaskShortcuts;

		/// <summary>
		/// If true, the agent walks to the waypoints. If false, the agent stays at the starting position.
		/// </summary>
		[Tooltip("If true, the agent walks to the waypoints. If false, the agent stays at the starting position.")]
		[SerializeField] private bool walk = true;

		/// <summary>
		/// The target which the agent should aim at.
		/// </summary>
		[Tooltip("The target which the agent should aim at.")]
		[SerializeField] private GameObject target;

		/// <summary>
		/// The time in seconds the agent should aim at the target.
		/// </summary>
		[Tooltip("The time in seconds the agent should aim at the target.")]
		[SerializeField] private int aimAtTime = 5;

		/// <summary>
		/// If true, the agent aims its head at the target.
		/// </summary>
		[Tooltip("If true, the agent aims its head at the target.")]
		[SerializeField] private bool aimHead = true;

		/// <summary>
		/// If true, the agent aims its left arm at the target.
		/// </summary>
		[Tooltip("If true, the agent aims its left arm at the target.")]
		[SerializeField] private bool aimLeftArm = true;

		/// <summary>
		/// If true, the agent aims its right arm at the target.
		/// </summary>
		[Tooltip("If true, the agent aims its right arm at the target.")]
		[SerializeField] private bool aimRightArm = true;

		/// <summary>
		/// If true, the agent waves its right arm.
		/// </summary>
		[Tooltip("If true, the agent waves its right arm.")]
		[SerializeField] private bool waveRightArm = true;

		protected override void Start()
		{
			base.Start();
			if (walk)
			{
				for (int i = 0; i < waypoints.Count; i++)
				{
					taskSystem.Tasks.GoTo(waypoints[i].position);
				}
			}


			if (useTaskShortcuts)
			{
				Debug.Log("Agent has been equipped with tasks via shortcuts");

				// The quickest and most intuitive way is to use the task shortcuts of the agent
				if (waveRightArm)
				{
					AgentBaseTask wave1 = taskSystem.Tasks.PlayAnimation("WaveRight", 5, "", 0, "Right Arm");
				}

				// A target can be added to all animations but works best with no animation or animations that are meant for aiming (eg. "startAimAt") 

				if (aimHead)
				{
					AgentBaseTask pointingHead = taskSystem.Tasks.PlayAnimation("NoAnimation", aimAtTime, "", 0, "Head", target);
				}
				if (aimLeftArm)
				{
					AgentBaseTask pointingLeft = taskSystem.Tasks.PlayAnimation("PointingLeft", aimAtTime, "", 0, "Left Arm", target);
				}
				if (aimRightArm)
				{
					AgentBaseTask pointingRight = taskSystem.Tasks.PlayAnimation("PointingRight", aimAtTime, "", 0, "Right Arm", target);
				}
			}
			else
			{
				// Alternative way: Create the tasks and schedule them explicitly
				if (waveRightArm)
				{
					AgentAnimationTask wave1 = new AgentAnimationTask("WaveRight", 5, "", "Right Arm");
					taskSystem.ScheduleTask(wave1, 0, "Right Arm");
				}

				if (aimHead)
				{
					AgentAnimationTask pointingHead = new AgentAnimationTask("NoAnimation", aimAtTime, "", "Head", target);
					taskSystem.ScheduleTask(pointingHead, 0, "Head");
				}
				if (aimLeftArm)
				{
					AgentAnimationTask pointingLeft = new AgentAnimationTask("PointingLeft", aimAtTime, "", "Left Arm", target);
					taskSystem.ScheduleTask(pointingLeft, 0, "Left Arm");
				}
				if (aimRightArm)
				{
					AgentAnimationTask pointingRight = new AgentAnimationTask("PointingRight", aimAtTime, "", "Right Arm", target);
					taskSystem.ScheduleTask(pointingRight, 0, "Right Arm");
				}
			}
			// If you want the agent to rotate towards a target before pointing, you can use the PointAt TaskAction
			AgentBaseTask pointAt = taskSystem.Tasks.PointAt(target, true, false, aimAtTime);

			// or to rotate without using TaskActions you can use the AgentRotationTask
		}
	}
}