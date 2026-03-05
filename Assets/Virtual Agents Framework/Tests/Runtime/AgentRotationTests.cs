using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.ScheduleBasedExecution;
using UnityEngine.AI;

namespace i5.VirtualAgents
{
    public class AgentRotationIntegrationTests
    {
        private GameObject floorGameObject;
        private GameObject agentGameObject;
        private GameObject cameraGameObject;
        private GameObject lightGameObject;
        private ScheduleBasedTaskSystem taskSystem;
        private TaskActions taskActions;
        
        private NavMeshDataInstance navMeshDataInstance;
        
        private List<GameObject> sceneObjects = new List<GameObject>();
        
        // Offset for all positions/coordinates to evaluate that the rotation works not only at the origin
        private Vector3 offset; 

        [SetUp]
        public void SetUp()
        {
#if !UNITY_EDITOR
                // Stop executing test and marks it as "Skipped/Ignored" in the runner
                Assert.Ignore("Skipping test: Loading standard prefabs via AssetDatabase is only supported in the Unity Editor.");
#else
            
            // Speed up simulation
            Time.timeScale = 10f;
            
            // Offset for all positions/coordinates to evaluate that the rotation works not only at the origin
            offset = new Vector3(50f, -50f, 50f);
            
            // Lighting setup
            lightGameObject = new GameObject("Directional Light");
            Light lightComp = lightGameObject.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightGameObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            sceneObjects.Add(lightGameObject);
            
            // Floor setup
            floorGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floorGameObject.name = "TestFloor";
            floorGameObject.transform.position = Vector3.zero + offset;
            floorGameObject.transform.localScale = new Vector3(3f, 1f, 3f); 
            sceneObjects.Add(floorGameObject);
            
            // NavMesh setup
            NavMeshBuildSettings buildSettings = NavMesh.GetSettingsByID(0);
            List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
            List<NavMeshBuildMarkup> markups = new List<NavMeshBuildMarkup>();
            NavMeshBuilder.CollectSources(floorGameObject.transform, LayerMask.GetMask("Default"), NavMeshCollectGeometry.PhysicsColliders, 0, markups, sources);
            Bounds bounds = new Bounds(Vector3.zero + offset, new Vector3(100f, 10f, 100f));
            NavMeshData navMeshData = NavMeshBuilder.BuildNavMeshData(buildSettings, sources, bounds, Vector3.zero, Quaternion.identity);
            navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
            
            
            // Agent setup
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Virtual Agents Framework/Runtime/Prefabs/AgentStandard.prefab");
            Assert.That(prefab, Is.Not.Null, "AgentStandard prefab not found at the specified path.");
            agentGameObject = Object.Instantiate(prefab, Vector3.zero + offset, Quaternion.identity);
            taskSystem = agentGameObject.GetComponent<ScheduleBasedTaskSystem>();
            if (taskSystem == null)
            {
                taskSystem = agentGameObject.AddComponent<ScheduleBasedTaskSystem>();
            }
            taskActions = new TaskActions(taskSystem);
            sceneObjects.Add(agentGameObject);
            
            // Camera setup
            cameraGameObject = new GameObject("TestCamera");
            Camera cam = cameraGameObject.AddComponent<Camera>();
            cameraGameObject.transform.position = new Vector3(0, 3f, -8f) + offset;
            cameraGameObject.transform.LookAt(new Vector3(0, 1.5f, 0) + offset);
            sceneObjects.Add(cameraGameObject);
#endif
        }

        [TearDown]
        public void TearDown()
        {
            // Reset time scale
            Time.timeScale = 1f;

            // Clean up the scene after each test
            foreach (var onj in sceneObjects)
            {
                Object.DestroyImmediate(onj);
            }
            
            if (navMeshDataInstance.valid)
            {
                navMeshDataInstance.Remove();
            }
            
        }

