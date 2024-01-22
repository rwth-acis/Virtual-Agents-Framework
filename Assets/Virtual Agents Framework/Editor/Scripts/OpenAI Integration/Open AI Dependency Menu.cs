using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;

namespace i5.VirtualAgents.Editor
{
    public class OpenAIDependencyMenu : MonoBehaviour
    {
        [MenuItem("Virtual Agents Framework/OpenAI/Add unofficial OpenAI Helper Package")]
        static void AddDependencies()
        {
            Debug.Log("Added unofficial OpenAI Helper Package to the package menager. Download will automaticlly start in a few seconds...");
            Client.Add("https://github.com/srcnalt/OpenAI-Unity.git");
            
        }

        [MenuItem("Virtual Agents Framework/OpenAI/Remove OpenAI Helper Package")]
        static void RemoveDependencies()
        {
            Debug.Log("Removed unofficial OpenAI Helper Package from the package menager.");
            Client.Remove("com.srcnalt.openai-unity");
            
        }
        [MenuItem("Virtual Agents Framework/OpenAI/Link to OpenAI Helper Package")]
        static void Link()
        {
            Application.OpenURL("https://github.com/srcnalt/OpenAI-Unity");
        }
    }
}
