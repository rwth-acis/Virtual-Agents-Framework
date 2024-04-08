using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace i5.VirtualAgents
{
    public class TestAllSamples : IPrebuildSetup, IPostBuildCleanup
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
            { "Wait", "Assets/Virtual Agents Framework/Samples/Wait Sample/Wait Sample.unity" }
        };

        //Method to setup the test, is called once before building the tests (IPrebuildSetup)
        public void Setup()
        {
            Debug.LogWarning("Known issues: When running tests in Player mode each scene gives one \"no valid NavMesh\", although the NavMesh is working correctly.");
            // Set the time scale to speed up the simulation
            // Also ensures that the simulation can handle frame drops
            Time.timeScale = 10f;

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
            // Reset the time scale
            Time.timeScale = 1f;
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

            yield return new WaitForSeconds(50);

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

            yield return new WaitForSeconds(50);

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

            yield return new WaitForSeconds(25);

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

            yield return new WaitForSeconds(50);

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

            yield return new WaitForSeconds(50);

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

            yield return new WaitForSeconds(20);

            //TODO: Add more sample specific asserts 
        }
#endif
    }
}