        [UnityTest]
        public IEnumerator VerifyRotationTypes()
        {
            GameObject targetGameObject = new GameObject("Target") { transform = { position = new Vector3(5, 0, 5) + offset }}; // 45 degrees
            
            // 1. Rotation to GameObject
            AgentRotationTask rotationTask1 = new AgentRotationTask(targetGameObject);
            taskSystem.ScheduleTask(rotationTask1);
            yield return new WaitUntil(() => rotationTask1.IsFinished);
            Vector3 expectedDirection1 = new Vector3(1, 0, 1).normalized;
            Assert.That(agentGameObject.transform.forward, Is.EqualTo(expectedDirection1).Using(new Vector3EqualityComparer(0.05f)));
            sceneObjects.Add(targetGameObject);
            
            // 2. Rotation to Coordinates
            Vector3 targetCoordinates = new Vector3(-5, 0, 0) + offset; // Left (270 or -90 degrees)
            AgentRotationTask rotationTask2 = new AgentRotationTask(targetCoordinates);
            taskSystem.ScheduleTask(rotationTask2);
            yield return new WaitUntil(() => rotationTask2.IsFinished);
            Vector3 expectedDirection2 = new Vector3(-1, 0, 0);
            Assert.That(agentGameObject.transform.forward, Is.EqualTo(expectedDirection2).Using(new Vector3EqualityComparer(0.05f)));
            
            // 3. Rotation by Angle (Relative)
            // The agent is currently facing Left (-90 degrees). 
            // Rotating BY 90 degrees (positive is right) should make it face Forward (0 degrees).
            AgentRotationTask rotationTask3 = new AgentRotationTask(90f, true);
            taskSystem.ScheduleTask(rotationTask3);
            yield return new WaitUntil(() => rotationTask3.IsFinished);
            Vector3 expectedDirection3 = new Vector3(0, 0, 1);
            Assert.That(agentGameObject.transform.forward, Is.EqualTo(expectedDirection3).Using(new Vector3EqualityComparer(0.05f)));

            // 4. Rotation to Angle (Absolute)
            // Rotating TO 180 degrees should make the agent face Backward, regardless of current rotation.
            AgentRotationTask rotationTask4 = new AgentRotationTask(180f, false);
            taskSystem.ScheduleTask(rotationTask4);
            yield return new WaitUntil(() => rotationTask4.IsFinished);
            Vector3 expectedDirection4 = new Vector3(0, 0, -1);
            Assert.That(agentGameObject.transform.forward, Is.EqualTo(expectedDirection4).Using(new Vector3EqualityComparer(0.05f)));
        }
        
        [UnityTest]
        public IEnumerator VerifyRotationAtDifferentSpeeds()
        {
            // Slow rotation time test
            AgentRotationTask rotationTask = new AgentRotationTask(new Vector3(0, 0, -5) + offset,50);
            taskSystem.ScheduleTask(rotationTask);
            float startTimeSlowRotation = Time.time;
            yield return new WaitUntil(() => rotationTask.IsFinished);
            float elapsedTimeSlowRotation = Time.time - startTimeSlowRotation;
            
            // Normal rotation time test
            rotationTask = new AgentRotationTask(new Vector3(0, 0, 5) + offset,100);
            taskSystem.ScheduleTask(rotationTask);
            float startTimeNormalRotation = Time.time;
            yield return new WaitUntil(() => rotationTask.IsFinished);
            float elapsedTimeNormalRotation = Time.time - startTimeNormalRotation;
            
            // Fast rotation time test
            rotationTask = new AgentRotationTask(new Vector3(0, 0, -5) + offset, 200);
            taskSystem.ScheduleTask(rotationTask);
            float startTimeFastRotation = Time.time;
            yield return new WaitUntil(() => rotationTask.IsFinished);
            float elapsedTimeFastRotation = Time.time - startTimeFastRotation;
            
            
            float threshold = 0.35f;
            Assert.That(elapsedTimeFastRotation, Is.LessThan(elapsedTimeNormalRotation), "Fast rotation should complete quicker than normal rotation");
            Assert.That(elapsedTimeNormalRotation, Is.EqualTo(elapsedTimeSlowRotation / 2f).Within(threshold), "Slow rotation should be approximately half the time of normal rotation");
            Assert.That(elapsedTimeFastRotation, Is.EqualTo(elapsedTimeNormalRotation / 2f).Within(threshold), "Fast rotation should be approximately half the time of normal rotation");
            
            // Check that "Degree per Second" speed setting is applied correctly
            Assert.That(elapsedTimeSlowRotation, Is.EqualTo(180f/50f).Within(threshold), "Slow rotation should complete in 180/50 seconds");
            Assert.That(elapsedTimeNormalRotation, Is.EqualTo(180f/100f).Within(threshold), "Normal rotation should complete in 180/100 seconds");
            Assert.That(elapsedTimeFastRotation, Is.EqualTo(180f/200f).Within(threshold), "Fast rotation should complete in 180/200 seconds");
        }
        
