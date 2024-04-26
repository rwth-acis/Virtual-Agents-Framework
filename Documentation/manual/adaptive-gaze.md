# Adaptive Gaze

To make an agent look more life like it can automatically look at objects in front of him, swerve between the objects, idle in between and pick up on sudden interest changes, like objects that start to move.
To get this functionally the <xref:i5.VirtualAgents.AdaptiveGaze> component needs to be added to the agent.
The automatic gaze is automatically overwritten if a <xref:i5.VirtualAgents.AgentTasks.AgentAnimationTask> is played on the head layer of the agent and will be reactivated when the animation ends. The selection of a gaze Target can also be overwritten, so that the agent looks constantly at the object specified in <xref:i5.VirtualAgents.AdaptiveGaze.OverwriteGazeTarget>.

## Algorithm
The agent scans for objects ahead that are on the specified ``seeLayers`` and checks if they have the <xref:i5.VirtualAgents.AdaptiveGazeTarget> component. Then, it assesses their visibility using the `occlusionLayers`. To determine the relevance to the agent, several factors are considered, including distance to the target, target-defined importance, duration of the agent's gaze, and perceived novelty.

`timeLookedAt` is modified in two ways:
- `timeLookedAt` increases every time that the item is looked at.
- `timeLookedAt` decreases every time that the item is not looked at.

`novelty` is modified in two ways:
- `novelty` increases by 5 if the item has not been looked at before.
- `novelty` increases by 10 if the importance of the item increased since last seeing it.
- `novelty` decreases by 1 every time that the item is looked at.


## Setup Requirements

- The agent needs the <xref:i5.VirtualAgents.AdaptiveGaze> component.
  - Targets that can be looked at need to be on specific layers, specified in `seeLayers`.
  - Objects that should obstruct the view of the agent need to be on specific layers, specified in `occlusionLayers`.
- Objects that should be looked at need the <xref:i5.VirtualAgents.AdaptiveGazeTarget> component.
  - <xref:i5.VirtualAgents.AdaptiveGazeTarget> needs to be on one of the `seeLayers` that was specified in the agent.
	The layer can be changed in the top right corner of the inspector.
  - (optional) A collider that makes sense for the Target, if no target is added a standard collider will be added, see `ExampleOfAutoCollider` in the sample scene.

The standard for `seeLayers` is `Everything` and the standard for `occlusionLayers` is `Default`. **It is very much recommended that one or more layers are created (as explained [here](https://docs.unity3d.com/Manual/create-layers.html)) and set as `seeLayers` for the purpose of identifying possible gaze targets as that drastically reduces the computation load.**

## Starting and stopping adaptive gazing
<xref:i5.VirtualAgents.AdaptiveGaze.Activate> and <xref:i5.VirtualAgents.AdaptiveGaze.Deactivate> can be used to start and stop the adaptive gazing directly on the <xref:i5.VirtualAgents.AdaptiveGaze> component. 

The following shortcuts that are part of the task actions are also available to schedule adaptive gazing as a [task](task-system.md) :
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.ActivateOrDeactivateAdaptiveGaze*>: Start or stops adaptive gazing until it is stopped or started again. This is realized with a task that only runs once. This also automatically adds a <xref:i5.VirtualAgents.AdaptiveGaze> component if the agent doesn't have one.
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.StartAdaptiveGazeForTime*>: Schedule a task that starts adaptive gazing for the specified time and then deactivates it by scheduling a wait task between a start and stop task. This will also stop adaptive gazing, when it was started with <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.ActivateOrDeactivateAdaptiveGaze*> before.

Starting the adaptive gaze as a task can be useful as it e.g. allows for the task to be scheduled or wait for a different task to finish using the <xref:i5.VirtualAgents.AgentTasks.AgentBaseTask.WaitFor*> function, see example scene.

## Options - <xref:i5.VirtualAgents.AdaptiveGaze>
The <xref:i5.VirtualAgents.AdaptiveGaze> component has several options that can be modified to fit the agents purpose or personality:
- `detectionRadius` defines how big the detection cube in which items can be seen in front of the agent is.
  Use Window > Analysis > Physics Debug > Queries to see the detection radius, decrease detection Interval to see the cube on every frame.
- `maxNumberOfTargetsInRange` should be an upper limit of how many items can be in front of the agent, otherwise undefined behaviour.
- `detectionIntervalWhenWalking` and `detectionIntervalWhenIdle` is the detection interval in which the detection cube is checked for items in seconds.
  Longer intervals allow the agent to look after items that move, even when they move outside the detection cube.
- `chanceHighestRankedTarget`, `chanceSecondHighestTarget`, `chanceThirdHighestTarget` define the chances for looking at the most interesting, second interesting and third interesting item based on the calculated interest value.
- `chanceRandomTarget` defines the chance for looking at a random item in sight.
- `chanceIdleTarget` defines the chance for the agent to not look at anything specific and to play the ideal animation instead.
- <xref:i5.VirtualAgents.AdaptiveGaze.OverwriteGazeTarget> defines a target that the agent will look at as long is it is set

## Options - <xref:i5.VirtualAgents.AdaptiveGazeTarget>
- `Importance` defines how important the object is to any agent from 1-10.
  If the `Importance` increases since the agent last looked at it, the `novelty` for that target will also be increased for that agent.
- `CanCurrentlyBeLookedAt` can be used to switch the objects perceivability off.
  This is used by the `Item` component when the item is picked up or dropped.

## Example Scenes

The framework contains one example scene that demonstrates the adaptive gaze functionality.
In that the agent walks past multiple objects with the <xref:i5.VirtualAgents.AdaptiveGazeTarget> component and looks at them dynamically.
The `AdaptiveGazeSampleController` adds multiple waypoints where the agent should walk. 
Optionally, the sample controller also provides options to overwrite the adaptive gaze at the beginning for a specified `AimAtTime`. 
Alternatively, while in play mode any object can be dragged into the <xref:i5.VirtualAgents.AdaptiveGaze.OverwriteGazeTarget> attribute of <xref:i5.VirtualAgents.AdaptiveGaze> to make the agent look at it constantly.

The example scene also demonstrates how the Shortcut Task actions can be used to activate adaptive gazing. Activate `useTaskActionsForAdaptiveGaze` on the controller to use that example in Play Mode.