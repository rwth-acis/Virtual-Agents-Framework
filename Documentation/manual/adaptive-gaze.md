# Adaptive gaze

To make an agent look more life like it can automatically look at objects in front of him, swerve between the objects, idle in between and pick up on sudden interest changes, like objects that start to move. To get this functionally the <xref:i5.VirtualAgents.AdaptiveGaze> component needs to be added to the agent. The automatic gaze is automatically overwritten if a <xref:i5.VirtualAgents.AgentTasks.AgentAnimationTask> is played on the head layer of the agent and will be reactivated when the animation ends.


## Algorithm
The agent checks if there are any <xref:i5.VirtualAgents.AdaptiveGazeTarget>s in front of him and then checks if they are seeable. For calculating the value of interest to the agent, the distance to the target, the importance defined by the target, the time that the agent already look at the target and the perceived novelty is used. 

`timeLookedAt` is modified in two ways:
- `timeLookedAt` increases every time that the item is looked at
- `timeLookedAt` decreases every time that the item is not looked at

`novelty` is modified in two ways:
- `novelty` increases by 5 if the item has not been looked at before
- `novelty` increases by 10 if the importance of the item increased since last seeing it
- `novelty` decreases by 1 every time that the item is looked at
## Requirements

- Agent needs the <xref:i5.VirtualAgents.AdaptiveGaze> component
  - Targets that can be looked at need to be on specific layers, specified in `seeLayers`
  - Objects that should obstruct the view of the agent need to be on specific layers, specified in `occlisonLayers`
- Objects that should be looked at need the <xref:i5.VirtualAgents.AdaptiveGazeTarget> component
  - <xref:i5.VirtualAgents.AdaptiveGazeTarget> needs to be on one of the `seeLayers` that was specified in the agent. The layer can be changed in the top right corner of the inspector 
  - (optional) A collider that makes sense for the Target, if no target is added a standard collider will be added, see `ExampleOfAutoCollider` in the sample scene

## Options - <xref:i5.VirtualAgents.AdaptiveGaze>
The <xref:i5.VirtualAgents.AdaptiveGaze> component has several options that can be modified to fit the agents purpose or personality:
- <xref:i5.VirtualAgents.AdaptiveGaze.detectionRadius> defines how big the detection cube in which items can be seen in front of the agent is. Use Window > Analysis > Physics Debug > Queries to see the detection radius, decrease detection Interval to see the cube on every frame.
- <xref:i5.VirtualAgents.AdaptiveGaze.maxNumberOfTargetsInRange> should be an upper limit of how many items can be in front of the agent, otherwise undefined behavior 
- <xref:i5.VirtualAgents.AdaptiveGaze.detectionIntervalWhenWalking> and <xref:i5.VirtualAgents.AdaptiveGaze.detectionIntervalWhenIdle> is the detection Interval in which the detection cube is checked for items in seconds. Longer Intervals allow the agent to look after items that move, even when they move outside the detection cube
- <xref:i5.VirtualAgents.AdaptiveGaze.chanceHigestedRankedTarget>, <xref:i5.VirtualAgents.AdaptiveGaze.chanceSecondHigestedTarget>, <xref:i5.VirtualAgents.AdaptiveGaze.chanceThirdHigestedTarget> define the chances for looking at the most interesting, second interesting and third interesting item based on the calculated interest value
- <xref:i5.VirtualAgents.AdaptiveGaze.chanceRandomTarget> defines the chance for looking at a random item in sight
- <xref:i5.VirtualAgents.AdaptiveGaze.chanceIdelTarget> defines the chance for the agent to not look at anything specific and to play the ideal animation instead
## Options - <xref:i5.VirtualAgents.AdaptiveGazeTarget>
- `Importance` defines how important the object is to any agent from 1-10. If the `Importance` increases since the agent last looked at it, the `novalty` for that target will also be increased for that agent
- `CanCurrentlyBeLookedAt` can be used to switch the objects perceivability off. This is used by the `Item` component, when the item is picked up or dropped. 

## Example Scenes

The framework contains one example scene that demonstrates the adaptive gaze functionality.
In that the agent walks past multiple objects with the <xref:i5.VirtualAgents.AdaptiveGazeTarget> component and looks at them dynamically. 
The `adaptiveGazeSampleController` adds multiple waypoint where the agent should walk. Optionally, the sample controller also provides options to overwrite the adaptive gaze at the beginning for a specified `AimAtTime`.