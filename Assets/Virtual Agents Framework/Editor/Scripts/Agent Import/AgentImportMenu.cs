using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace i5.VirtualAgents
{
    /// <summary>
    /// 
    /// </summary>
    public class AgentImportMenu : EditorWindow
    {


        [MenuItem("Virtual Agents Framework/Custom Agent Model Import/1. Create Agent from Humanoid Model")]
        public static void turnAvataraIntoAgent()
        {
            // Get the selected GameObject
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                Debug.LogWarning("Please select the custom first parent of the custom model.");
                return;
            }

            // Specify the name to the existing prefab
            string prefabName = "AgentWithoutModel";

            // Find the prefab by name within the project
            string[] prefabGuids = AssetDatabase.FindAssets(prefabName + " t:Prefab");

            if (prefabGuids.Length == 0)
            {
                Debug.LogError("Prefab not found: " + prefabName);
                return;
            }

            // Load the prefab
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuids[0]);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError("Prefab not found at path: " + prefabPath);
                return;
            }

            // Instantiate the prefab into the scene
            GameObject instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            // Create a copy of the selected object that can be used to move the children out, just copieng the children wouldn't keep the connections between the children
            GameObject copyOfSelectedObject = Instantiate(selectedObject);
            
            // Create a list to store the children
            List<Transform> childrenToMove = new List<Transform>();

            // Iterate over the children and add them to the list
            foreach (Transform child in copyOfSelectedObject.transform)
            {
                childrenToMove.Add(child);
            }

            // Move the children to the new parent
            foreach (Transform child in childrenToMove)
            {
                child.SetParent(instantiatedPrefab.transform, false);
            }


            // Destroy the clonedObject
            DestroyImmediate(copyOfSelectedObject);
            // Set the position of the instantiated prefab next to the position of the original selected object
            instantiatedPrefab.transform.position = selectedObject.transform.position + new Vector3(0,0, selectedObject.transform.localScale.y);
            instantiatedPrefab.transform.rotation = selectedObject.transform.rotation;
            instantiatedPrefab.transform.localScale = selectedObject.transform.localScale;

            Selection.activeGameObject = instantiatedPrefab;
            Selection.activeGameObject.name = "AgentBasedOn" + selectedObject.name;
            Debug.Log("Children added to the existing prefab and instantiated in the scene. Please keep the new agent selected and continue with step 2.");



        }
        [MenuItem("Virtual Agents Framework/Custom Agent Model Import/2. Change Animator Avatar & Assign Bones")]
        public static void changeAnimatorAvatar()
        {
            GameObject selectedObject = Selection.activeGameObject;
            
            if(selectedObject == null)
            {
                Debug.LogWarning("Please select the parent object of the agent that has been created in the first step. Normally, it is named AgendBasedOnXYZ.");
                return;
            }

            if (!selectedObject.TryGetComponent<Agent>(out _))
            {
                Debug.LogWarning("Please select the parent object of the agent that has been created in the first step. Normally, it is named AgendBasedOnXYZ.");
                return;
            }
            if (!selectedObject.TryGetComponent<Animator>(out var animator))
            {
                Debug.LogWarning("Please select the parent object of the agent that has been created in the first step. Normally, it is named AgendBasedOnXYZ.");
                return;
            }

            Debug.LogWarning("This step has to be done manually: To change the Animator Avatar select the AgendBasedOnX Gameobject and the Animator component. In the Animator change the Avatar to the Avatar that comes with the Avatar, i.g. MaxculineAnimationAvatar.");
            Debug.Log("Checking if the Avatar " + animator.avatar.name + " fits the provided model: ");
            if (animator.GetBoneTransform(HumanBodyBones.Hips) == null && animator.GetBoneTransform(HumanBodyBones.RightLowerArm) == null)
            {
                Debug.LogError("The Avatar " + animator.avatar.name + " doesn't fit the provided model. Please change the Avatar to the Avatar that comes with the Avatar, i.g. MaxculineAnimationAvatar.");
            }
            else 
            {
                Debug.Log("The Avatar " + animator.avatar.name + " fits the provided model. Mesh Sockets and Animation Rigging will be set up according to that.");
                fixAnimationRiggingBasedOnAnimatoravatar(selectedObject, animator);
            }

        }

        
        private static void fixAnimationRiggingBasedOnAnimatoravatar(GameObject selectedObject, Animator animator)
        {
            //Add correct Source Objects to MeshSockets
            WeightedTransform weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.RightHand), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/RightHandSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.LeftHand), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/LeftHandSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.RightLowerArm), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/RightLowerArmSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/LeftLowerArmSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.RightUpperArm), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/RightUpperArmSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.LeftUpperArm), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/LeftUpperArmSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);

            weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.Chest), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/RightBackSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/LeftBackSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);

            weightedTransform = new WeightedTransform(animator.GetBoneTransform(HumanBodyBones.Hips), 1.0f);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/HipsBackLeftSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/HipsBackRightSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);
            selectedObject.transform.Find("AnimationRigging/MeshSockets/HipsFrontLeftSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);

            selectedObject.transform.Find("AnimationRigging/MeshSockets/HipsFrontRightSocket").GetComponent<MultiParentConstraint>().data.sourceObjects.Add(weightedTransform);

        }
    }
}