        [UnityTest]
        public IEnumerator VerifyRotationIgnoresYAxis()
        {
            // Target is high in the air and offset on X/Z
            GameObject target = new GameObject("HighTarget")
            { transform = { position = new Vector3(5, 500, 5) + offset } };

            AgentRotationTask rotationTask = new AgentRotationTask(target);
            taskSystem.ScheduleTask(rotationTask);

            yield return new WaitUntil(() => rotationTask.IsFinished);

            // Forward vector should remain flat on the XZ plane
            Vector3 expectedDirection = new Vector3(1, 0, 1).normalized; 
            Assert.That(agentGameObject.transform.forward, Is.EqualTo(expectedDirection).Using(new Vector3EqualityComparer(0.05f)));
            
            // Validate that the agent didn't tilt physically
            Assert.AreEqual(0f, agentGameObject.transform.rotation.eulerAngles.x, 0.05f);
            Assert.AreEqual(0f, agentGameObject.transform.rotation.eulerAngles.z, 0.05f);

            sceneObjects.Add(target);
        }

        [UnityTest]
        public IEnumerator VerifyPointAtWithRotation()
        {
            // Target directly behind agent
            GameObject target = new GameObject("TargetBehind")
            { transform = { position = new Vector3(0, 0, -5) + offset } };

            // PointAt should recognize the angle > 90 and schedule a rotation first
            AgentBaseTask rotationTask = taskActions.PointAt(target, aimLeftArm: true, aimRightArm: false, aimAtTime: 5, priority: 0);
            
            yield return new WaitUntil(() => rotationTask.IsFinished);

            Vector3 expectedDirection = new Vector3(0, 0, -1);
            Assert.That(agentGameObject.transform.forward, Is.EqualTo(expectedDirection).Using(new Vector3EqualityComparer(0.05f)));

            sceneObjects.Add(target);
        }

        [UnityTest]
        public IEnumerator VerifyGoToWithRotation()
        {
            Vector3 destination = new Vector3(3, 0, 3) + offset;
    
            // Create a dynamic obstacle blocking the destination
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.name = "DestinationBlocker";
            obstacle.transform.localScale = new Vector3(2f, 2f, 2f);
            obstacle.transform.position = destination;
            
    
            // Update NavMesh to respect obstacle
            NavMeshObstacle navObstacle = obstacle.AddComponent<NavMeshObstacle>();
            navObstacle.carving = true;
            yield return null;  // Yield a frame to let the NavMesh carving update

            // GoTo schedules an AgentMovementTask followed by an AgentRotationTask
            taskActions.GoTo(destination);

            // Wait enough time for the agent to walk to the obstacle, stop, and rotate
            yield return new WaitForSeconds(4f);

            Vector3 expectedDirection = (destination - agentGameObject.transform.position).normalized; 
            expectedDirection.y = 0; // Ignore Y axis for rotation
            expectedDirection = expectedDirection.normalized; 
            float remainingDistance = Vector3.Distance(agentGameObject.transform.position, destination);
            Assert.That(agentGameObject.transform.forward, Is.EqualTo(expectedDirection).Using(new Vector3EqualityComparer(0.1f)));
            // Check that the agent is next to the destination, but not directly at the destination due to the obstacle
            Assert.That(remainingDistance, Is.GreaterThan(1.0f), "The agent should have stopped short of the destination due to the obstacle.");
            Assert.That(remainingDistance, Is.LessThan(1.5f), "The agent should have stopped short of the destination due to the obstacle.");
            
            // Cleanup
            sceneObjects.Add(obstacle);
        }
    }
}