using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace i5.VirtualAgents
{
    public class TestAllSamples
#if !SAMPLES_PACKAGED
: IPrebuildSetup, IPostBuildCleanup
#endif
    {
#if !SAMPLES_PACKAGED

        // Paths to the scenes that are included in the samples
        private readonly Dictionary<string, string> pathToScenes = new()
        {
            { "Navigation", "Assets/Virtual Agents Framework/Samples/Navigation Sample/Navigation Sample.unity" },
            { "Dynamic Navigation", "Assets/Virtual Agents Framework/Samples/DynamicNavigation/Dynamic Navigation Sample.unity" },
            { "Item Pick Up", "Assets/Virtual Agents Framework/Samples/ItemPickUpSample/Item Pick Up Sample.unity" },
            { "Adaptive Gaze", "Assets/Virtual Agents Framework/Samples/AdaptiveGazeSample/Adaptive Gaze Sample.unity" },
            { "Aiming", "Assets/Virtual Agents Framework/Samples/Aiming Sample/Aiming Sample.unity" },
            { "Independent Tasks", "Assets/Virtual Agents Framework/Samples/Parallel Tasks Sample/Independent Tasks/Independent Tasks Sample.unity" },
            { "Synchronized Tasks", "Assets/Virtual Agents Framework/Samples/Parallel Tasks Sample/Synchronized Tasks/Synchronized Tasks Sample.unity" },
            { "Wait", "Assets/Virtual Agents Framework/Samples/Wait Sample/Wait Sample.unity" },
            { "TaskBundle", "Assets/Virtual Agents Framework/Samples/TaskBundle Sample/TaskBundle Sample.unity" },
            { "Behaviour", "Assets/Virtual Agents Framework/Tests/Runtime/BehaviourTreeTestScene/BehaviourTreeSampleScene.unity" }
        };

        //Method to setup the test, is called once before building the tests (IPrebuildSetup)
        public void Setup()
        {
#if UNITY_EDITOR // This is still executed for the build tests

            //Include all scenes in the build settings

            //Get the current build settings
            var includedScenes = EditorBuildSettings.scenes.ToList();

            //Add the scenes to the build settings if they are not already included
            foreach (var scene in pathToScenes)
            {
                bool alreadyIncluded = false;
                foreach (var sceneInBuildSettings in EditorBuildSettings.scenes)
                {
                    if (sceneInBuildSettings.path == scene.Value && sceneInBuildSettings.enabled == true)
                    {
                        alreadyIncluded = true;
                    }

                }
                if (alreadyIncluded == false)
                {
                    includedScenes.Add(new EditorBuildSettingsScene(scene.Value, true));
                }
            }



            EditorBuildSettings.scenes = includedScenes.ToArray();
#endif
        }
        //Method to cleanup the test, is called once after all tests have been run (IPostBuildCleanup)
        public void Cleanup()
        {
#if UNITY_EDITOR // This is still executed for the build tests
            // Remove the scenes that were added to the build settings
            var includedScenes = EditorBuildSettings.scenes.ToList();
            foreach (var scene in pathToScenes)
            {
                includedScenes.RemoveAll(x => x.path == scene.Value);
            }
            EditorBuildSettings.scenes = includedScenes.ToArray();

#endif
        }

        //Method to setup the test, is called once before each test (SetUp)
        [SetUp]
        public void SetUpTest()
        {
            // Set the time scale to speed up the simulation
            // Also ensures that the simulation can handle frame drops
            Time.timeScale = 10f;
        }
        [TearDown]
        public void TearDownTest()
        {
            // Reset the time scale
            Time.timeScale = 1f;
        }

        [UnityTest]
        public IEnumerator VerifySceneNavigation()
        {
            pathToScenes.TryGetValue("Navigation", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);
            yield return new WaitForSeconds(65);

            var Waypoint = GameObject.Find("Waypoint3");

            var vector3 = Agent.transform.position;
            var expected = Waypoint.transform.position;
            Assert.That(vector3, Is.EqualTo(expected).Using(new Vector3EqualityComparer(1f)));

        }
        [UnityTest]
        public IEnumerator VerifySceneDynamicNavigation()
        {
            pathToScenes.TryGetValue("Dynamic Navigation", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);

            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            yield return new WaitForSeconds(50);

            //TODO: Add more sample specific asserts 
        }
        [UnityTest]
        public IEnumerator VerifySceneItem()
        {
            pathToScenes.TryGetValue("Item Pick Up", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);



            //Check if the agent is moving after 5 seconds
            yield return new WaitForSeconds(5);
            bool isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.True);

            yield return new WaitForSeconds(45);
            //TODO: Add more sample specific asserts 
        }
        [UnityTest]
        public IEnumerator VerifySceneAdaptiveGaze()
        {
            pathToScenes.TryGetValue("Adaptive Gaze", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            //Check if the agent is moving after 5 seconds
            yield return new WaitForSeconds(5);
            bool isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.True);

            yield return new WaitForSeconds(45);

            //TODO: Add more sample specific asserts 
        }
        [UnityTest]
        public IEnumerator VerifySceneAiming()
        {
            pathToScenes.TryGetValue("Aiming", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            yield return new WaitForSeconds(20);

            //TODO: Add more sample specific asserts 
        }
        [UnityTest]
        public IEnumerator VerifySceneIndependentTasks()
        {
            pathToScenes.TryGetValue("Independent Tasks", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            //Check if the agent is moving after 5 seconds
            yield return new WaitForSeconds(5);
            bool isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.True);

            yield return new WaitForSeconds(45);

            //TODO: Add more sample specific asserts 
        }
        [UnityTest]
        public IEnumerator VerifySceneSynchronizedTasks()
        {
            pathToScenes.TryGetValue("Synchronized Tasks", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            //Check if the agent is moving after 5 seconds
            yield return new WaitForSeconds(5);
            bool isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.True);

            yield return new WaitForSeconds(45);

            //TODO: Add more sample specific asserts 
        }
        [UnityTest]
        public IEnumerator VerifySceneWait()
        {
            pathToScenes.TryGetValue("Wait", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            yield return new WaitForSeconds(5);
            bool isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.True);

            yield return new WaitForSeconds(25);

            //Check if the agent has stopped moving
            isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.False);
            //TODO: Add more sample specific asserts
        }

        [UnityTest]
        public IEnumerator VerifyTaskBundle()
        {
            pathToScenes.TryGetValue("TaskBundle", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            yield return new WaitForSeconds(5);
            bool isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.True);

            yield return new WaitForSeconds(30);

            //TODO: Add more sample specific asserts
        }


        [UnityTest]
        public IEnumerator VerifyBehaviourTree()
        {
            pathToScenes.TryGetValue("Behaviour", out string path);
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync(path);
            while (!sceneLoaded.isDone)
            {
                yield return null;
            }
            var Agent = GameObject.Find("AgentStandard");
            Assert.That(Agent, Is.Not.Null);

            //Check if the agent is moving after 5 seconds
            yield return new WaitForSeconds(5);
            bool isMoving = Agent.GetComponent<NavMeshAgent>().velocity != Vector3.zero;
            Assert.That(isMoving, Is.True);

            yield return new WaitForSeconds(45);

            //TODO: Add more sample specific asserts
        }
#endif
    }
}
