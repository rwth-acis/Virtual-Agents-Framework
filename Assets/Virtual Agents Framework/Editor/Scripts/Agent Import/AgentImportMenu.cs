using System.Collections.Generic;
using System.Linq;
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
                Debug.LogWarning("No Animator component found. Using default animator. This is might a problem. It is recommended to add a Animator Component with a fitting avatar, usually this happens automatically when importing the model as ab FBX file into Unity.");
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
            Debug.Log("Checking if the Avatar " + (animator.avatar ? animator.avatar.name : "null") + " fits the provided model: ");
            
            bool isAvatarValid = animator.avatar != null && 
                                 animator.GetBoneTransform(HumanBodyBones.Hips) != null && 
                                 animator.GetBoneTransform(HumanBodyBones.RightLowerArm) != null;

            if (!isAvatarValid)
            {
                Debug.LogWarning("Avatar is invalid or missing bones. Attempting automatic fix for hierarchy...");
                
                if (TryCreateAutomaticAvatar(selectedObject, animator))
                {
                    Debug.Log("Successfully created and assigned a new Avatar for the hierarchy.");
                    FixAnimationRiggingBasedOnAnimatorAvatar(selectedObject, animator);
                }
                else
                {
                    selectedObject.name = "Failed" + selectedObject.name;
                    Debug.LogError("Automatic fix failed. The model hierarchy does not match the known structure, or the Avatar is fundamentally incompatible.");
                }
            }
            else
            {
                Debug.Log("The Avatar fits the provided model. Mesh Sockets and Animation Rigging will be set up according to that.");
                FixAnimationRiggingBasedOnAnimatorAvatar(selectedObject, animator);
            }
        }
        
        /// <summary>
        /// Attempts to build a Humanoid Avatar for some naming convention, e.g. Ready Player Me avatars.
        /// </summary>
        private static bool TryCreateAutomaticAvatar(GameObject rootObject, Animator animator)
        {
            // Verify the basic hierarchy
            Transform hips = FindRecursive(rootObject.transform, "Hips");
            if (hips == null) return false;

            Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();
            EnforceTPose(rootObject.transform, originalRotations);
            
            HumanDescription description = new HumanDescription();
            
            // 1. Setup Skeleton (List of all bones in the hierarchy)
            List<SkeletonBone> skeletonBones = new List<SkeletonBone>();
            // We need to traverse the entire hierarchy to build the skeleton definition
            foreach (Transform t in rootObject.GetComponentsInChildren<Transform>())
            {
                SkeletonBone bone = new SkeletonBone();
                bone.name = t.name;
                bone.position = t.localPosition;
                bone.rotation = t.localRotation;
                bone.scale = t.localScale;
                skeletonBones.Add(bone);
            }
            description.skeleton = skeletonBones.ToArray();

            // 2. Setup Human (Mapping from Bone Name -> Unity HumanBone)
            List<UnityEngine.HumanBone> humanBones = new List<UnityEngine.HumanBone>();
            
            // Helper to add mapping if bone exists in hierarchy
            void AddMap(string boneName, string humanName) {
                if (skeletonBones.Any(b => b.name == boneName)) {
                    humanBones.Add(new UnityEngine.HumanBone { boneName = boneName, humanName = humanName, limit = new HumanLimit { useDefaultValues = true } });
                }
            }

            // -- Body --
            AddMap("Hips", "Hips");
            AddMap("Spine", "Spine");
            AddMap("Spine1", "Chest");
            AddMap("Spine2", "UpperChest");
            AddMap("Neck", "Neck");
            AddMap("Head", "Head");

            // -- Legs --
            AddMap("LeftUpLeg", "LeftUpperLeg");
            AddMap("LeftLeg", "LeftLowerLeg");
            AddMap("LeftFoot", "LeftFoot");
            AddMap("LeftToeBase", "LeftToes");
            
            AddMap("RightUpLeg", "RightUpperLeg");
            AddMap("RightLeg", "RightLowerLeg");
            AddMap("RightFoot", "RightFoot");
            AddMap("RightToeBase", "RightToes");

            // -- Arms --
            AddMap("LeftShoulder", "LeftShoulder");
            AddMap("LeftArm", "LeftUpperArm");
            AddMap("LeftForeArm", "LeftLowerArm");
            AddMap("LeftHand", "LeftHand");

            AddMap("RightShoulder", "RightShoulder");
            AddMap("RightArm", "RightUpperArm");
            AddMap("RightForeArm", "RightLowerArm");
            AddMap("RightHand", "RightHand");

            // -- Fingers --
            // Left Hand
            AddMap("LeftHandThumb1", "Left Thumb Proximal");
            AddMap("LeftHandThumb2", "Left Thumb Intermediate");
            AddMap("LeftHandThumb3", "Left Thumb Distal");

            AddMap("LeftHandIndex1", "Left Index Proximal");
            AddMap("LeftHandIndex2", "Left Index Intermediate");
            AddMap("LeftHandIndex3", "Left Index Distal");

            AddMap("LeftHandMiddle1", "Left Middle Proximal");
            AddMap("LeftHandMiddle2", "Left Middle Intermediate");
            AddMap("LeftHandMiddle3", "Left Middle Distal");

            AddMap("LeftHandRing1", "Left Ring Proximal");
            AddMap("LeftHandRing2", "Left Ring Intermediate");
            AddMap("LeftHandRing3", "Left Ring Distal");

            AddMap("LeftHandPinky1", "Left Little Proximal");
            AddMap("LeftHandPinky2", "Left Little Intermediate");
            AddMap("LeftHandPinky3", "Left Little Distal");

            // Right Hand
            AddMap("RightHandThumb1", "Right Thumb Proximal");
            AddMap("RightHandThumb2", "Right Thumb Intermediate");
            AddMap("RightHandThumb3", "Right Thumb Distal");

            AddMap("RightHandIndex1", "Right Index Proximal");
            AddMap("RightHandIndex2", "Right Index Intermediate");
            AddMap("RightHandIndex3", "Right Index Distal");

            AddMap("RightHandMiddle1", "Right Middle Proximal");
            AddMap("RightHandMiddle2", "Right Middle Intermediate");
            AddMap("RightHandMiddle3", "Right Middle Distal");

            AddMap("RightHandRing1", "Right Ring Proximal");
            AddMap("RightHandRing2", "Right Ring Intermediate");
            AddMap("RightHandRing3", "Right Ring Distal");

            AddMap("RightHandPinky1", "Right Little Proximal");
            AddMap("RightHandPinky2", "Right Little Intermediate");
            AddMap("RightHandPinky3", "Right Little Distal");
            

            description.human = humanBones.ToArray();

            // 3. Build the Avatar
            Avatar newAvatar = AvatarBuilder.BuildHumanAvatar(rootObject, description);
            
            if (newAvatar != null && newAvatar.isValid)
            {
                newAvatar.name = "AutoGeneratedAvatar";
                animator.avatar = newAvatar;
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Forces the Upper Arms to align horizontally (T-Pose) based on the character's facing direction.
        /// </summary>
        private static void EnforceTPose(Transform root, Dictionary<Transform, Quaternion> undoList)
        {
            // Find all necessary bone transforms
            Transform leftArm = FindRecursive(root, "LeftArm");
            Transform leftForeArm = FindRecursive(root, "LeftForeArm");
            Transform leftHand = FindRecursive(root, "LeftHand");

            Transform rightArm = FindRecursive(root, "RightArm");
            Transform rightForeArm = FindRecursive(root, "RightForeArm");
            Transform rightHand = FindRecursive(root, "RightHand");

            // Local helper to align a bone segment to a target direction
            void AlignBone(Transform boneToRotate, Transform childBone, Vector3 targetDirection)
            {
                if (boneToRotate == null || childBone == null) return;

                // 1. Save original rotation for restoration later
                if (!undoList.ContainsKey(boneToRotate))
                {
                    undoList[boneToRotate] = boneToRotate.localRotation;
                }

                // 2. Calculate the current direction of the bone (vector to its child)
                Vector3 currentDirection = (childBone.position - boneToRotate.position).normalized;

                // 3. Calculate the rotation needed to align current direction to target
                Quaternion rotationFix = Quaternion.FromToRotation(currentDirection, targetDirection);

                // 4. Apply the rotation
                boneToRotate.rotation = rotationFix * boneToRotate.rotation;
            }

            // --- LEFT SIDE (Target: Negative Right / Local -X) ---
            Vector3 leftTarget = -root.right;
            
            // Step 1: Straighten Upper Arm (Aligns Shoulder -> Elbow)
            AlignBone(leftArm, leftForeArm, leftTarget);
            
            // Step 2: Straighten Forearm (Aligns Elbow -> Hand)
            AlignBone(leftForeArm, leftHand, leftTarget);


            // --- RIGHT SIDE (Target: Positive Right / Local +X) ---
            Vector3 rightTarget = root.right;

            // Step 1: Straighten Upper Arm (Aligns Shoulder -> Elbow)
            AlignBone(rightArm, rightForeArm, rightTarget);

            // Step 2: Straighten Forearm (Aligns Elbow -> Hand)
            AlignBone(rightForeArm, rightHand, rightTarget);
        }
        
        private static Transform FindRecursive(Transform current, string name)
        {
            if (current.name == name) return current;
            foreach (Transform child in current)
            {
                var found = FindRecursive(child, name);
                if (found != null) return found;
            }
            return null;
        }

        private static void FixAnimationRiggingBasedOnAnimatorAvatar(GameObject selectedObject, Animator animator)
        {
            // 1. Validation Checks
            bool isAvatarValid = animator.avatar != null &&
                                 animator.GetBoneTransform(HumanBodyBones.Hips) != null &&
                                 animator.GetBoneTransform(HumanBodyBones.RightLowerArm) != null;

            if (!isAvatarValid)
            {
                Debug.LogError("Cannot set up Animation Rigging: Avatar is invalid or missing critical bones.");
                return;
            }

            // 2. Helper Method
            void AddSourceToConstraint(string socketPath, HumanBodyBones boneType)
            {
                Transform bone = animator.GetBoneTransform(boneType);
                if (bone == null)
                {
                    Debug.LogWarning($"Bone {boneType} not found in Animator. Skipping {socketPath}.");
                    return;
                }

                Transform socket = selectedObject.transform.Find(socketPath);
                if (socket == null)
                {
                    Debug.LogError($"Socket path not found: {socketPath}");
                    return;
                }

                var constraint = socket.GetComponent<MultiParentConstraint>();
                if (constraint != null)
                {
                    WeightedTransform newSource = new WeightedTransform(bone, 1.0f);
                    WeightedTransformArray sources = new WeightedTransformArray{ newSource };

                    constraint.data.sourceObjects = sources;
                }
                else
                {
                    Debug.LogError($"MultiParentConstraint missing on {socket.name}");
                }
            }

            // 3. Apply constraints using the helper
            Debug.Log("Starting Animation Rigging Setup...");

            AddSourceToConstraint("AnimationRigging/MeshSockets/RightHandSocket", HumanBodyBones.RightHand);
            AddSourceToConstraint("AnimationRigging/MeshSockets/LeftHandSocket", HumanBodyBones.LeftHand);
            
            AddSourceToConstraint("AnimationRigging/MeshSockets/RightLowerArmSocket", HumanBodyBones.RightLowerArm);
            AddSourceToConstraint("AnimationRigging/MeshSockets/LeftLowerArmSocket", HumanBodyBones.LeftLowerArm);
            
            AddSourceToConstraint("AnimationRigging/MeshSockets/RightUpperArmSocket", HumanBodyBones.RightUpperArm);
            AddSourceToConstraint("AnimationRigging/MeshSockets/LeftUpperArmSocket", HumanBodyBones.LeftUpperArm);

            // For sockets that share the same bone (Chest/Spine), we call it multiple times
            AddSourceToConstraint("AnimationRigging/MeshSockets/RightBackSocket", HumanBodyBones.Chest);
            AddSourceToConstraint("AnimationRigging/MeshSockets/LeftBackSocket", HumanBodyBones.Chest);

            // For Hips
            AddSourceToConstraint("AnimationRigging/MeshSockets/HipsBackLeftSocket", HumanBodyBones.Hips);
            AddSourceToConstraint("AnimationRigging/MeshSockets/HipsBackRightSocket", HumanBodyBones.Hips);
            AddSourceToConstraint("AnimationRigging/MeshSockets/HipsFrontLeftSocket", HumanBodyBones.Hips);
            AddSourceToConstraint("AnimationRigging/MeshSockets/HipsFrontRightSocket", HumanBodyBones.Hips);
            
            
            MeshSockets meshSockets = selectedObject.GetComponent<MeshSockets>();
            if (meshSockets == null)
            {
                Debug.LogWarning("MeshSockets component not found on the used prefab. Skipping Two Bone IK setup. Please check that the CustomAgentWithoutModel prefab has a MeshSockets component with the correct socket structure.");
            }
            else
            {
                meshSockets.TwoBoneIKConstraintLeftArm.data.root = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                meshSockets.TwoBoneIKConstraintLeftArm.data.mid = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                meshSockets.TwoBoneIKConstraintLeftArm.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftHand);

                meshSockets.TwoBoneIKConstraintRightArm.data.root = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                meshSockets.TwoBoneIKConstraintRightArm.data.mid = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                meshSockets.TwoBoneIKConstraintRightArm.data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);
            }
            
            EditorUtility.SetDirty(selectedObject);
            EditorUtility.SetDirty(meshSockets);
            
            Debug.Log("Successfully set up Animation Rigging based on the Animator's Avatar.");

        }
    }
}