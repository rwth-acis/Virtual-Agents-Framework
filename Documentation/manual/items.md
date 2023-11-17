# Items

Objects that an agent can interact with in any way, should get the <xref:i5.VirtualAgents.Item> component.
Currently, this allows items to be picked up and dropped.

## Picking up and dropping items
Items can be picked up with a <xref:i5.VirtualAgents.AgentTasks.AgentPickUpTask> or <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.PickUp*> when they are close enough, the task will fail if the object is not near enough or if <xref:i5.VirtualAgents.Item.canBePickedUp> of the items return false.
To bring an agent easily in the reach of an item the shortcut <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndPickUp*> can be used.
With all methods a socket can be specified to which the item will be attached when after it is picked up. Currently, the following sockets are supported:

1. `MeshSockets.SocketId.RightHand`
2. `MeshSockets.SocketId.LeftHand`
3. `MeshSockets.SocketId.Spine`

When the LeftHand socket is selected a simple [inverse kinematics](https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/TwoBoneIKConstraint.html) (IK) animation on the left hand will be played, for the Spine and RightHand socket, the animation will be played on the right Hand.
The item can also specify a <xref:i5.VirtualAgents.Item.grapTarget>, this is where the IK animation will aim for and also what will we placed at the socket. Notice that the rotation of the agent's hand will match the <xref:i5.VirtualAgents.Item.grapTarget>'s rotation.

Items can be dropped with a <xref:i5.VirtualAgents.AgentTasks.AgentDropTask> or the shortcut <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.DropItem*>.
This will detach the item from the agent and invoke the <xref:i5.VirtualAgents.Item.dropEvent> of the Item.
As seen in the [example](items.md#example-scenes) this could be used to reactivate physics on the item.

### Requirements
1. The agent needs the `MeshSockets` and `RigBuilder` components, as well as everything that is part of the `AnimationRigging` child object in the agent prefab, in which the Sockets for the item positions are defined as well as the inverse kinematics animation for the grab animation.
   For more information on the used Unity Package see [animation rigging package](https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/index.html).
2. Items that should be picked up need the <xref:i5.VirtualAgents.Item> component.
    1. The Items <xref:i5.VirtualAgents.Item.canBePickedUp> method needs to return true.
    2. (optional) A <xref:i5.VirtualAgents.Item.grapTarget> can be defined, that's where the agent will move its hand position and rotation wise and what attaches to the specified socked when picking the object up. The default is the local zero coordinates of the item.


## Example Scenes

The framework contains one example scene that demonstrates the ability to pickup and drop items.

The samples contain a `ItemController` that controls the movement of the items in the scene, moving them in squares as long as there are not picked up. It also listens to the items <xref:i5.VirtualAgents.Item.dropEvent> and activates the rigidbody physics of the sword item when the event is invoked.
The `ItemPickUpSampleController` first adds a movement task and a pickup task for each item in the scene, as defined in the controller object. Both items will be picked up with the right Hand (currently not changeable), but the second one will be stored in the left-hand socket.
After that the first item, that looks like a sword is dropped at a specified point. The second item is dropped by calling the method that drops all items at a specific point.

## Adaptive gaze

To make an agent look more life like it can automatically look at objects and items in front of him, swerve between the objects, idle in between and pick up on sudden interest changes, like objects that start to move. To make the agent look at objects that are about to be picked up the <xref:i5.VirtualAgents.AdaptiveGazeTarget> component can be added to the object.
It will automatically be deactivated when the object is picked up.

See [adaptive gaze](adaptive-gaze.md) for more information.