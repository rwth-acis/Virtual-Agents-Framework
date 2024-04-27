# TaskBundles
TaskBundles are a way to group multiple sub-tasks together.
The resulting task bundle can be added to a task system like any other task.
A TaskBundle can also include a list of boolean preconditions.
They are checked beforehand and determine if the TaskBundle prematurely finishes, in the event that one of the preconditions is not met.
Tasks in a TaskBundle are executed back to back in sequence and cannot be interrupted by other tasks.

# Construct a TaskBundle
The are three TaskBundle constructors, that can be used to create a TaskBundle:
1. `TaskBundle(MonoBehaviour coroutineHost)`
2. `TaskBundle(MonoBehaviour coroutineHost, List<AgentBaseTask> tasks)`
3. `TaskBundle(MonoBehaviour coroutineHost, List<AgentBaseTask> tasks, List<Func <bool>> preconditions)`

The `coroutineHost` is the MonoBehaviour that will host the coroutine that executes the TaskBundle.
`tasks` is a list of subtasks. Of note is, that TaskActions cannot be used here, as they would also add the sub-tasks to the regular scheduler.
`preconditions` is a list of boolean functions, so in particular lambda expressions can be used.
Other functions may be evaluated beforehand, so are not suitable for this purpose.
The lambda functions must return a boolean value. `() => {...}` syntax is a lambda expression that defines an anonymous function inline.
Parameters can be added in brackets on the left side of the arrow.

Example:
```csharp
public List<Transform> waypoints;
List<AgentBaseTask> tasks = new List<AgentBaseTask>();
List<System.Func<bool>> preconditions = new List<System.Func<bool>>();
preconditions.Add(() =>
            {
                // This lambda function checks if the agent is close to the last waypoint
                // Compare vectors equality with accuracy of 0.5
                float distance = Vector3.Distance(waypoints[waypoints.Count -1].position, agent.transform.position);
                return distance > 1.0f;
            });
for (int i = 0; i < waypoints.Count; i++)
            {
                tasks.Add(new AgentMovementTask(waypoints[i].position));
            }
TaskBundle myTaskBundle = new TaskBundle(this, tasks, preconditions);
```


# Example Scene

The framework contains one example scene that more comprehensively demonstrates the use of TaskBundles.
In the `TaskBundleSample` the agent attempts to execute two TaskBundles, of which only one's preconditions are met.

The `TaskBundleController` has the option to add multiple waypoints where the agent should walk.
At the end of the execution of the first TaskBundle, the agent is close to the last waypoint, which leads to the precondition of the second bundle to fail.