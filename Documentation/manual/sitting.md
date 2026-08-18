# Sitting

The agent can sit down on a <xref:i5.VirtualAgents.Chair> using <xref:i5.VirtualAgents.AgentTasks.AgentSittingTask>, or use the convenience shortcut <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndSit*> from <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions> to first walk to the chair and then sit.

## Requirements

The sitting implementation relies on the <xref:i5.VirtualAgents.Chair> component on a game object in the scene that represents the chair. You should add the chair component to the root of the chair and assign at least the following two transforms to the chair component. You can add empty game objects as children of the chair to assign these transforms:

1. <xref:i5.VirtualAgents.Chair.StandingFeetPosition> - This is the position the agent will initially walk to and stand on when sitting down and to where it will return when standing up. Ideally it should be positioned right in front of the chair.
2. <xref:i5.VirtualAgents.Chair.SeatedHipPosition> - This is the position the agent will move its hips to when sitting down. Ideally it should be positioned slightly above the chair seat.

Additional chair settings:
- <xref:i5.VirtualAgents.Chair.SeatedFeetPosition> - Used as the seated foot target, this could be a footrest on the chair. When it is not set, the <xref:i5.VirtualAgents.Chair> falls back to <xref:i5.VirtualAgents.Chair.StandingFeetPosition>.
- <xref:i5.VirtualAgents.Chair.distanceBetweenFeet> - Controls how far apart the left and right foot are placed apart while seated. The default value should work for most agents and chairs.

## Using <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndSit*> (recommended)

You can use two convenience shortcuts from <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions> to easily schedule sitting tasks. If you want the agent to walk to the <xref:i5.VirtualAgents.Chair> before sitting, use <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndSit*>. Similarly, you can use <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.StandUp*> to make the agent stand up from the chair.

```csharp
            taskSystem.Tasks.GoToAndSit(Chair); // Chair is a reference to a game object with the Chair component, see sample scene for an example
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.StandUp(Chair);

```

## Using <xref:i5.VirtualAgents.AgentTasks.AgentSittingTask>

<xref:i5.VirtualAgents.AgentTasks.AgentSittingTask> takes a <xref:i5.VirtualAgents.Chair> instance and a <xref:i5.VirtualAgents.AgentTasks.SittingDirection>:

- <xref:i5.VirtualAgents.AgentTasks.SittingDirection.SITDOWN> makes the agent sit if it is standing.
- <xref:i5.VirtualAgents.AgentTasks.SittingDirection.STANDUP> makes the agent stand if it is sitting.
- <xref:i5.VirtualAgents.AgentTasks.SittingDirection.TOGGLE> switches between both states depending on the current animation state.

```csharp
AgentSittingTask sittingTask = new AgentSittingTask(Chair, SittingDirection.SITDOWN); // Chair is a reference to a game object with the Chair component, see sample scene for an example
AgentSittingTask standingTask = new AgentSittingTask(Chair, SittingDirection.STANDUP);

taskSystem.ScheduleTask(sittingTask);
taskSystem.Tasks.WaitForSeconds(3);
taskSystem.ScheduleTask(standingTask);

```

## Example Scenes

The framework contains an example scene that demonstrates sitting on different chairs and stools.

During execution of the scene, the agent walks to a chair, sits down, stands up, picks up an item, and then walks to a stool to do the same. Check out the ``Chair`` and ``Stool`` gameobjects in the scene to see how the chair component is set up and how the different transforms are assigned.

## Related

The <xref:i5.VirtualAgents.Chair> component is a subclass of <xref:i5.VirtualAgents.Item> component, so it can also be picked up and dropped like any other item. Learn more about items in the [items documentation](items.md).