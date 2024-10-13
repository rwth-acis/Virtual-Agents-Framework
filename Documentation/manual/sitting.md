# Sitting

The agent has the ability to sit on chairs or other objects of variable height. To do this, the GameObject the agent should sit on needs to have multiple positions defined, which are marked as Empty child GameObjects.


## Requirements
GameObjects intended to be used for sitting must be set up with the following Empty GameObjects as children:
1. **"SitPosition"** - The position where the agent should sit, i.e. the position the hip should rest at while sitting.
2. **"FeetPosition"** - The position the agent should stand before and after sitting. The agent should walk to this position before initiating the task.
3. **"Footrest"** - The position the agent should rest his feet on while sitting. This might be the literal footrest of a chair or an arbitrary position in the air, for chairs where the agent can't reach the ground with their feet. This is the only optional position, as the FeetPosition will be used instead, if it is missing.

*The Empties should be named exactly as described above.*

## Usage
After ensuring that a chair GameObject is set up correctly, the agent can be instructed to sit on it by creating a new `AgentSittingTask`.
Its first parameter is the GameObject the agent should sit on, the second parameter is either `SittingDirection.SITDOWN` or `SittingDirection.STANDUP`, depending on whether the agent should sit down or stand up.
Alternatively use `SittingDirection.TOGGLE` to toggle between sitting and standing, depending on the current state.

```csharp
AgentSittingTask sittingTask = new AgentSittingTask(Chair, SittingDirection.SITDOWN);
AgentSittingTask standingTask = new AgentSittingTask(Chair, SittingDirection.STANDUP);

taskSystem.ScheduleTask(sittingTask);
taskSystem.Tasks.WaitForSeconds(3);
taskSystem.ScheduleTask(standingTask);
```


## Example Scenes

The framework contains an example scene that demonstrates the ability to sit on different chairs and stools.

During execution of the scene, the agent will walk to a chair, sit down, stand up, and then walk to a stool to do the same.
