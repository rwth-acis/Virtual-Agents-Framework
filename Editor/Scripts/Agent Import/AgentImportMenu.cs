using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace i5.VirtualAgents
{
    /// <summary>
    /// This script provides a menu item to create an agent from a humanoid model
    /// </summary>
    public class AgentImportMenu : EditorWindow
    {


        [MenuItem("Virtual Agents Framework/Custom Agent Model Import/Create Agent from Humanoid Model")]
        public static void TurnAvatarIntoAgent()
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
            string customPrefabName = "CustomAgentWithoutModel";

            // Find the prefab by name within the project
            string[] prefabGuids = AssetDatabase.FindAssets(prefabName + " t:Prefab");
            string[] customPrefabGuids = AssetDatabase.FindAssets(customPrefabName + " t:Prefab");

            // If a custom Prefab is defined by the user, use that one, otherwise use the default one
            if (customPrefabGuids.Length == 0)
            {
                Debug.Log("Using default preset prefab. Optionally a prefab named \"CustomAgentWithoutModel\" based on the \"com.i5.virtualagents/Runtime/Prefabs/AgentWithoutModel.prefab\" can be used to modify all following imports. ");
                if (prefabGuids.Length == 0)
                {
                    Debug.LogError("Prefab not found: " + prefabName);
                    return;
                }
            }
            else
            {
                prefabGuids = customPrefabGuids;
                Debug.Log("Using custom preset prefab from: " + AssetDatabase.GUIDToAssetPath(prefabGuids[0]));
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

            // Create a copy of the selected object that can be used to move the children out, just copying the children wouldn't keep the connections between the children
            GameObject copyOfSelectedObject = Instantiate(selectedObject);

            // Create a list to store the children
            List<Transform> childrenToMove = new();

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

            // If imported model already has a animator component with a avatar, use that one, otherwise use the default avatar
            if (selectedObject.TryGetComponent<Animator>(out var animator))
            {
                if (animator.avatar != null)
                {
                    Debug.Log("Using Animator avatar provided by the model. ");
                    // Set the avatar to null to avoid problems when the new avatar is the same as the old one
                    instantiatedPrefab.GetComponent<Animator>().avatar = null;
                    // Making sure that the avatar was set to null and that the previous line was not optimized away by the compiler
                    if (instantiatedPrefab.GetComponent<Animator>().avatar != null)
                    {
                        Debug.LogError("Avatar was not successfully set to null, this causes problems, when the new avatar is the same and result in Unity not updating the HumanBones correctly. ");
                    }
                    instantiatedPrefab.GetComponent<Animator>().avatar = animator.avatar;
                }
                // Otherwise the default avatar thats specified in the prefab will be used
            }
            else
            {
                Debug.LogWarning("No Animator component found. Using default animator. This is usually a problem. It is recommended to add a Animator Component with a fitting avatar, usually this happens automatically when importing the model into unity. ");
            }


            // Destroy the cloned object
            DestroyImmediate(copyOfSelectedObject);
            // Set the position of the instantiated prefab next to the position of the original selected object
            instantiatedPrefab.transform.SetPositionAndRotation(selectedObject.transform.position + new Vector3(0, 0, selectedObject.transform.localScale.y), selectedObject.transform.rotation);
            instantiatedPrefab.transform.localScale = selectedObject.transform.localScale;

            Selection.activeGameObject = instantiatedPrefab;
            Selection.activeGameObject.name = "AgentBasedOn" + selectedObject.name;

            CheckAnimatorAvatar();
        }

        private static void CheckAnimatorAvatar()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (!selectedObject.TryGetComponent<Agent>(out _))
            {
                selectedObject.name = "Failed" + selectedObject.name;
                Debug.LogError("No agent component found. Please check that the CustomAgentWithoutModel prefab has an Agent component.");
                return;
            }
            if (!selectedObject.TryGetComponent<Animator>(out var animator))
            {
                selectedObject.name = "Failed" + selectedObject.name;
                Debug.LogError("No Animator component found. Please check that the CustomAgentWithoutModel prefab has an Animator component.");
                return;
            }
            Debug.Log("Checking if the Avatar " + animator.avatar.name + " fits the provided model: ");
            if (animator.GetBoneTransform(HumanBodyBones.Hips) == null && animator.GetBoneTransform(HumanBodyBones.RightLowerArm) == null)
            {
                selectedObject.name = "Failed" + selectedObject.name;
                Debug.LogError("The Avatar " + animator.avatar.name + " doesn't fit the provided model. Please change the Avatar to the Avatar that comes with the Model, e.g. MasculineAnimationAvatar.");
            }
            else
            {
                Debug.Log("The Avatar " + animator.avatar.name + " fits the provided model. Mesh Sockets and Animation Rigging will be set up according to that.");
                FixAnimationRiggingBasedOnAnimatorAvatar(selectedObject, animator);
            }

        }


        private static void FixAnimationRiggingBasedOnAnimatorAvatar(GameObject selectedObject, Animator animator)
        {
            // Add correct Source Objects to MeshSockets
            WeightedTransform weightedTransform = new(animator.GetBoneTransform(HumanBodyBones.RightHand), 1.0f);
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

            //TODO: The following changes only show up once in the inspector/editor and are not actually saved afterwards for yet unknown reasons. For now this is fixed by an automatic failsafe in AgentPickUpTask which is computationally heavy  
            MeshSockets meshSockets = selectedObject.GetComponent<MeshSockets>();
            meshSockets.TwoBoneIKConstraintLeftArm.data.root = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            meshSockets.TwoBoneIKConstraintLeftArm.data.mid = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            meshSockets.TwoBoneIKConstraintLeftArm.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftHand);

            meshSockets.TwoBoneIKConstraintRightArm.data.root = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            meshSockets.TwoBoneIKConstraintRightArm.data.mid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            meshSockets.TwoBoneIKConstraintRightArm.data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);
        }
    }
}