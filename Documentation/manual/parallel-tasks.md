# Parallel Tasks

Tasks cannot only be executed sequentially but also in parallel if they, e.g., affect different regions of the agent's body.
This means that an agent can, e.g., walk while waving its hand at the same time.
To realize this, the Virtual Agents Framework provides a parallel tasks structure where tasks can be scheduled independently but also be synchronized using dependencies between tasks.

## Parallel Structure

The agent consists of a series of task managers which determine which task to execute next.
By calling <xref:i5.VirtualAgents.Agent.ScheduleTask*> without specifying a layer, the task, by default, affects the entire body.
Apart from this, there is a separate task manager for each relevant body region of the agent.
Currently, this includes the following body regions:
-	Left Arm
-	Right Arm
-	Head

When applying parallel tasks, the main use case is to layer animations.
It is recommended to always assign an animation task to the base layer and then overwrite a specific body region with a custom animation.
In the example of the walking agent which is waving its hand, the walking animation would be set up as a task on the base layer.
The waving animation is scheduled as a task on one of the two arm regions.
As a result, the agent displays the full walking movements and only its arm is concerned with the waving motion.

## Synchronizing Task Layers

In some cases, tasks need to wait for each other to start synchronously.
In more complex tasks, this can be achieved by implementing a start condition on the tasks using the <xref:i5.VirtualAgents.TaskSystem.IAgentTask.CanStart*> property.
This pre-condition can contain any Boolean expression.
A task can, e.g., wait for another task but it can also wait for a specific condition in the agent's environment to become true.

Apart from this general purpose start-condition, the <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentBaseTask> also implements a dependency system where tasks can automatically wait for each other.
If task `a` depends on the completion of task `b` and both inherit from <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentBaseTask>, call `a.WaitFor(b)`.
Internally, <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentBaseTask.WaitFor*> creates a pre-condition for task `a`.
The <xref:i5.VirtualAgents.TaskSystem.IAgentTask.CanStart*> property of `a` will only become `true`, once `b` has finished its execution, indicated by its <xref:i5.VirtualAgents.TaskSystem.IAgentTask.IsFinished> property.

If task `a` depends on multiple tasks `b` and `c`, they can quickly be defined in <xref:i5.VirtualAgents.TaskSystem.AgentTasks.AgentBaseTask.WaitFor*> by listing all depending tasks as `a.WaitFor(b, c)`.

## Example Scenes

The framework contains two example scenes: One of them demonstrates the independent execution of parallel tasks.
Here, the agent is assigned a series of walking tasks on its base layer and a combination of waiting and waving tasks on its left arm layer, as well as a head-shaking animation for the head.
The tasks are just executed but do not contain any synchronization.
In contract to this, the synchronization sample contains the same task sequence but here, the second waving animation will wait for the head shaking animation to complete.
Both samples contain a TasksSampleController which demonstrates how to schedule the tasks and how to set the dependencies.
