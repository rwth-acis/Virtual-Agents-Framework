using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.BehaviourTrees;
using i5.VirtualAgents.BehaviourTrees.Visual;
using i5.VirtualAgents.Editor.BehaviourTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor
{
    [CustomEditor(typeof(BehaviourTreeRunner))]
    public class BehaviourTreeRunnerInspector : UnityEditor.Editor
    {
        // Root node of the inspector
        private VisualElement inspector;

        // The property fields used to display the properties of the currently selected node
        private List<PropertyField> propertyFieldsForCurrentNode = new List<PropertyField>();

        private NodeView currentlySelectedNode = null;

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of the inspector UI
            inspector = new VisualElement();

            // Load and clone a visual tree from UXML
            VisualTreeAsset visualTree = AssetManager.Load<VisualTreeAsset>("BehaviourTreeRunnerInspector.uxml");
            visualTree.CloneTree(inspector);

            // Setup the Behaviour Tree view
            BehaviourTreeView behaviourTreeView = inspector.Query<BehaviourTreeView>();
            behaviourTreeView.SetupManipulators(true);
            behaviourTreeView.OnNodeSelect = OnNodeSelectionChanged; // Register callback on node select in order to display the corresponding property fields for the node
            BehaviourTreeAsset tree = (target as BehaviourTreeRunner).Tree;

            void SetupNewTree(BehaviourTreeAsset tree)
            {
                if (tree != null)
                {
                    behaviourTreeView.Tree = tree;
                    behaviourTreeView.PopulateView(tree);
                }
            }

            SetupNewTree(tree);


            // Setup tree when a new one is selected
            PropertyField treePropertyField = inspector.Query<PropertyField>("tree");
            treePropertyField.RegisterValueChangeCallback((x) => SetupNewTree(x.changedProperty.objectReferenceValue as BehaviourTreeAsset));
            
            // Reset overwrite data on button press
            UnityEngine.UIElements.Button resetButton = inspector.Query<UnityEngine.UIElements.Button>("reset");
            resetButton.clicked += () => {
                if(currentlySelectedNode != null)
                {
                    var property = SearchValidOverwriteData(currentlySelectedNode,true);
                    CreatePropertyFields(property,currentlySelectedNode);
                }
            };
            

            // Return the finished inspector UI
            return inspector;
        }


        private void CreatePropertyFields(SerializedProperty serializedNodeOverwriteData, NodeView view)
        {
            // Clear old property fields
            foreach (var propertyField in propertyFieldsForCurrentNode)
            {
                propertyField.RemoveFromHierarchy();
            }
            propertyFieldsForCurrentNode.Clear();

            // Just a wrapper to pass targetNode and serializedNodeOverwriteData to CreatePropertyField, while having a valid signature for MapOverData
            int wrapper(SerializableType type, int index)
            {
                return CreatePropertyField(type,index,view.node,serializedNodeOverwriteData);
            }

            view.node.Data.MapOverData(wrapper);
        }

        private SerializedProperty SearchValidOverwriteData(NodeView view, bool forceReset)
        {
            BehaviourTreeRunner runner = target as BehaviourTreeRunner;
            var nodesData = runner.nodesOverwriteData.data;
            SerializedProperty serializedNodeOverwriteData = null;
            int entryIndex = nodesData.FindIndex((SerializationEntry<SerializationDataContainer> o) => o.Key == view.node.Guid);
            if(entryIndex >= 0)
            {
                SerializedProperty serializedArray = serializedObject.FindProperty("nodesOverwriteData.data");
                serializedNodeOverwriteData = serializedArray.GetArrayElementAtIndex(entryIndex).FindPropertyRelative("Value");
                // Check integrity
                if(view.node.CheckIntegrity(nodesData[entryIndex].Value) || forceReset)
                {
                    serializedArray.DeleteArrayElementAtIndex(entryIndex);
                    serializedObject.ApplyModifiedProperties();
                    return CreateNodeOverwriteData(view);
                }
                return serializedNodeOverwriteData;
            }
            return CreateNodeOverwriteData(view);
        }

        private void OnNodeSelectionChanged(NodeView view)
        {
            currentlySelectedNode = view;
            SerializedProperty property = SearchValidOverwriteData(view, false);
            CreatePropertyFields(property,view);
            serializedObject.ApplyModifiedProperties();
        }


        private SerializedProperty CreateNodeOverwriteData(NodeView view)
        {
            SerializedProperty nodesData = serializedObject.FindProperty("nodesOverwriteData.data");
            int size = nodesData.arraySize;
            nodesData.InsertArrayElementAtIndex(size); // Insert a new entry at the end
            var entry = nodesData.GetArrayElementAtIndex(size);
            entry.FindPropertyRelative("Key").stringValue = view.node.Guid;
            SerializedProperty nodeOverwriteData = entry.FindPropertyRelative("Value");

            // Copies all data from the serializationData List origin into the serialized serializationData array destination
            void CopySerializedData<T>(List<SerializationEntry<T>> origin, string destinationPath)
            {
                SerializedProperty destination = nodeOverwriteData.FindPropertyRelative(destinationPath);
                // Needs to be cleared first, since it contains the copied values from the previous entry, due to InsertArrayElementAtIndex not working as described in the documentation
                destination.ClearArray();
                for (int i = 0; i < origin.Count; i++)
                {
                    SerializationEntry<T> data = origin[i];
                    destination.InsertArrayElementAtIndex(i); //Make space for the new data entry in the serialized array
                    SerializedProperty arrayElement = destination.GetArrayElementAtIndex(i);

                    // Copy key and data
                    arrayElement.FindPropertyRelative("Key").stringValue = data.Key;
                    SerializedProperty value = arrayElement.FindPropertyRelative("Value");
                    if (typeof(T) == typeof(Vector3))
                    {
                        value.vector3Value = (Vector3)(data.Value as Vector3?); // This is necessary, since direct cast can't be used because T is not constrained to inherit from Vector3 and the as operator
                                                                                // can only be used on nullable types. Therefore the conversion to the nullable type Vector3? which is then casted to the actual Vector3 type
                    }
                    else if (typeof(T) == typeof(float))
                    {
                        value.floatValue = (float)(data.Value as float?);
                    }
                    else if (typeof(T) == typeof(string))
                    {
                        value.stringValue = data.Value as string;
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        value.intValue = (int)(data.Value as int?);
                    }
                    else if (typeof(T) == typeof(GameObject))
                    {
                        value.objectReferenceValue = data.Value as GameObject;
                    }
                    else if (typeof(T) == typeof(Transform))
                    {
                        value.objectReferenceValue = data.Value as Transform;
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        value.boolValue = (bool)(data.Value as bool?);
                    }
                    else if (typeof(T) == typeof(List<float>))
                    {
                        List<float> floatList = data.Value as List<float>;
                        for (int j = 0; j < floatList.Count; j++)
                        {
                            value.InsertArrayElementAtIndex(j);
                            value.GetArrayElementAtIndex(j).floatValue = floatList[j];
                        }
                    }
                    else if (typeof(T) == typeof(BehaviourTreeAsset))
                    {
                        value.objectReferenceValue = data.Value as BehaviourTreeAsset;
                    }
                    else if (typeof(T) == typeof(Quaternion))
                    {
                        // Cast to nullable Quaternion first, just like you did with Vector3
                        value.quaternionValue = (Quaternion)(data.Value as Quaternion?); 
                    }
                    else if (typeof(T) == typeof(AudioClip))
                    {
                        value.objectReferenceValue = data.Value as AudioClip;
                    }
                    else if (typeof(T) == typeof(AudioSource))
                    {
                        value.objectReferenceValue = data.Value as AudioSource;
                    }
                    else
                    {
                        throw new NotImplementedException(typeof(T) + " has no copy handler");
                    }

                }
            }

            // Copy the serialization data from the node to the newly created nodesData
            var d = view.node.Data;
            CopySerializedData(d.serializedVectors.data, "serializedVectors.data");
            CopySerializedData(d.serializedFloats.data, "serializedFloats.data");
            CopySerializedData(d.serializedStrings.data, "serializedStrings.data");
            CopySerializedData(d.serializedInts.data, "serializedInts.data");
            CopySerializedData(d.serializedGameobjects.data, "serializedGameobjects.data");
            CopySerializedData(d.serializedTransforms.data, "serializedTransforms.data");
            CopySerializedData(d.serializedBools.data, "serializedBools.data");
            CopySerializedData(d.serializedListFloats.data, "serializedListFloats.data");
            CopySerializedData(d.serializedTrees.data, "serializedTrees.data");
            CopySerializedData(d.serializedQuaternions.data, "serializedQuaternions.data");
            CopySerializedData(d.serializedAudioClips.data, "serializedAudioClips.data");
            CopySerializedData(d.serializedAudioSources.data, "serializedAudioSources.data");

            return nodeOverwriteData;
        }

        // Creates a property field of the provided type for the serialized data saved in the array with the name propertyName
        private int CreatePropertyField(SerializableType type, int counter, VisualNode targetNode, SerializedProperty nodeOverwriteData)
        {
            string propertyName = SerializationDataContainer.TypeToPath(type);
            // Retrieve the serialized array
            SerializedProperty propertyArray = nodeOverwriteData.FindPropertyRelative(propertyName + ".data");
            if(propertyArray != null && counter < propertyArray.arraySize)
            {
                SerializedProperty propertyValue = propertyArray.GetArrayElementAtIndex(counter).FindPropertyRelative("Value");
                // Create the property field for the element with index counter
                PropertyField field = new PropertyField(propertyValue);
                field.label = targetNode.Data.GetKeyByIndex(counter, type);
                field.BindProperty(serializedObject);

                // Insert the field at the beginning of the inspector's children list
                inspector.Insert(inspector.childCount - 2, field); // Use the Insert method with index 0 to add the field above existing tree

                propertyFieldsForCurrentNode.Add(field);
            }
            else
            {
                Debug.LogWarning("Serialized property of type " + type + " not found. Check that this type is fully implemented.");
            }
            return 0;
        }
    }
}
