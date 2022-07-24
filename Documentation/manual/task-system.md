# Task System

A possible way to influence the agent's behavior is to assign tasks to the agent which the agent then works on.

## Tasks

Tasks are the elements which tell the agent what to do.

### Structure

All tasks need to implement the <xref:i5.VirtualAgents.TaskSystem.IAgentTask> interface.
It determines the necessary methods that a task must have like <xref:i5.VirtualAgents.TaskSystem.IAgentTask.Execute(i5.VirtualAgents.Agent)> to start the task's execution or <xref:i5.VirtualAgents.TaskSystem.IAgentTask.Update> which is called every frame during the execution.

### Pre-Implemented Tasks

The Virtual Agents Framework contains an ever-growing collection of pre-implemented tasks.
These tasks are meant as a starting point for composing behaviors of agents.
Currently, the following tasks exist and can already be used:
- <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentAnimationTask>: Play animations on the agent
- <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentMovementTask>: Let the agent walk to a given location
- <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentWaitTask>: Let the agent wait for a specific amount of time

### Adding Own Tasks

In addition to the pre-implemented tasks, developers can also add own tasks that implement the <xref:i5.VirtualAgents.TaskSystem.IAgentTask> interface.
It is recommended to inherit from the <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentBaseTask> class when creating new tasks.
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
After that, call <xref:i5.VirtualAgents.Agent.ScheduleTask*> to schedule the task on a specific agent.
Optionally, you can set the priority of the task.
By default, it is set to 0, so negative values will be executed after all other tasks and positive values take priority over default tasks.
The higher the number the earlier the task will be executed.
Moreover, you can provide a layer argument to specify which animation layer the task affects.
The agent is set up with different layers so that multiple actions can happen in parallel.
By default the "Base Layer" is chosen, so it affects the entire body of the agent.
For more information on parallel layers see the documentation on [parallel tasks](parallel-tasks.md).

### Shortcuts

In order to keep the code brief and understandable, it is not always necessary to create the task instance object yourself and to schedule it on the agent explicitly.
For common actions, it is also possible to call one of the shortcut functions on the agent.
Currently, the following shortcut functions exist:
- <xref:i5.VirtualAgents.TaskSystem.TaskActions.GoTo*>
  - <xref:i5.VirtualAgents.TaskSystem.TaskActions.GoTo(Vector3,System.Int32)>: Let the agent walk to the specified coordinates.
  - <xref:i5.VirtualAgents.TaskSystem.TaskActions.GoTo(Transform,Vector3,System.Int32)>: Let the agent walk to the specified transform of an object in the scene.
  You can add an optional offset so that the agent does not run into the object but stops next to it.
  - <xref:i5.VirtualAgents.TaskSystem.TaskActions.GoTo(GameObject,Vector3,System.Int32)>: Let the agent walk to the specified object in the scene.
  You can add an optional offset so that the agent does not run into the object but stops next to it.
- <xref:i5.VirtualAgents.TaskSystem.TaskActions.WaitForSeconds*>: The agent waits for the given amount of seconds.
- <xref:i5.VirtualAgents.TaskSystem.TaskActions.PlayAnimation*>: Play an animation for the given time.
Specify a start and stop trigger which will cause the animation to start and stop in the animator.
If you add you own animations, set them up in a way that there are transitions in and out of your animation with the start and stop triggers set as conditions for entering the transition.
