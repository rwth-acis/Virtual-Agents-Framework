# Task System

A possible way to influence the agent's behavior is to assign tasks to the agent which the agent then works on.

## Tasks

Tasks are the elements which tell the agent what to do.

### Structure

All tasks need to implement the <xref:i5.VirtualAgents.AgentTasks.IAgentTask> interface.
It determines the necessary methods that a task must have like <xref:i5.VirtualAgents.ITask.StartExecution(i5.VirtualAgents.Agent)> to start the task's execution or <xref:i5.VirtualAgents.ITask.Tick(i5.VirtualAgents.Agent)> which is called every frame during the execution.

### Pre-Implemented Tasks

The Virtual Agents Framework contains an ever-growing collection of pre-implemented tasks.
These tasks are meant as a starting point for composing behaviors of agents.
Currently, the following tasks exist and can already be used:
- <xref:i5.VirtualAgents.AgentTasks.AgentAnimationTask>: Play animations on the agent
- <xref:i5.VirtualAgents.AgentTasks.AgentMovementTask>: Let the agent walk to a given location or follow an object dynamically
- <xref:i5.VirtualAgents.AgentTasks.AgentWaitTask>: Let the agent wait for a specific amount of time
- <xref:i5.VirtualAgents.AgentTasks.AgentPickUpTask>: Let the agent pickup an object next to them
- <xref:i5.VirtualAgents.AgentTasks.AgentPickUpTask>: Let the agent drop an object or all objects that the agent is holding
### Adding Own Tasks

In addition to the pre-implemented tasks, developers can also add own tasks that implement the <xref:i5.VirtualAgents.AgentTasks.IAgentTask> interface.
It is recommended to inherit from the <xref:i5.VirtualAgents.AgentTasks.AgentBaseTask> class when creating new tasks.
This base class already contains a lot of common logic for tasks, like marking a task as finished and then automatically invoking the event that is defined in the interface. 

If you have created a generic and configurable task that might be interesting to other developers, feel welcome to post a pull request on GitHub that contributes the task to the framework as a pre-implemented task.

## Task Scheduling

Tasks are scheduled on agents using a priority queue.
Each agent has a task manager which will evaluate the queue and start the execution of the next task. 

### Scheduling a Task Instance on an Agent

To assign a task to an agent, first create an instance of your task by calling its constructor.

```
MyTask myTask = new MyTask();
```

Configure the task as required, e.g., by passing arguments to the constructor or by setting properties.
After that, call <xref:i5.VirtualAgents.ScheduleBasedExecution.ScheduleBasedTaskSystem.ScheduleTask(IAgentTask,System.Int32,System.String)> to schedule the task on a specific agent.
Optionally, you can set the priority of the task.
By default, it is set to 0, so negative values will be executed after all other tasks and positive values take priority over default tasks.
The higher the number the earlier the task will be executed.
Moreover, you can provide a layer argument to specify which animation layer the task affects.
The agent is set up with different layers so that multiple actions can happen in parallel.
By default, the "Base Layer" is chosen, so it affects the entire body of the agent.
For more information on parallel layers see the documentation on [parallel tasks](parallel-tasks.md).

### Shortcuts

In order to keep the code brief and understandable, it is not always necessary to create the task instance object yourself and to schedule it on the agent explicitly.
For common actions, it is also possible to call one of the shortcut functions on the agent.
Currently, the following shortcut functions exist:
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoTo*>
  - <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoTo(Vector3,System.Int32)>: Let the agent walk to the specified coordinates.
  - <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoTo(Transform,Vector3,System.Int32)>: Let the agent walk to the specified transform of an object in the scene.
  You can add an optional offset so that the agent does not run into the object but stops next to it.
  - <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoTo(GameObject,Vector3,System.Int32,System.Boolean)>: Let the agent walk to the specified object in the scene.
  You can add an optional offset so that the agent does not run into the object but stops next to it. 
  The agent can also follow an object dynamically, so that the agent will follow the object until it is reached. Partial incomplete paths will be allowed, when that option is enabled. 
  
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.WaitForSeconds*>: The agent waits for the given amount of seconds.
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.PlayAnimation*>: Play an animation for the given time.
Specify a start and stop trigger which will cause the animation to start and stop in the animator.
If you add you own animations, set them up in a way that there are transitions in and out of your animation with the start and stop triggers set as conditions for entering the transition.
Specifying a GameObject as an `aimTarget` for the animation, will start inverse kinematics (IK) on the specified layer. This can be used with `NoAnimation` as a start trigger to start the IK with no animation or with animations that benefit from the IK, for example the provided `pointingLeft` and `pointingRight` animation. 
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.PickUp*>: Pick up an item that is currently in reach of the agent, see [items](items.md) for more information
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndPickUp*>: Schedules an GoTo Task that makes the agent walk to the item before trying to pick it up
- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.DropItem*>: Drop the specified item if it is currently hold be the agent, if no item is specified, all items are dropped.

- <xref:i5.VirtualAgents.ScheduleBasedExecution.TaskActions.GoToAndDropItem*>: Schedules an GoTo Task that makes the agent walk to the specified coordinates or transform before dropping the specified item or all items, if no item is specified.
