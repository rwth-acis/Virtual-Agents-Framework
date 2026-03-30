using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.ScheduleBasedExecution;

namespace i5.VirtualAgents
{
    public class TaskActionsCompletionTests
    {
        private class TaskExecutionResult
        {
            public string TaskName;
            public bool IsFinished;
            public string State;
            public bool OnTaskFinishedCalled;
            public bool ManagerOnTaskFinishedCalled;
            public bool ReachedTerminalState;
            public string ErrorMessage;

            public bool Passed =>
                string.IsNullOrEmpty(ErrorMessage) &&
                ReachedTerminalState &&
                IsFinished &&
                (State == TaskState.Success.ToString() || State == TaskState.Failure.ToString()) &&
                OnTaskFinishedCalled &&
                ManagerOnTaskFinishedCalled;
        }

        private GameObject floorGameObject;
        private GameObject agentGameObject;
        private GameObject cameraGameObject;
        private GameObject lightGameObject;
        private ScheduleBasedTaskSystem taskSystem;
        private TaskActions taskActions;

        private NavMeshDataInstance navMeshDataInstance;
        private readonly List<GameObject> sceneObjects = new List<GameObject>();
        private Vector3 offset;

        [SetUp]
        public void SetUp()
        {
#if !UNITY_EDITOR
            Assert.Ignore("Skipping test: Loading standard prefabs via AssetDatabase is only supported in the Unity Editor.");
#else
            Time.timeScale = 1f;
            offset = new Vector3(50f, -50f, 50f);

            lightGameObject = new GameObject("Directional Light");
            Light lightComp = lightGameObject.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightGameObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            sceneObjects.Add(lightGameObject);

            floorGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floorGameObject.name = "TestFloor";
            floorGameObject.transform.position = offset;
            floorGameObject.transform.localScale = new Vector3(3f, 1f, 3f);
            sceneObjects.Add(floorGameObject);

            NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);
            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
            NavMeshBuilder.CollectSources(
                floorGameObject.transform,
                LayerMask.GetMask("Default"),
                NavMeshCollectGeometry.PhysicsColliders,
                0,
                markups,
                sources);
            Bounds bounds = new Bounds(offset, new Vector3(100f, 10f, 100f));
            NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(buildSettings, sources, bounds, Vector3.zero, Quaternion.identity);
            navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Virtual Agents Framework/Runtime/Prefabs/AgentStandard.prefab");
            Assert.That(prefab, Is.Not.Null, "AgentStandard prefab not found at the specified path.");

            agentGameObject = Object.Instantiate(prefab, offset, Quaternion.identity);
            taskSystem = agentGameObject.GetComponent<ScheduleBasedTaskSystem>();
            if (taskSystem == null)
            {
                taskSystem = agentGameObject.AddComponent<ScheduleBasedTaskSystem>();
            }
            taskActions = new TaskActions(taskSystem);
            sceneObjects.Add(agentGameObject);

            cameraGameObject = new GameObject("TestCamera");
            Camera cam = cameraGameObject.AddComponent<Camera>();
            cameraGameObject.transform.position = new Vector3(0f, 3f, -8f) + offset;
            cameraGameObject.transform.LookAt(new Vector3(0f, 1.5f, 0f) + offset);
            sceneObjects.Add(cameraGameObject);
#endif
        }

        [TearDown]
        public void TearDown()
        {
            Time.timeScale = 1f;

            foreach (GameObject obj in sceneObjects)
            {
                Object.DestroyImmediate(obj);
            }

            if (navMeshDataInstance.valid)
            {
                navMeshDataInstance.Remove();
            }
        }

