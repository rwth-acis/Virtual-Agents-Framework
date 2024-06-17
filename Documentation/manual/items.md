# Items

Objects that an agent can interact with in any way, should get the <xref:i5.VirtualAgents.Item> component.
Currently, this allows items to be picked up and dropped.

## Picking Up and Dropping Items
Items can be picked up with a <xref:i5.VirtualAgents.AgentTasks.AgentPickUpTask> or <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.PickUp*> when they are close enough, the task will fail if the object is not near enough or if <xref:i5.VirtualAgents.Item.CanBePickedUp> of the items return false.
To bring an agent easily in the reach of an item the shortcut <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndPickUp*> can be used.
With all methods a socket can be specified to which the item will be attached when after it is picked up.
Currently, the following sockets are supported:

1. `MeshSockets.SocketId.RightHand`
2. `MeshSockets.SocketId.LeftHand`
3. `MeshSockets.SocketId.RightLowerArm`
4. `MeshSockets.SocketId.LeftLowerArm`
5. `MeshSockets.SocketId.RightUpperArm`
6. `MeshSockets.SocketId.LeftUpperArm`
7. `MeshSockets.SocketId.RightBack`
8. `MeshSockets.SocketId.LeftBack`
9. `MeshSockets.SocketId.HipsFrontLeft`
10. `MeshSockets.SocketId.HipsFrontRight`
11. `MeshSockets.SocketId.HipsBackLeft`
12. `MeshSockets.SocketId.HipsBackRight`
13. `MeshSockets.SocketId.AdditionalSocket1` to `MeshSockets.SocketId.AdditionalSocket10`

The position and rotation of existing sockets can be changed by modifying the corresponding game objects in the Agent Prefab under `Agent > AnimationRigging > MeshSockets`. Up to 10 additional Sockets can also be added there, by creating a new game object with a `Multi-Parent Constraint` and the <xref:i5.VirtualAgents.MeshSocket> component. In the <xref:i5.VirtualAgents.MeshSocket> component one of the 10 `MeshSockets.SocketId.AdditionalSocketX` can be selected and then also be used in the code.


When the LeftHand or RightHand socket is selected a simple [inverse kinematics](https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/constraints/TwoBoneIKConstraint.html) (IK) animation on the hand will be played, for all other sockets the animation will be played on the right hand.
The item can also specify a <xref:i5.VirtualAgents.Item.GrabTarget>, this is where the IK animation will aim for and also what will we placed at the socket.
The hand of the agent will also automatically be rotated towards the <xref:i5.VirtualAgents.Item.GrabTarget>.

Items can be dropped with a <xref:i5.VirtualAgents.AgentTasks.AgentDropTask> or the shortcut <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.DropItem*>.
This will detach the item from the agent and invoke the <xref:i5.VirtualAgents.Item.dropEvent> of the Item.
As seen in the [example](items.md#example-scenes) this could be used to reactivate physics on the item.

## Requirements
1. The agent needs the `MeshSockets` and `RigBuilder` components, as well as everything that is part of the `AnimationRigging` child object in the agent prefab, in which the Sockets for the item positions are defined as well as the inverse kinematics animation for the grab animation.
   For more information on the used Unity Package see [animation rigging package](https://docs.unity3d.com/Packages/com.unity.animation.rigging@1.3/manual/index.html).
2. Items that should be picked up need the <xref:i5.VirtualAgents.Item> component.
    1. The Items <xref:i5.VirtualAgents.Item.CanBePickedUp> method needs to return true.
    2. (optional) A <xref:i5.VirtualAgents.Item.GrabTarget> can be defined, that's where the agent will move its hand position and rotation wise and what attaches to the specified socked when picking the object up. The default is the local zero coordinates of the item.


## Example Scenes

The framework contains an example scene that demonstrates the ability to pick up and drop items.

The samples contain a `ItemController` that controls the movement of the items in the scene, moving them in squares as long as there are not picked up.
It also listens to the items <xref:i5.VirtualAgents.Item.dropEvent> and activates the Rigidbody physics of the sword item when the event is invoked.
The `ItemPickUpSampleController` first adds a movement task and a pickup task for each item in the scene, as defined in the controller object.
Two swords will be picked up by the right hand, the first one gets attached to the hand socket, while the second one gets attached to the spine socket.
The third item is a ring that gets picked up and attached to the left hand.
The last item is a pill formed item, that will be picked up and held in the left hand.
After that the first item is dropped at a specified point.
The second item is dropped by calling the method that drops all items at a specific point.

## [Adaptive Gaze](adaptive-gaze.md)

To make an agent appear more life-like it can automatically look at objects and items in front of him, swerve between the objects, idle in between and pick up on sudden interest changes, like objects that start to move.
To make the agent look at objects that are about to be picked up the <xref:i5.VirtualAgents.AdaptiveGazeTarget> component can be added to the object.
It will automatically be deactivated when the object is picked up.

See [adaptive gaze](adaptive-gaze.md) for more information.