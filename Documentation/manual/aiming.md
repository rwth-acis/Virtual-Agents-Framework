# Aiming
The framework provides the capabilities to have an agent point or aim at something in the scene.
The provided aiming capabilities are used by the [adaptive gaze](adaptive-gaze.md) and the inverse kinematics pointing/aiming animation, that can be used by specifying a target in the <xref:i5.VirtualAgents.AgentTasks.AgentAnimationTask> or in the shortcut <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.PlayAnimation(System.String,System.Single,System.String,System.Int32,System.String,GameObject)>.
All of these automatically add a <xref:i5.VirtualAgents.AimAt> to the agent.

## <xref:i5.VirtualAgents.AimAt> Script
The <xref:i5.VirtualAgents.AimAt> script defines what the target is, what part(s) of the agent should point at the target, and what bones move to accomplish the pointing.
There are several bone presets, for example `Right Arm` that can be selected by specifying a layer.
As of now the aiming animation has predefined options for the following:
1. `Right Arm`
2. `Left Arm`
3. `Right Leg`
4. `Left Leg`
5. `Head`
6. `Base Layer` (orient chest, spine and hips towards the target)

The <xref:i5.VirtualAgents.AimAt> script can be extended to allow for more or differently weighted bone presets.

## Example Scenes

The framework contains one example scene that directly demonstrates the pointing functionality, as well as the [adaptive gaze](adaptive-gaze.md#example-scenes) sample that uses the aiming capabilities indirectly.
In the 'AimingSample' the agent aims at a moving target with its two hands and its head.

The `AimingSampleController` has the option to add multiple waypoints where the agent should walk, while pointing at the targets.
If selected in the inspector the controller adds pointing tasks for the head, the left arm and the right arm.
It also allows to first play a wave animation on the right arm.