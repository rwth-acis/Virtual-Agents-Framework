TaskActionsCompletionTests Result: (Test creates this table in the logs when run)

| Task | IsFinished | State | OnTaskFinishedCalled | ManagerOnTaskFinishedCalled |
| --- | --- | --- | --- | --- |
| GoTo(Vector3) | False ❌| Success | False ❌| True ✅ |
| GoTo(Transform) | False ❌| Success | False ❌| True ✅ |
| GoTo(GameObject) | False ❌| Success | False ❌| True ✅ |
| WaitForSeconds | False ❌| Success | False ❌| True ✅ |
| PlayAnimation | True ✅ | Success | True ✅ | True ✅ |
| PickUp | True ✅ | Success | True ✅ | False ❌|
| DropItem(GameObject) | True ✅ | Success | True ✅ | True ✅ |
| DropItem() | True ✅ | Success | True ✅ | True ✅ |
| GoToAndPickUp | False ❌| Success | False ❌| True ✅ |
| GoToAndDropItem(Vector3) | False ❌| Success | False ❌| True ✅ |
| GoToAndDropItem(Transform) | False ❌| Success | False ❌| True ✅ |
| StartAdaptiveGazeForTime(start) | True ✅ | Success | True ✅ | False ❌|
| StartAdaptiveGazeForTime(stop) | True ✅ | Success | True ✅ | False ❌|
| ActivateOrDeactivateAdaptiveGaze(True) | True ✅ | Success | True ✅ | True ✅ |
| ActivateOrDeactivateAdaptiveGaze(false) | True ✅ | Success | True ✅ | True ✅ |
| PointAt | True ✅ | Success | True ✅ | True ✅ |


isFinished and OnTaskfinishedCalled is usually not set/called for tasks that are ended by EvaluateTaskState from the BaseTask class, instead of the FinishTask and FinishTaskAsFailed from the AgentBaseTask class.

ManagerOnTaskFinishedCalled indicates whether the AgentTaskManager's OnTaskFinished event was called for the task.
False calls are less a structural problem and more a problem with how the TaskAction is set up for that specific task and what the test expects, e.g. task is finished in TaskBundle but not in the manager.

Note: This is not comprehensive. Some TaskActions will have different behavior for different cases.
