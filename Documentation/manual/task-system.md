# Task System

A possible way to influence the agent's behavior is to assign tasks to the agent which the agent then works on.

## Tasks

### Structure

All tasks need to implement the <xref:i5.VirtualAgents.TaskSystem.IAgentTask> interface. It determines the necessary methods that a task must have like <xref:i5.VirtualAgents.TaskSystem.IAgentTask.Execute(i5.VirtualAgents.Agent)> to start the task's execution or <xref:i5.VirtualAgents.TaskSystem.IAgentTask.Update> which is called every frame during the execution.

### Pre-Implemented Tasks

The Virtual Agents Framework contains an ever-growing collection of pre-implemented tasks. These tasks are meant as a starting point for composing behaviors of agents. Currently, the following tasks exist and can already be used:
-	<xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentAnimationTask>: Play animations on the agent
-	<xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentMovementTask>: Let the agent walk to a given location
-	<xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentWaitTask>: Let the agent wait for a specific amount of time

### Adding Own Tasks

In addition to the pre-implemented tasks, developers can also add own tasks that implement the <xref:i5.VirtualAgents.TaskSystem.IAgentTask> interface. It is recommended to inherit from the <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentBaseTask> class when creating new tasks. This base class already contains a lot of common logic for tasks, like marking a task as finished and then automatically invoking the event that is defined in the interface. 

If you have created a generic and configurable task that might be interesting to other developers, feel welcome to post a pull request on GitHub that contributes the task to the framework as a pre-implemented task.

## Task Scheduling

### Task Manager System

### Scheduling a Task Instance on an Agent

### Shortcuts
