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
        private enum AvatarCreationResult
        {
            Success,
            PendingManual,
            Failed
        }

        // Expected names of the agent prefabs
        private const string prefabName = "AgentWithoutModel";
        private const string customPrefabName = "CustomAgentWithoutModel";

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

            // Prevent importing non-human-rigged model assets directly
            string selectedAssetPath = AssetDatabase.GetAssetPath(selectedObject);
            if (string.IsNullOrEmpty(selectedAssetPath))
            {
                Object sourceAsset = PrefabUtility.GetCorrespondingObjectFromSource(selectedObject);
                if (sourceAsset != null)
                {
                    selectedAssetPath = AssetDatabase.GetAssetPath(sourceAsset);
                }
            }

            if (!string.IsNullOrEmpty(selectedAssetPath))
            {
                AssetImporter importer = AssetImporter.GetAtPath(selectedAssetPath);
                if (importer is ModelImporter modelImporter &&
                    modelImporter.animationType != ModelImporterAnimationType.Human)
                {
                    string message =
                        "Selected model asset uses a generic or non-human rig. Please set the rig animation type to Humanoid before importing it as an agent model.\n " +
                        "1. In the project window select your asset at \"" + selectedAssetPath + "\"\n " +
                        "2. In the Inspector window select Rig > Animation Type > Humanoid." + "\n " +
                        "3. Click apply." + "\n " +
                        "4. Then try again.";
                    int answer = EditorUtility.DisplayDialogComplex("Wrong rigging", message, "Select the asset for me",
                        "Cancel", "Continue anyway");
                    switch (answer)
                    {
                        case 0: // ok
                            // Select the asset at selectedAssetPath in the project window
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(selectedAssetPath);
                            return;
                        case 1: // cancel
                            return;
                        case 2: // alternative, continue anyway
                            break;
                    }
                }
            }

            // Find the prefab by name within the project
            string[] prefabGuids = AssetDatabase.FindAssets(prefabName + " t:Prefab");
            string[] customPrefabGuids = AssetDatabase.FindAssets(customPrefabName + " t:Prefab");

            // If a custom Prefab is defined by the user, use that one, otherwise use the default one
            if (customPrefabGuids.Length == 0)
            {
                Debug.Log(
                    "Using default preset prefab. Optionally a prefab named \"CustomAgentWithoutModel\" based on the \"com.i5.virtualagents/Runtime/Prefabs/AgentWithoutModel.prefab\" can be used to modify all following imports. ");
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
            if (instantiatedPrefab == null)
            {
                Debug.LogError("Failed to instantiate prefab.");
                return;
            }

            Undo.RegisterCreatedObjectUndo(instantiatedPrefab, "Create Agent from Humanoid Model");

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

                // Move rig to local 0 X/Z to remove export offset; preserve Y.
                // Anything with a Mesh Renderer is unaffected by this action
                Vector3 localPosition = child.localPosition;
                child.localPosition = new Vector3(0f, localPosition.y, 0f);
            }

            // If imported model already has an animator component with an avatar, use that one, otherwise use the default avatar
            if (selectedObject.TryGetComponent<Animator>(out var animator))
            {
                if (animator.avatar != null)
                {
                    Debug.Log("Using Animator avatar provided by the model. ");
                    // Set the avatar to null to avoid problems when the new avatar is the same as the old one
                    Animator instantiatedAnimator = instantiatedPrefab.GetComponent<Animator>();
                    if (instantiatedAnimator == null)
                    {
                        Debug.LogError("Instantiated prefab has no Animator component.");
                        DestroyImmediate(copyOfSelectedObject);
                        return;
                    }

                    instantiatedAnimator.avatar = null;
                    // Making sure that the avatar was set to null and that the previous line was not optimized away by the compiler
                    if (instantiatedAnimator.avatar != null)
                    {
                        Debug.LogError("Avatar was not successfully set to null. This causes problems, when the new avatar is the same and results in Unity not updating the HumanBones correctly.");
                    }

                    instantiatedAnimator.avatar = animator.avatar;
                }
                // Otherwise the default avatar that is specified in the prefab will be used
            }
            else
            {
                Debug.LogWarning(
                    "No Animator component found. Using default animator. This might be a problem. It is recommended to add an Animator Component with a fitting avatar, usually this happens automatically when importing the model as an FBX file into Unity.");
            }

            // Destroy the cloned object
            DestroyImmediate(copyOfSelectedObject);
            // Set the position of the instantiated prefab next to the position of the original selected object
            instantiatedPrefab.transform.SetPositionAndRotation(
                selectedObject.transform.position + new Vector3(0, 0, selectedObject.transform.localScale.y),
                selectedObject.transform.rotation);
            instantiatedPrefab.transform.localScale = selectedObject.transform.localScale;

            Selection.activeGameObject = instantiatedPrefab;
            Selection.activeGameObject.name = "AgentBasedOn" + selectedObject.name;

            CheckAnimatorAvatar();
        }

        private static void FailSetup(GameObject obj, string errorMessage)
        {
            obj.name = "Failed" + obj.name;
            Debug.LogError(errorMessage);
            string message = "An error occurred while setting up the agent:\n" + errorMessage + "\n" +
                             "The incomplete agent is named " + obj.name + ".\n" +
                             "Would you like to delete it or keep it and resolve the issue manually?";
            bool answer = EditorUtility.DisplayDialog("Import Setup Error", message, "Delete Failed Agent (recommended)",
                "Keep Failed Agent");
            if (answer)
            {
                DestroyImmediate(obj);
            }
            
            
        }

        private static void CheckAnimatorAvatar()
        {
            GameObject newlyCreatedAgent = Selection.activeGameObject; // This should be the newly created agent

            if (!newlyCreatedAgent.TryGetComponent<Agent>(out _))
            {
                FailSetup(newlyCreatedAgent,
                    "No agent component found. Please check that the CustomAgentWithoutModel prefab has an Agent component.");
                return;
            }

            if (!newlyCreatedAgent.TryGetComponent<Animator>(out var animator))
            {
                FailSetup(newlyCreatedAgent,
                    "No Animator component found. Please check that the CustomAgentWithoutModel prefab has an Animator component.");
                return;
            }

            Debug.Log("Checking if the Avatar " + (animator.avatar ? animator.avatar.name : "null") +
                      " fits the provided model: ");

            bool isAvatarValid = animator.avatar != null &&
                                 animator.avatar.isHuman &&
                                 animator.GetBoneTransform(HumanBodyBones.Hips) != null &&
                                 animator.GetBoneTransform(HumanBodyBones.RightLowerArm) != null;

            if (!isAvatarValid)
            {
                Debug.LogWarning("Avatar is invalid or missing bones. Attempting automatic fix for hierarchy...");

                AvatarCreationResult creationResult = TryCreateAutomaticAvatar(newlyCreatedAgent, animator);
                if (creationResult == AvatarCreationResult.Success)
                {
                    Debug.Log("Successfully created and assigned a new Avatar for the hierarchy.");
                    FixAnimationRiggingBasedOnAnimatorAvatar(newlyCreatedAgent, animator);
                }
                else if (creationResult == AvatarCreationResult.PendingManual)
                {
                    Debug.Log("Automatic fix could not resolve all bones. Manual mapping window opened.");
                    // Branch continues in ManualAvatarMappingWindow.Show
                }
                else
                {
                    FailSetup(newlyCreatedAgent,
                        "Automatic fix failed. The model hierarchy does not match the known structure, or the Avatar is fundamentally incompatible.");
                }
            }
            else
            {
                Debug.Log(
                    "The Avatar fits the provided model. Mesh Sockets and Animation Rigging will be set up according to that.");
                FixAnimationRiggingBasedOnAnimatorAvatar(newlyCreatedAgent, animator);
            }
        }

        /// <summary>
        /// Helper to build, validate, and assign the Avatar
        /// </summary>
        private static bool TryBuildAvatar(GameObject rootObject, Animator animator, HumanDescription description,
            bool applyRiggingNow)
        {
            EnforceTPose(rootObject.transform, description.human); // AvatarBuilder.BuildHumanAvatar requires T-Pose

            // Rebuild skeleton to match the enforced T-pose transforms
            List<SkeletonBone> skeletonBones = new List<SkeletonBone>();
            foreach (Transform t in rootObject.GetComponentsInChildren<Transform>())
            {
                SkeletonBone bone = new SkeletonBone
                {
                    name = t.name,
                    position = t.localPosition,
                    rotation = t.localRotation,
                    scale = t.localScale
                };
                skeletonBones.Add(bone);
            }

            description.skeleton = skeletonBones.ToArray();

            Avatar newAvatar = AvatarBuilder.BuildHumanAvatar(rootObject, description);
            if (newAvatar != null && newAvatar.isValid)
            {
                newAvatar.name = "AutoGeneratedAvatar";
                animator.avatar = newAvatar;

                if (applyRiggingNow)
                {
                    FixAnimationRiggingBasedOnAnimatorAvatar(rootObject, animator);
                }

                return true;
            }

            return false;
        }

        private static void FixAnimationRiggingBasedOnAnimatorAvatar(GameObject selectedObject, Animator animator)
        {
            // Validation Checks
            bool isAvatarValid = animator.avatar != null &&
                                 animator.avatar.isHuman &&
                                 animator.GetBoneTransform(HumanBodyBones.Hips) != null &&
                                 animator.GetBoneTransform(HumanBodyBones.RightLowerArm) != null;

            if (!isAvatarValid)
            {
                Debug.LogError("Cannot set up Animation Rigging: Avatar is invalid or missing critical bones.");
                return;
            }

            const float socketSnapThreshold = 0.1f;

            // Helper Method
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
                    WeightedTransformArray sources = new WeightedTransformArray { newSource };

                    constraint.data.sourceObjects = sources;
                }
                else
                {
                    Debug.LogError($"MultiParentConstraint missing on {socket.name}");
                }
            }

            void AlignSocketToBone(string socketPath, HumanBodyBones boneType, HumanBodyBones? boneType2 = null)
            {
                Transform bone = animator.GetBoneTransform(boneType);
                Transform bone2 = boneType2.HasValue ? animator.GetBoneTransform(boneType2.Value) : null;
                if (bone == null)
                {
                    return;
                }

                Transform socket = selectedObject.transform.Find(socketPath);
                if (socket == null)
                {
                    return;
                }
                // If we have a second bone, then the position should be in between both bones
                Vector3 position = bone2 == null ? bone.position : (bone.position + bone2.position) / 2f;

                if (Vector3.Distance(socket.position, position) > socketSnapThreshold)
                {
                    socket.position = position;
                    EditorUtility.SetDirty(socket);
                }
            }

            // Apply constraints using the helper
            Debug.Log("Starting Animation Rigging Setup...");

            AddSourceToConstraint("AnimationRigging/MeshSockets/RightHandSocket", HumanBodyBones.RightHand);
            AddSourceToConstraint("AnimationRigging/MeshSockets/LeftHandSocket", HumanBodyBones.LeftHand);

            AddSourceToConstraint("AnimationRigging/MeshSockets/RightLowerArmSocket", HumanBodyBones.RightLowerArm);
            AddSourceToConstraint("AnimationRigging/MeshSockets/LeftLowerArmSocket", HumanBodyBones.LeftLowerArm);

            AddSourceToConstraint("AnimationRigging/MeshSockets/RightUpperArmSocket", HumanBodyBones.RightUpperArm);
            AddSourceToConstraint("AnimationRigging/MeshSockets/LeftUpperArmSocket", HumanBodyBones.LeftUpperArm);

            AlignSocketToBone("AnimationRigging/MeshSockets/RightHandSocket", HumanBodyBones.RightHand,HumanBodyBones.RightMiddleProximal);
            AlignSocketToBone("AnimationRigging/MeshSockets/LeftHandSocket", HumanBodyBones.LeftHand, HumanBodyBones.LeftMiddleProximal);
            AlignSocketToBone("AnimationRigging/MeshSockets/RightLowerArmSocket", HumanBodyBones.RightLowerArm);
            AlignSocketToBone("AnimationRigging/MeshSockets/LeftLowerArmSocket", HumanBodyBones.LeftLowerArm);
            AlignSocketToBone("AnimationRigging/MeshSockets/RightUpperArmSocket", HumanBodyBones.RightUpperArm);
            AlignSocketToBone("AnimationRigging/MeshSockets/LeftUpperArmSocket", HumanBodyBones.LeftUpperArm);

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
                Debug.LogWarning(
                    "MeshSockets component not found on the used prefab. Skipping Two Bone IK setup. Please check that the CustomAgentWithoutModel prefab has a MeshSockets component with the correct socket structure.");
            }
            else
            {
                // Left Arm
                meshSockets.TwoBoneIKConstraintLeftArm.data.root =
                    animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                meshSockets.TwoBoneIKConstraintLeftArm.data.mid =
                    animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                meshSockets.TwoBoneIKConstraintLeftArm.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                
                // Right Arm
                meshSockets.TwoBoneIKConstraintRightArm.data.root =
                    animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                meshSockets.TwoBoneIKConstraintRightArm.data.mid =
                    animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                meshSockets.TwoBoneIKConstraintRightArm.data.tip = animator.GetBoneTransform(HumanBodyBones.RightHand);
                
                // Left Leg
                meshSockets.TwoBoneIKConstraintLeftLeg.data.root =
                    animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                meshSockets.TwoBoneIKConstraintLeftLeg.data.mid =
                    animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                meshSockets.TwoBoneIKConstraintLeftLeg.data.tip = animator.GetBoneTransform(HumanBodyBones.LeftFoot);

                // Right Leg
                meshSockets.TwoBoneIKConstraintRightLeg.data.root =
                    animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                meshSockets.TwoBoneIKConstraintRightLeg.data.mid =
                    animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                meshSockets.TwoBoneIKConstraintRightLeg.data.tip = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                
                // Hips
                meshSockets.MultiParentConstraintHip.data.constrainedObject =
                    animator.GetBoneTransform(HumanBodyBones.Hips);
                
                // Spine
                meshSockets.MultiAimConstraintSpine.data.constrainedObject =
                    animator.GetBoneTransform(HumanBodyBones.Spine);
            }

            EditorUtility.SetDirty(selectedObject);
            EditorUtility.SetDirty(meshSockets);

            Debug.Log("Successfully set up Animation Rigging based on the Animator's Avatar.");

        }

        /// <summary>
        /// Attempts to build a Humanoid Avatar for some naming convention and falls back to manuel selection otherwise
        /// </summary>
        private static AvatarCreationResult TryCreateAutomaticAvatar(GameObject rootObject, Animator animator)
        {
            // Create dictionary for quick bone lookup by name, ignoring case and common separators, and taking the first match in case of duplicates
            Dictionary<string, Transform> boneLookup = rootObject.GetComponentsInChildren<Transform>()
                .GroupBy(t => NormalizeBoneName(t.name))
                .ToDictionary(g => g.Key, g => g.First());


            HumanDescription description = new HumanDescription();

            // 1. Setup Skeleton (List of all bones in the hierarchy)
            List<SkeletonBone> skeletonBones = new List<SkeletonBone>();
            foreach (Transform t in rootObject.GetComponentsInChildren<Transform>())
            {
                SkeletonBone bone = new SkeletonBone
                {
                    name = t.name,
                    position = t.localPosition,
                    rotation = t.localRotation,
                    scale = t.localScale
                };
                skeletonBones.Add(bone);
            }

            description.skeleton = skeletonBones.ToArray();

            // 2. Setup Human (Mapping from Bone Name -> Unity HumanBone)
            List<UnityEngine.HumanBone> humanBones = new List<UnityEngine.HumanBone>();

            string NormalizeBoneName(string boneName)
            {
                if (string.IsNullOrEmpty(boneName))
                {
                    return string.Empty;
                }

                return new string(boneName
                    .Where(c => c != ' ' && c != '-' && c != '_')
                    .Select(char.ToLowerInvariant)
                    .ToArray());
            }
            
            // Helper to add mapping if a candidate bone exists in hierarchy
            void AddMap(string humanName, params string[] boneNames)
            {
                foreach (string boneName in boneNames)
                {
                    string normalizedName = NormalizeBoneName(boneName);
                    if (boneLookup.TryGetValue(normalizedName, out Transform actualBone))
                    {
                        humanBones.Add(new UnityEngine.HumanBone
                        {
                            boneName = actualBone.name, // Keep the original name in tact!
                            humanName = humanName,
                            limit = new HumanLimit { useDefaultValues = true }
                        });
                        return;
                    }
                }
            }

            // Body
            AddMap("Hips", "Hips", "pelvis");
            AddMap("Spine", "Spine", "spine_01");
            AddMap("Chest", "Spine1", "spine_02");
            AddMap("UpperChest", "Spine2", "spine_03");
            AddMap("Neck", "Neck", "neck_01");
            AddMap("Head", "Head", "head");

            // Legs
            AddMap("LeftUpperLeg", "LeftUpLeg", "thigh_l");
            AddMap("LeftLowerLeg", "LeftLeg", "calf_l");
            AddMap("LeftFoot", "LeftFoot", "foot_l");
            AddMap("LeftToes", "LeftToeBase", "ball_l");

            AddMap("RightUpperLeg", "RightUpLeg", "thigh_r");
            AddMap("RightLowerLeg", "RightLeg", "calf_r");
            AddMap("RightFoot", "RightFoot", "foot_r");
            AddMap("RightToes", "RightToeBase", "ball_r");

            // Arms
            AddMap("LeftShoulder", "LeftShoulder", "clavicle_l");
            AddMap("LeftUpperArm", "LeftArm", "upperarm_l");
            AddMap("LeftLowerArm", "LeftForeArm", "lowerarm_l");
            AddMap("LeftHand", "LeftHand", "hand_l");

            AddMap("RightShoulder", "RightShoulder", "clavicle_r");
            AddMap("RightUpperArm", "RightArm", "upperarm_r");
            AddMap("RightLowerArm", "RightForeArm", "lowerarm_r");
            AddMap("RightHand", "RightHand", "hand_r");

            // Fingers
            // Left Hand
            AddMap("Left Thumb Proximal", "LeftHandThumb1", "thumb_01_l");
            AddMap("Left Thumb Intermediate", "LeftHandThumb2", "thumb_02_l");
            AddMap("Left Thumb Distal", "LeftHandThumb3", "thumb_03_l");

            AddMap("Left Index Proximal", "LeftHandIndex1", "index_01_l");
            AddMap("Left Index Intermediate", "LeftHandIndex2", "index_02_l");
            AddMap("Left Index Distal", "LeftHandIndex3", "index_03_l");

            AddMap("Left Middle Proximal", "LeftHandMiddle1", "middle_01_l");
            AddMap("Left Middle Intermediate", "LeftHandMiddle2", "middle_02_l");
            AddMap("Left Middle Distal", "LeftHandMiddle3", "middle_03_l");

            AddMap("Left Ring Proximal", "LeftHandRing1", "ring_01_l");
            AddMap("Left Ring Intermediate", "LeftHandRing2", "ring_02_l");
            AddMap("Left Ring Distal", "LeftHandRing3", "ring_03_l");

            AddMap("Left Little Proximal", "LeftHandPinky1", "pinky_01_l");
            AddMap("Left Little Intermediate", "LeftHandPinky2", "pinky_02_l");
            AddMap("Left Little Distal", "LeftHandPinky3", "pinky_03_l");

            // Right Hand
            AddMap("Right Thumb Proximal", "RightHandThumb1", "thumb_01_r");
            AddMap("Right Thumb Intermediate", "RightHandThumb2", "thumb_02_r");
            AddMap("Right Thumb Distal", "RightHandThumb3", "thumb_03_r");

            AddMap("Right Index Proximal", "RightHandIndex1", "index_01_r");
            AddMap("Right Index Intermediate", "RightHandIndex2", "index_02_r");
            AddMap("Right Index Distal", "RightHandIndex3", "index_03_r");

            AddMap("Right Middle Proximal", "RightHandMiddle1", "middle_01_r");
            AddMap("Right Middle Intermediate", "RightHandMiddle2", "middle_02_r");
            AddMap("Right Middle Distal", "RightHandMiddle3", "middle_03_r");

            AddMap("Right Ring Proximal", "RightHandRing1", "ring_01_r");
            AddMap("Right Ring Intermediate", "RightHandRing2", "ring_02_r");
            AddMap("Right Ring Distal", "RightHandRing3", "ring_03_r");

            AddMap("Right Little Proximal", "RightHandPinky1", "pinky_01_r");
            AddMap("Right Little Intermediate", "RightHandPinky2", "pinky_02_r");
            AddMap("Right Little Distal", "RightHandPinky3", "pinky_03_r");


            description.human = humanBones.ToArray();

            // Build the Avatar
            if (TryBuildAvatar(rootObject, animator, description, applyRiggingNow: false))
                return AvatarCreationResult.Success;


            List<HumanBodyBones> missingBones =
                GetMissingHumanBones(humanBones, ManualAvatarMappingWindow.SupportedHumanBones);
            if (missingBones.Count > 0)
            {
                ManualAvatarMappingWindow.Show(rootObject, animator, description, humanBones, missingBones);
                return AvatarCreationResult.PendingManual;
            }

            return AvatarCreationResult.Failed;
        }

        /// <summary>
        /// Forces the character into a T-Pose by aligning arms horizontally and legs vertically. This is required for AvatarBuilder.BuildHumanAvatar
        /// </summary>
        private static void EnforceTPose(Transform root, UnityEngine.HumanBone[] mappedBones)
        {
            Dictionary<string, Transform> boneLookup = root.GetComponentsInChildren<Transform>()
                .GroupBy(t => t.name)
                .ToDictionary(g => g.Key, g => g.First());

            // Helper to get a Transform from the human mapping
            Transform GetMappedBone(string humanName)
            {
                var mappedBone = mappedBones.FirstOrDefault(b => b.humanName == humanName);
                if (mappedBone.boneName != null && boneLookup.TryGetValue(mappedBone.boneName, out Transform bone))
                {
                    return bone;
                }

                return null;
            }

            // Local helper to align a bone segment while preserving its local Up vector to prevent twisting
            void AlignBone(Transform boneToRotate, Transform childBone, Vector3 targetDirection)
            {
                if (boneToRotate == null || childBone == null) return;

                Vector3 currentDirection = (childBone.position - boneToRotate.position).normalized;

                // Calculate the shortest rotation to the target
                Quaternion shortestRotation = Quaternion.FromToRotation(currentDirection, targetDirection);

                // Apply it
                boneToRotate.rotation = shortestRotation * boneToRotate.rotation;
            }

            // --- ARMS (Target: Horizontal) ---
            Vector3 leftArmTarget = -root.right;
            Vector3 rightArmTarget = root.right;

            // Straighten Left Arm
            AlignBone(GetMappedBone("LeftUpperArm"), GetMappedBone("LeftLowerArm"), leftArmTarget);
            AlignBone(GetMappedBone("LeftLowerArm"), GetMappedBone("LeftHand"), leftArmTarget);

            // Straighten Right Arm
            AlignBone(GetMappedBone("RightUpperArm"), GetMappedBone("RightLowerArm"), rightArmTarget);
            AlignBone(GetMappedBone("RightLowerArm"), GetMappedBone("RightHand"), rightArmTarget);

            // --- LEGS (Target: Vertical) ---
            Vector3 legTarget = -root.up;

            // Straighten Left Leg
            AlignBone(GetMappedBone("LeftUpperLeg"), GetMappedBone("LeftLowerLeg"), legTarget);
            AlignBone(GetMappedBone("LeftLowerLeg"), GetMappedBone("LeftFoot"), legTarget);

            // Straighten Right Leg
            AlignBone(GetMappedBone("RightUpperLeg"), GetMappedBone("RightLowerLeg"), legTarget);
            AlignBone(GetMappedBone("RightLowerLeg"), GetMappedBone("RightFoot"), legTarget);

            // --- FEET (Target: Forward) ---
            Vector3 footTarget = root.forward;

            AlignBone(GetMappedBone("LeftFoot"), GetMappedBone("LeftToes"), footTarget);
            AlignBone(GetMappedBone("RightFoot"), GetMappedBone("RightToes"), footTarget);
        }

        private static List<HumanBodyBones> GetMissingHumanBones(List<UnityEngine.HumanBone> mappedBones,
            IReadOnlyList<HumanBodyBones> supportedBones)
        {
            HashSet<string> mappedHumanNames = new HashSet<string>(mappedBones.Select(b => b.humanName));
            List<HumanBodyBones> missing = new List<HumanBodyBones>();

            foreach (HumanBodyBones bone in supportedBones)
            {
                string humanName = GetHumanName(bone);
                if (!mappedHumanNames.Contains(humanName))
                {
                    missing.Add(bone);
                }
            }

            return missing;
        }

        private static string GetHumanName(HumanBodyBones bone)
        {
            int index = (int)bone;
            if (index < 0 || index >= HumanTrait.BoneName.Length)
            {
                return bone.ToString();
            }

            return HumanTrait.BoneName[index];
        }



        private sealed class ManualAvatarMappingWindow : EditorWindow
        {
            public static readonly HumanBodyBones[] SupportedHumanBones =
            {
                HumanBodyBones.Hips,
                HumanBodyBones.Spine,
                HumanBodyBones.Chest,
                HumanBodyBones.UpperChest,
                HumanBodyBones.Neck,
                HumanBodyBones.Head,
                HumanBodyBones.LeftShoulder,
                HumanBodyBones.LeftUpperArm,
                HumanBodyBones.LeftLowerArm,
                HumanBodyBones.LeftHand,
                HumanBodyBones.RightShoulder,
                HumanBodyBones.RightUpperArm,
                HumanBodyBones.RightLowerArm,
                HumanBodyBones.RightHand,
                HumanBodyBones.LeftUpperLeg,
                HumanBodyBones.LeftLowerLeg,
                HumanBodyBones.LeftFoot,
                HumanBodyBones.LeftToes,
                HumanBodyBones.RightUpperLeg,
                HumanBodyBones.RightLowerLeg,
                HumanBodyBones.RightFoot,
                HumanBodyBones.RightToes,
                HumanBodyBones.LeftThumbProximal,
                HumanBodyBones.LeftThumbIntermediate,
                HumanBodyBones.LeftThumbDistal,
                HumanBodyBones.LeftIndexProximal,
                HumanBodyBones.LeftIndexIntermediate,
                HumanBodyBones.LeftIndexDistal,
                HumanBodyBones.LeftMiddleProximal,
                HumanBodyBones.LeftMiddleIntermediate,
                HumanBodyBones.LeftMiddleDistal,
                HumanBodyBones.LeftRingProximal,
                HumanBodyBones.LeftRingIntermediate,
                HumanBodyBones.LeftRingDistal,
                HumanBodyBones.LeftLittleProximal,
                HumanBodyBones.LeftLittleIntermediate,
                HumanBodyBones.LeftLittleDistal,
                HumanBodyBones.RightThumbProximal,
                HumanBodyBones.RightThumbIntermediate,
                HumanBodyBones.RightThumbDistal,
                HumanBodyBones.RightIndexProximal,
                HumanBodyBones.RightIndexIntermediate,
                HumanBodyBones.RightIndexDistal,
                HumanBodyBones.RightMiddleProximal,
                HumanBodyBones.RightMiddleIntermediate,
                HumanBodyBones.RightMiddleDistal,
                HumanBodyBones.RightRingProximal,
                HumanBodyBones.RightRingIntermediate,
                HumanBodyBones.RightRingDistal,
                HumanBodyBones.RightLittleProximal,
                HumanBodyBones.RightLittleIntermediate,
                HumanBodyBones.RightLittleDistal
            };

            private static readonly HumanBodyBones[] RequiredHumanBones =
            {
                HumanBodyBones.Hips,
                HumanBodyBones.Spine,
                HumanBodyBones.Chest,
                HumanBodyBones.Neck,
                HumanBodyBones.Head,
                HumanBodyBones.LeftUpperArm,
                HumanBodyBones.LeftLowerArm,
                HumanBodyBones.LeftHand,
                HumanBodyBones.RightUpperArm,
                HumanBodyBones.RightLowerArm,
                HumanBodyBones.RightHand,
                HumanBodyBones.LeftUpperLeg,
                HumanBodyBones.LeftLowerLeg,
                HumanBodyBones.LeftFoot,
                HumanBodyBones.RightUpperLeg,
                HumanBodyBones.RightLowerLeg,
                HumanBodyBones.RightFoot
            };

            private sealed class MissingBoneEntry
            {
                public HumanBodyBones Bone;
                public Transform AssignedTransform;
            }


            private GameObject rootObject;
            private Animator animator;
            private HumanDescription description;
            private List<UnityEngine.HumanBone> autoHumanBones;
            private List<MissingBoneEntry> missingBoneEntries;
            private Vector2 scrollPosition;
            private bool setupCompleted;
            private string validationWarning = "";

            public static void Show(
                GameObject rootObject,
                Animator animator,
                HumanDescription description,
                List<UnityEngine.HumanBone> autoHumanBones,
                List<HumanBodyBones> missingBones)
            {
                ManualAvatarMappingWindow window = CreateInstance<ManualAvatarMappingWindow>();
                window.titleContent = new GUIContent("Manual Avatar Mapping");
                window.rootObject = rootObject;
                window.animator = animator;
                window.description = description;
                window.autoHumanBones = new List<UnityEngine.HumanBone>(autoHumanBones);
                window.missingBoneEntries = missingBones
                    .Select(bone => new MissingBoneEntry { Bone = bone })
                    .ToList();
                window.minSize = new Vector2(420f, 320f);
                window.ShowUtility();
            }

            private void OnGUI()
            {
                EditorGUILayout.LabelField("Map missing bones", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Drag the matching Transform from the hierarchy of the model " + rootObject.name + " for each bone that could not be mapped automatically.",
                    MessageType.Info);
                
                if (!string.IsNullOrEmpty(validationWarning))
                {
                    EditorGUILayout.HelpBox(validationWarning, MessageType.Warning);
                }
                EditorGUILayout.Space();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                foreach (MissingBoneEntry entry in missingBoneEntries)
                {
                    string label = GetHumanName(entry.Bone);
                    // 1. Capture the user's input into a temporary variable
                    Transform pickedTransform = (Transform)EditorGUILayout.ObjectField(label, entry.AssignedTransform, typeof(Transform), true);

                    // 2. Validate if the user changed the field
                    if (pickedTransform != entry.AssignedTransform)
                    {
                        if (pickedTransform == null)
                        {
                            // Always allow clearing the field
                            entry.AssignedTransform = null; 
                        }
                        else if (!pickedTransform.IsChildOf(rootObject.transform))
                        {
                            // Must belong to this avatar's hierarchy
                            validationWarning = $"Cannot assign '{pickedTransform.name}'. It must be a child of '{rootObject.name}'.";
                        }
                        else
                        {
                            // Valid! Accept the assignment.
                            entry.AssignedTransform = pickedTransform;
                        }
                    }
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Build Avatar"))
                    {
                        TryBuildAvatar();
                    }

                    if (GUILayout.Button("Cancel"))
                    {
                        Close();
                    }
                }
            }

            private void OnDestroy()
            {
                if (!setupCompleted && rootObject != null)
                {
                    FailSetup(rootObject, "Manual Avatar Mapping was canceled. Aborting...");
                }
            }

            private void TryBuildAvatar()
            {
                List<UnityEngine.HumanBone> mergedBones = new List<UnityEngine.HumanBone>(autoHumanBones);

                foreach (MissingBoneEntry entry in missingBoneEntries)
                {
                    if (entry.AssignedTransform == null)
                    {
                        continue;
                    }

                    mergedBones.Add(new UnityEngine.HumanBone
                    {
                        boneName = entry.AssignedTransform.name,
                        humanName = GetHumanName(entry.Bone),
                        limit = new HumanLimit { useDefaultValues = true }
                    });
                }

                List<HumanBodyBones> stillMissing = GetMissingHumanBones(mergedBones, RequiredHumanBones);
                if (stillMissing.Count > 0)
                {
                    string missingList = string.Join(", ", stillMissing.Select(GetHumanName));
                    EditorUtility.DisplayDialog("Missing Required Bones", "Please assign: " + missingList, "OK");
                    return;
                }

                description.human = mergedBones.ToArray();

                if (AgentImportMenu.TryBuildAvatar(rootObject, animator, description, applyRiggingNow: true))
                {
                    Debug.Log("Successfully created and assigned a new Avatar for the hierarchy.");
                    setupCompleted = true;
                    Close();
                }
                else
                {
                    EditorUtility.DisplayDialog("Avatar Build Failed",
                        "Unity could not build a valid avatar. Please verify the bone assignments.", "OK");
                }
            }
        }
    }
}