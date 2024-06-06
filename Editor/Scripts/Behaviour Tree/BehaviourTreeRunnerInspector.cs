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
using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor
{
    [CustomEditor(typeof(BehaviourTreeRunner))]
    public class BehaviourTreeRunnerInspector : UnityEditor.Editor
    {
        // Root node of the inspector
        private VisualElement inspector;

        // The property fields used to display the propertys of the currently selected node
        private List<PropertyField> propertyFieldsForCurrentNode = new List<PropertyField>();

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of the inspector UI
            inspector = new VisualElement();

            // Load and clone a visual tree from UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/BehaviourTreeRunnerInspector.uxml");
            visualTree.CloneTree(inspector);

            // Setup the behaviour tree view
            BehaviourTreeView behaviourTreeView = inspector.Query<BehaviourTreeView>();
            behaviourTreeView.SetupManipulators(true);
            behaviourTreeView.OnNodeSelect = OnNodeSelectionChanged; // Register callback on node select in order to display the corrsponding property fields for the node
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

            // Return the finished inspector UI
            return inspector;
        }



        private void OnNodeSelectionChanged(NodeView view)
        {
            // Search if overwrite data exisits
            BehaviourTreeRunner runner = target as BehaviourTreeRunner;
            var nodesData = runner.nodesOverwriteData.data;
            SerializedProperty nodeOverwriteData = null;
            for (int i = 0; i < nodesData.Count && nodeOverwriteData == null; i++)
            {
                SerializationEntry<SerializationDataContainer> entry = nodesData[i];
                if (entry.Key == view.node.Guid)
                {
                    nodeOverwriteData = serializedObject.FindProperty("nodesOverwriteData.data").GetArrayElementAtIndex(i).FindPropertyRelative("Value");
                }
            }

            if (nodeOverwriteData == null)
            {
                // No data found => create it!
                nodeOverwriteData = CreateNodeOverwriteData(view);
            }
            // Create Property fields for the overwrite data

            // Clear old property fields
            foreach (var propertyField in propertyFieldsForCurrentNode)
            {
                propertyField.RemoveFromHierarchy();
            }
            propertyFieldsForCurrentNode.Clear();

            // Create new ones for current node data
            VisualNode targetNode = view.node;

            // Needed in order to expose the data in the original order
            int vectorCounter = 0;
            int floatCounter = 0;
            int stringCounter = 0;
            int intCounter = 0;
            int gameobjectCounter = 0;
            int boolCounter = 0;
            int listFloatCounter = 0;
            int treesCounter = 0;

            SerializableType[] serializationOrder = new SerializableType[targetNode.Data.serializationOrder.Count];

            targetNode.Data.serializationOrder.CopyTo(serializationOrder);

            int index = 1;
            foreach (var type in serializationOrder)
            {

                switch (type)
                {
                    case SerializableType.VECTOR3:
                        CreatePropertyField("serializedVectors", ref vectorCounter, SerializableType.VECTOR3, targetNode, nodeOverwriteData, index);
                        break;
                    case SerializableType.FLOAT:
                        CreatePropertyField("serializedFloats", ref floatCounter, SerializableType.FLOAT, targetNode, nodeOverwriteData, index);
                        break;
                    case SerializableType.STRING:
                        CreatePropertyField("serializedStrings", ref stringCounter, SerializableType.STRING, targetNode, nodeOverwriteData, index);
                        break;
                    case SerializableType.INT:
                        CreatePropertyField("serializedInts", ref intCounter, SerializableType.INT, targetNode, nodeOverwriteData, index);
                        break;
                    case SerializableType.GAMEOBJECT:
                        CreatePropertyField("serializedGameobjects", ref gameobjectCounter, SerializableType.GAMEOBJECT, targetNode, nodeOverwriteData, index);
                        break;
                    case SerializableType.BOOL:
                        CreatePropertyField("serializedBools", ref boolCounter, SerializableType.BOOL, targetNode, nodeOverwriteData, index);
                        break;
                    case SerializableType.LIST_FLOAT:
                        CreatePropertyField("serializedListFloats", ref listFloatCounter, SerializableType.LIST_FLOAT, targetNode, nodeOverwriteData, index);
                        break;
                    case SerializableType.TREE:
                        CreatePropertyField("serializedTrees", ref treesCounter, SerializableType.TREE, targetNode, nodeOverwriteData, index);
                        break;
                    default:
                        throw new NotImplementedException(type + " has no property field handler");
                }
                index++;
            }

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

            // Copys all data from the seralizationData List origin into the serialized seralizationData array destination
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
                        value.vector3Value = (Vector3)(data.Value as Vector3?); // This is neccesarry, since direct cast can't be used because T is not constrained to inherit from Vector3 and the as operator
                                                                                // can only be used on nullable types. Therfore the conversion to the nullable type Vector3? which is then casted to the actual Vector3 type
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
                    else
                    {
                        throw new NotImplementedException(typeof(T) + " has no copy handler");
                    }

                }
            }

            // Copy the serialization data from the node to the newly created nodesData
            CopySerializedData(view.node.Data.serializedVectors.data, "serializedVectors.data");
            CopySerializedData(view.node.Data.serializedFloats.data, "serializedFloats.data");
            CopySerializedData(view.node.Data.serializedStrings.data, "serializedStrings.data");
            CopySerializedData(view.node.Data.serializedInts.data, "serializedInts.data");
            CopySerializedData(view.node.Data.serializedGameobjects.data, "serializedGameobjects.data");
            CopySerializedData(view.node.Data.serializedBools.data, "serializedBools.data");
            CopySerializedData(view.node.Data.serializedListFloats.data, "serializedListFloats.data");
            CopySerializedData(view.node.Data.serializedTrees.data, "serializedTrees.data");

            return nodeOverwriteData;
        }

        // Creates a property field of the provided type for the serialized data saved in the array with the name propertyName
        private void CreatePropertyField(string propertyName, ref int counter, SerializableType type, VisualNode targetNode, SerializedProperty nodeOverwriteData, int index)
        {
            // Retrieve the serialized array
            SerializedProperty baseProperty = nodeOverwriteData.FindPropertyRelative(propertyName + ".data").GetArrayElementAtIndex(counter).FindPropertyRelative("Value");
            // Create the property field for the element with index counter
            PropertyField field = new PropertyField(baseProperty);
            field.label = targetNode.Data.GetKeyByIndex(counter, type);
            field.BindProperty(serializedObject);

            // Insert the field at the beginning of the inspector's children list
            inspector.Insert(index, field); // Use the Insert method with index 0 to add the field above existing tree

            propertyFieldsForCurrentNode.Add(field);
            counter++;
        }
    }
}
