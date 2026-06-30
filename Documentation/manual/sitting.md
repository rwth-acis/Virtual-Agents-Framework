# Sitting

The agent can sit down on a <xref:i5.VirtualAgents.Chair> using <xref:i5.VirtualAgents.AgentTasks.AgentSittingTask>, or use the convenience shortcut <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndSit*> from <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions> to first walk to the chair and then sit.

## Requirements

The sitting implementation relies on the <xref:i5.VirtualAgents.Chair> component on a game object in the scene that represents the chair. You should add the chair component to the root of the chair and assign at least the following two transforms to the chair component. You can add empty game objects as children of the chair to assign these transforms:

1. <xref:i5.VirtualAgents.Chair.StandingFeetPosition> - This is the position the agent will initially walk to and stand on when sitting down and to where it will return when standing up. Idearly it should be positioned right in front of the chair.
2. <xref:i5.VirtualAgents.Chair.SeatedHipPosition> - This is the position the agent will moves it's hips to when sitting down. Idearly it should be positioned slightly above the chair seat. Note that the rotation of this transform decides the rotation of the entire agent.

Additional chair settings:
- <xref:i5.VirtualAgents.Chair.SeatedFeetPosition> - Used as the seated foot target, this could be a feet rest on the chair. When it is not set, the <xref:i5.VirtualAgents.Chair> falls back to <xref:i5.VirtualAgents.Chair.StandingFeetPosition>. Its rotation determines foot positioning (0°: next to each other, 90°/270°: behind each other, 180°: crossed-legged) while pointing towards the SeatedHipPosition (see skateboard in example scene).
- <xref:i5.VirtualAgents.Chair.distanceBetweenFeet> - Controls how far apart the left and right foot are placed apart while seated. The default value should work for most agents and chairs.

## Using <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndSit*> (recommended)

If you want the agent to walk to the chair before sitting, use <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndSit*>. It can be used the same way to make the agent stand up from the chair again, just with a different <xref:i5.VirtualAgents.AgentTasks.SittingDirection> or no SittingDirection to toggle it automatically.

```csharp
            taskSystem.Tasks.GoToAndSit(Chair, SittingDirection.SITDOWN);
            taskSystem.Tasks.WaitForSeconds(3);
            // GoToAndSit is also used for making the agent standup from a chair
            taskSystem.Tasks.GoToAndSit(Chair, SittingDirection.STANDUP);
            
            // If no SittingDirection is defined it will automatically be toggled
            taskSystem.Tasks.GoToAndSit(Stool);
            taskSystem.Tasks.WaitForSeconds(3);
            taskSystem.Tasks.GoToAndSit(Stool); 

```

## Using <xref:i5.VirtualAgents.AgentTasks.AgentSittingTask>

<xref:i5.VirtualAgents.AgentTasks.AgentSittingTask> takes a <xref:i5.VirtualAgents.Chair> instance and a <xref:i5.VirtualAgents.AgentTasks.SittingDirection>:

- <xref:i5.VirtualAgents.AgentTasks.SittingDirection.SITDOWN> makes the agent sit if it is standing.
- <xref:i5.VirtualAgents.AgentTasks.SittingDirection.STANDUP> makes the agent stand if it is sitting.
- <xref:i5.VirtualAgents.AgentTasks.SittingDirection.TOGGLE> switches between both states depending on the current animation state.

```csharp
AgentSittingTask sittingTask = new AgentSittingTask(chair, SittingDirection.SITDOWN);
AgentSittingTask standingTask = new AgentSittingTask(chair, SittingDirection.STANDUP);

taskSystem.ScheduleTask(sittingTask);
taskSystem.Tasks.WaitForSeconds(3);
taskSystem.ScheduleTask(standingTask);

```

## Interacting with Mobile Seats (e.g. Wheelchairs & Skateboards)

You can combine sitting with agent movement to implement mobile seats such as wheelchairs or skateboards. By casting the returned task of <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndSit*> to a <xref:i5.VirtualAgents.TaskBundle>, you can subscribe to the `OnTaskFinished` event. Once the agent is seated, you can parent the vehicle/seat to the agent and activate relevant components (such as tire animations or custom movement parameters). You can find an example of how to do this for a wheelchair and a skateboard in the sitting example scene.

> [!NOTE]
> <xref:i5.VirtualAgents.AgentTasks.AgentMovementTask> relies on Unity's NavMesh system which allows in-place rotation. It is suitable for simple prototyping and works well for wheelchair movement, but not for realistic vehicle steering physics.

### Wheelchair Example
For a wheelchair, once the sitting task completes, the wheelchair is parented to the agent and its wheel/tire animations are driven by the agent's movement:

```csharp
// Sit on the wheelchair
TaskBundle wheelChairTask = (TaskBundle) taskSystem.Tasks.GoToAndSit(Wheelchair);

// Once seated, parent the wheelchair to the agent and connect the tire animation script
wheelChairTask.OnTaskFinished += () =>
{
    Wheelchair.transform.parent = agent.transform;
    Wheelchair.GetComponent<TireAnimation>().agent = agent.GetComponent<NavMeshAgent>();
};

// Instruct the agent to move to a waypoint (the wheelchair moves with it)
taskSystem.Tasks.GoTo(waypoint1);

// Stand up from/leave the wheelchair
TaskBundle wheelChairTaskEnd = (TaskBundle) taskSystem.Tasks.GoToAndSit(Wheelchair);

// Deparent the wheelchair once stood up
wheelChairTaskEnd.OnTaskFinished += () =>
{
    Wheelchair.transform.parent = null;
    Wheelchair.GetComponent<TireAnimation>().agent = null;
};
```

## Example Scenes

The framework contains an example scene that demonstrates sitting on different chairs, stools, and mobile seats.

During execution of the scene in the `AgentSittingController`:
1. The agent walks to a static chair, sits down, waits for 3 seconds, and then stands up.
2. The agent sits on a stool (demonstrating automatic state toggling when no explicit direction is specified), waits, and stands up.
3. The agent sits on a wheelchair, which is then parented to the agent to move with them to a waypoint, after which the agent leaves the wheelchair.
4. The agent sits on a skateboard, which modifies the agent's velocity/rotation attributes to simulate skateboarding dynamics. The agent picks up an item, rides to another waypoint, and then steps off the skateboard (reverting the movement settings).

Checkout the `Chair`, `Stool`, `Wheelchair`, and `Skateboard` gameobjects in the scene to see how the <xref:i5.VirtualAgents.Chair> component is set up and how the different transforms are assigned.

## Related

The <xref:i5.VirtualAgents.Chair> component is a subclass of <xref:i5.VirtualAgents.Item> component, so it can also be picked up and dropped like any other item. Learn more about items in the [items documentation](items.md).