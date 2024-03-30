using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;

namespace i5.VirtualAgents
{
    public class TestAgentReachingEndOfPath
    {
#if !SAMPLES_PACKAGED

        [SetUp]
        public void Setup()
        {
            // Set the time scale to speed up the simulation
            // Also ensures that the simulation can handle frame drops
            Time.timeScale = 10f;
        }

        [UnityTest]
        public IEnumerator VerifySceneNavigation()
        {
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/Navigation Sample/Navigation Sample.unity");
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
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/DynamicNavigation/Dynamic Navigation Sample.unity");
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
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/ItemPickUpSample/item Pick Up Sample.unity");
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
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/AdaptiveGazeSample/Adaptive Gaze Sample.unity");
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
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/Aiming Sample/Aiming Sample.unity");
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
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/Parallel Tasks Sample/Independent Tasks/Independent Tasks Sample.unity");
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
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/Parallel Tasks Sample/Synchronized Tasks/Synchronized Tasks Sample.unity");
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
            AsyncOperation sceneLoaded = SceneManager.LoadSceneAsync("Assets/Virtual Agents Framework/Samples/Wait Sample/Wait Sample.unity");
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