        [UnityTest]
        public IEnumerator VerifyTaskActionsSetIsFinishedAndRaiseOnTaskFinished()
        {
            List<TaskExecutionResult> taskResults = new List<TaskExecutionResult>();

            GameObject destinationObject = new GameObject("DestinationObject")
            {
                transform = { position = offset + new Vector3(1.5f, 0f, 0.5f) }
            };
            sceneObjects.Add(destinationObject);

            GameObject secondaryDestination = new GameObject("SecondaryDestination")
            {
                transform = { position = offset + new Vector3(-1.5f, 0f, 1.5f) }
            };
            sceneObjects.Add(secondaryDestination);

            GameObject pointTarget = new GameObject("PointTarget")
            {
                transform = { position = offset + new Vector3(0f, 0f, -3f) }
            };
            sceneObjects.Add(pointTarget);

            GameObject animationAimTarget = new GameObject("AnimationAimTarget")
            {
                transform = { position = offset + new Vector3(2f, 1f, 1f) }
            };
            sceneObjects.Add(animationAimTarget);
            
            GameObject pickupItem = CreatePickableItem("PickupItem", offset + new Vector3(1.6f, 0f, 0.6f));
            GameObject pickupItem2 = CreatePickableItem("PickupItem2", offset + new Vector3(1f, 0f, 1f));

            // Let MonoBehaviour.Start run on freshly created objects (e.g., Item.GrabTarget initialization).
            yield return null;

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.GoTo) + "(Vector3)",
                () => taskActions.GoTo(offset + new Vector3(1f, 0f, 1f)));

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.GoTo) + "(Transform)",
                () => taskActions.GoTo(destinationObject.transform, new Vector3(0.1f, 0f, 0.1f)));

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.GoTo) + "(GameObject)",
                () => taskActions.GoTo(destinationObject, new Vector3(-0.1f, 0f, -0.1f), follow: true));

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.WaitForSeconds),
                () => taskActions.WaitForSeconds(0.2f));

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.PlayAnimation),
                () => taskActions.PlayAnimation("PointingLeft", 0.2f, layer: "Left Arm", aimTarget: animationAimTarget));
            
            pickupItem.transform.position = agentGameObject.transform.position + agentGameObject.transform.forward * 0.3f;
            pickupItem.transform.position = new Vector3(pickupItem.transform.position.x, offset.y, pickupItem.transform.position.z);

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.PickUp),
                () => taskActions.PickUp(pickupItem), timeoutSeconds: 10f);

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.DropItem) + "(GameObject)",
                () => taskActions.DropItem(pickupItem));

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.DropItem) + "()",
                () => taskActions.DropItem());

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.GoToAndPickUp),
                () => taskActions.GoToAndPickUp(pickupItem2), timeoutSeconds: 12f);

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.GoToAndDropItem) + "(Vector3)",
                () => taskActions.GoToAndDropItem(offset + new Vector3(2f, 0f, 0f), pickupItem2), timeoutSeconds: 12f);

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.GoToAndDropItem) + "(Transform)",
                () => taskActions.GoToAndDropItem(secondaryDestination.transform), timeoutSeconds: 12f);

            AgentBaseTask[] adaptiveGazeTasks = null;
            bool adaptiveGazeInvocationFailed = false;
            try
            {
                adaptiveGazeTasks = taskActions.StartAdaptiveGazeForTime(0.2f);
            }
            catch (System.Exception ex)
            {
                adaptiveGazeInvocationFailed = true;
                AddInvocationFailure(taskResults, nameof(TaskActions.StartAdaptiveGazeForTime) + "(start)", ex.Message);
                AddInvocationFailure(taskResults, nameof(TaskActions.StartAdaptiveGazeForTime) + "(stop)", ex.Message);
            }

            if (!adaptiveGazeInvocationFailed && (adaptiveGazeTasks == null || adaptiveGazeTasks.Length != 2))
            {
                AddInvocationFailure(taskResults, nameof(TaskActions.StartAdaptiveGazeForTime) + "(start)", "Expected exactly 2 tasks (start/stop).");
                AddInvocationFailure(taskResults, nameof(TaskActions.StartAdaptiveGazeForTime) + "(stop)", "Expected exactly 2 tasks (start/stop).");
            }
            else if (!adaptiveGazeInvocationFailed)
            {
                yield return CaptureTaskResult(adaptiveGazeTasks[0], nameof(TaskActions.StartAdaptiveGazeForTime) + "(start)", taskResults);
                yield return CaptureTaskResult(adaptiveGazeTasks[1], nameof(TaskActions.StartAdaptiveGazeForTime) + "(stop)", taskResults);
            }

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.ActivateOrDeactivateAdaptiveGaze) + "(true)",
                () =>
                {
                    taskActions.ActivateOrDeactivateAdaptiveGaze(true);
                    if (!TryGetLastScheduledTaskOnLayer("Head", out AgentBaseTask scheduledTask, out string errorMessage))
                    {
                        throw new System.InvalidOperationException(errorMessage);
                    }

                    return scheduledTask;
                });

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.ActivateOrDeactivateAdaptiveGaze) + "(false)",
                () =>
                {
                    taskActions.ActivateOrDeactivateAdaptiveGaze(false);
                    if (!TryGetLastScheduledTaskOnLayer("Head", out AgentBaseTask scheduledTask, out string errorMessage))
                    {
                        throw new System.InvalidOperationException(errorMessage);
                    }

                    return scheduledTask;
                });

            yield return ExecuteTaskAndRecord(taskResults, nameof(TaskActions.PointAt),
                () => taskActions.PointAt(pointTarget, aimLeftArm: true, aimRightArm: false, aimAtTime: 1), timeoutSeconds: 8f);

            string markdownTable = BuildMarkdownResultTable(taskResults);
            Debug.Log("Task action completion results:\n" + markdownTable);

            StringBuilder failureMessageBuilder = new StringBuilder();
            foreach (TaskExecutionResult result in taskResults)
            {
                if (result.Passed)
                {
                    continue;
                }

                failureMessageBuilder.AppendLine($"- {result.TaskName}: IsFinished={result.IsFinished}, state={result.State}, onTaskFinishedCalled={result.OnTaskFinishedCalled}, managerOnTaskFinishedCalled={result.ManagerOnTaskFinishedCalled}, error={result.ErrorMessage ?? "none"}");
            }

            Assert.That(failureMessageBuilder.Length, Is.EqualTo(0),
                "One or more task actions did not complete correctly.\n" + failureMessageBuilder + "\n" + markdownTable);
        }

        private GameObject CreatePickableItem(string name, Vector3 position)
        {
            GameObject itemObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            itemObject.name = name;
            itemObject.transform.position = position;
            Item item = itemObject.AddComponent<Item>();
            item.CanBePickedUp = true;
            sceneObjects.Add(itemObject);
            return itemObject;
        }

        private IEnumerator ExecuteTaskAndRecord(List<TaskExecutionResult> results, string taskName, System.Func<AgentBaseTask> taskFactory, float timeoutSeconds = 8f)
        {
            AgentBaseTask task;
            try
            {
                task = taskFactory();
            }
            catch (System.Exception ex)
            {
                AddInvocationFailure(results, taskName, ex.Message);
                yield break;
            }

            yield return CaptureTaskResult(task, taskName, results, timeoutSeconds);
        }

        private IEnumerator CaptureTaskResult(AgentBaseTask task, string taskName, List<TaskExecutionResult> results, float timeoutSeconds = 8f)
        {
            if (task == null)
            {
                AddInvocationFailure(results, taskName, "Task was null.");
                yield break;
            }

            bool onTaskFinishedCalled = false;
            task.OnTaskFinished += () => onTaskFinishedCalled = true;

            bool managerOnTaskFinishedCalled = false;
            List<AgentTaskManager> subscribedManagers = new List<AgentTaskManager>();
            AgentTaskManager.TaskFinishedEvent managerTaskFinishedHandler = (sender, finishedTask) =>
            {
                if (ReferenceEquals(finishedTask, task))
                {
                    managerOnTaskFinishedCalled = true;
                }
            };

            foreach (AgentTaskManager manager in GetTaskManagers())
            {
                manager.OnTaskFinished += managerTaskFinishedHandler;
                subscribedManagers.Add(manager);
            }

            float startTime = Time.realtimeSinceStartup;
            yield return new WaitUntil(() =>
                task.IsFinished ||
                task.State == TaskState.Success ||
                task.State == TaskState.Failure ||
                Time.realtimeSinceStartup - startTime > timeoutSeconds);

            Debug.Log($"{taskName} results: IsFinished: {task.IsFinished}, state: {task.State}, onTaskFinishedCalled: {onTaskFinishedCalled}");
            bool reachedTerminalState = task.IsFinished || task.State == TaskState.Success || task.State == TaskState.Failure;

            foreach (AgentTaskManager manager in subscribedManagers)
            {
                manager.OnTaskFinished -= managerTaskFinishedHandler;
            }

            results.Add(new TaskExecutionResult
            {
                TaskName = taskName,
                IsFinished = task.IsFinished,
                State = task.State.ToString(),
                OnTaskFinishedCalled = onTaskFinishedCalled,
                ManagerOnTaskFinishedCalled = managerOnTaskFinishedCalled,
                ReachedTerminalState = reachedTerminalState,
                ErrorMessage = reachedTerminalState ? null : $"Task did not finish within {timeoutSeconds} seconds."
            });
        }

        private void AddInvocationFailure(List<TaskExecutionResult> results, string taskName, string errorMessage)
        {
            results.Add(new TaskExecutionResult
            {
                TaskName = taskName,
                IsFinished = false,
                State = "N/A",
                OnTaskFinishedCalled = false,
                ManagerOnTaskFinishedCalled = false,
                ReachedTerminalState = false,
                ErrorMessage = errorMessage
            });
        }

        private string BuildMarkdownResultTable(List<TaskExecutionResult> results)
        {
            StringBuilder markdownBuilder = new StringBuilder();
            markdownBuilder.AppendLine("| Task | IsFinished | State | OnTaskFinishedCalled | ManagerOnTaskFinishedCalled |");
            markdownBuilder.AppendLine("| --- | --- | --- | --- | --- |");

            foreach (TaskExecutionResult result in results)
            {
                markdownBuilder.AppendLine($"| {result.TaskName} | {result.IsFinished} | {result.State} | {result.OnTaskFinishedCalled} | {result.ManagerOnTaskFinishedCalled} |");
            }

            return markdownBuilder.ToString();
        }

        private bool TryGetLastScheduledTaskOnLayer(string layer, out AgentBaseTask task, out string errorMessage)
        {
            task = null;
            errorMessage = null;

            FieldInfo taskManagersField = typeof(ScheduleBasedTaskSystem).GetField("taskManagers", BindingFlags.Instance | BindingFlags.NonPublic);
            if (taskManagersField == null)
            {
                errorMessage = "Could not access internal task manager dictionary.";
                return false;
            }

            Dictionary<string, AgentTaskManager> taskManagers = taskManagersField.GetValue(taskSystem) as Dictionary<string, AgentTaskManager>;
            if (taskManagers == null)
            {
                errorMessage = "Task manager dictionary is null.";
                return false;
            }

            if (!taskManagers.ContainsKey(layer))
            {
                errorMessage = $"Layer '{layer}' does not exist on the task system.";
                return false;
            }

            AgentTaskManager manager = taskManagers[layer];
            FieldInfo lastTaskField = typeof(AgentTaskManager).GetField("lastTask", BindingFlags.Instance | BindingFlags.NonPublic);
            if (lastTaskField == null)
            {
                errorMessage = "Could not access internal lastTask field.";
                return false;
            }

            task = lastTaskField.GetValue(manager) as AgentBaseTask;
            if (task == null)
            {
                errorMessage = $"No task scheduled on layer '{layer}'.";
                return false;
            }

            return true;
        }

        private IEnumerable<AgentTaskManager> GetTaskManagers()
        {
            FieldInfo taskManagersField = typeof(ScheduleBasedTaskSystem).GetField("taskManagers", BindingFlags.Instance | BindingFlags.NonPublic);
            if (taskManagersField == null)
            {
                return new List<AgentTaskManager>();
            }

            Dictionary<string, AgentTaskManager> taskManagers = taskManagersField.GetValue(taskSystem) as Dictionary<string, AgentTaskManager>;
            if (taskManagers == null)
            {
                return new List<AgentTaskManager>();
            }

            return taskManagers.Values;
        }
    }
}

