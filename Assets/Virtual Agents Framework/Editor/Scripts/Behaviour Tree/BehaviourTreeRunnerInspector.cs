using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using i5.VirtualAgents.BehaviourTrees.Visual;
using i5.VirtualAgents.BehaviourTrees;
using UnityEngine.UIElements;
using i5.VirtualAgents.Editor.BehaviourTrees;
using UnityEditor.UIElements;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents.Editor
{
    [CustomEditor(typeof(BehaviorTreeRunner))]
    public class BehaviourTreeRunnerInspector : UnityEditor.Editor
    {
        VisualElement inspector;
        private List<PropertyField> propertyFieldsForCurrendNode = new List<PropertyField>();

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            inspector = new VisualElement();

            // Load and clone a visual tree from UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/BehaviourTreeRunnerInspector.uxml");

            visualTree.CloneTree(inspector);

            Button clear = inspector.Query<Button>("Clear");
            clear.clicked += () => { serializedObject.FindProperty("nodesOverwriteData.data").ClearArray(); serializedObject.ApplyModifiedProperties(); };

            BehaviourTreeView behaviourTreeView = inspector.Query<BehaviourTreeView>();
            behaviourTreeView.OnNodeSelect = OnNodeSelectionChanged;
            behaviourTreeView.ReadOnly = true;
            BehaviorTreeAsset tree = (target as BehaviorTreeRunner).behaviourTree; //serializedObject.FindProperty("behaviourTree").objectReferenceValue as BehaviorTreeAsset;
            if (tree != null)
            {
                behaviourTreeView.Tree = tree;
                behaviourTreeView.PopulateView(tree);
            }



            //Debug
            PropertyField field = new PropertyField(serializedObject.FindProperty("nodesOverwriteData"));
            field.label = "Debug";
            field.BindProperty(serializedObject);
            inspector.Add(field);


            // Return the finished inspector UI
            return inspector;
        }


        private void OnNodeSelectionChanged(NodeView view)
        {
            // Search if overwrite data exisits
            BehaviorTreeRunner runner = target as BehaviorTreeRunner;
            var nodesData = serializedObject.FindProperty("nodesOverwriteData.data");
            SerializedProperty nodeOverwriteData = null;
            for (int i = 0; i < nodesData.arraySize && nodeOverwriteData == null; i++)
            {
                SerializedProperty entry = nodesData.GetArrayElementAtIndex(i);
                if (entry.FindPropertyRelative("Key").stringValue == view.node.Guid)
                {
                    nodeOverwriteData = entry.FindPropertyRelative("Value");
                }
            }

            if (nodeOverwriteData == null)
            {
                // No data found => create it!
                int size = nodesData.arraySize;
                nodesData.InsertArrayElementAtIndex(size); // Insert a new entry at the end
                var entry = nodesData.GetArrayElementAtIndex(size);
                entry.FindPropertyRelative("Key").stringValue = view.node.Guid;
                nodeOverwriteData = entry.FindPropertyRelative("Value");

                // Copys all data from the seralizationData List origin into the serialized seralizationData array destination
                void CopySerializedData <T> (List<SerializationEntry<T>> origin, string destinationPath)
                {
                    SerializedProperty destination = nodeOverwriteData.FindPropertyRelative(destinationPath);
                    for (int i = 0;  i < origin.Count; i++)
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
                        if (typeof(T) == typeof(float))
                        {
                            value.floatValue = (float)(data.Value as float?);
                        }
                        if (typeof(T) == typeof(string))
                        {
                            value.stringValue = data.Value as string;
                        }
                        if (typeof(T) == typeof(int))
                        {
                            value.intValue = (int)(data.Value as int?);
                        }
                        if (typeof(T) == typeof(GameObject))
                        {
                            value.objectReferenceValue = data.Value as GameObject;
                        }
                    }
                }

                // Copy the serialization data from the node to the newly created nodesData
                CopySerializedData(view.node.Data.serializedVectors.data, "serializedVectors.data");
                CopySerializedData(view.node.Data.serializedFloats.data, "serializedFloats.data");
                CopySerializedData(view.node.Data.serializedStrings.data, "serializedStrings.data");
                CopySerializedData(view.node.Data.serializedInts.data, "serializedInts.data");
                CopySerializedData(view.node.Data.serializedGameobjects.data, "serializedGameobjects.data");
            }

            // Clear old property fields
            foreach (var propertyField in propertyFieldsForCurrendNode)
            {
                propertyField.RemoveFromHierarchy();
            }
            propertyFieldsForCurrendNode.Clear();

            // Create new ones for current node data
            VisualNode targetNode = view.node;

            // Needed in order to expose the data in the original order
            int vectorCounter = 0;
            int floatCounter = 0;
            int stringCounter = 0;
            int intCounter = 0;
            int gameobjectCounter = 0;

            SerializableType[] serializationOrder = new SerializableType[targetNode.Data.serializationOrder.Count];

            targetNode.Data.serializationOrder.CopyTo(serializationOrder);

            foreach (var type in serializationOrder)
            {

                switch (type)
                {
                    case SerializableType.VECTOR3:
                        CreatePropertyField("serializedVectors", ref vectorCounter, SerializableType.VECTOR3, targetNode, nodeOverwriteData);
                        break;
                    case SerializableType.FLOAT:
                        CreatePropertyField("serializedFloats", ref floatCounter, SerializableType.FLOAT, targetNode, nodeOverwriteData);
                        break;
                    case SerializableType.STRING:
                        CreatePropertyField("serializedStrings", ref stringCounter, SerializableType.STRING, targetNode, nodeOverwriteData);
                        break;
                    case SerializableType.INT:
                        CreatePropertyField("serializedInts", ref intCounter, SerializableType.INT, targetNode, nodeOverwriteData);
                        break;
                    case SerializableType.GAMEOBJECT:
                        CreatePropertyField("serializedGameobjects", ref gameobjectCounter, SerializableType.GAMEOBJECT, targetNode, nodeOverwriteData);
                        break;
                }
            }


            //PropertyField test = new PropertyField();
            //test.bindingPath = "excecutingAgent";
            //test.BindProperty(serializedObject);
            //inspector.Add(test);

            serializedObject.ApplyModifiedProperties();
        }

        // Creates a property field of the provided type for the serized data saved in the array with the name propertyName
        private void CreatePropertyField(string propertyName, ref int counter, SerializableType type, VisualNode targetNode, SerializedProperty nodeOverwriteData)
        {
            // Retrive the serialized array
            SerializedProperty baseProperty = nodeOverwriteData.FindPropertyRelative(propertyName + ".data").GetArrayElementAtIndex(counter).FindPropertyRelative("Value");
            // Create the property field for the element with index counter
            PropertyField field = new PropertyField(baseProperty);
            field.label = targetNode.Data.GetKeyByIndex(counter, type);
            field.BindProperty(serializedObject);
            inspector.Add(field);
            propertyFieldsForCurrendNode.Add(field);
            counter++;
        }

        public override void OnInspectorGUI()
        {
            //VisualNode root = (target as TreeWrapper).tree.Nodes[0];
            //if (GUILayout.Button("Update"))
            //{
            //    var nodes = serializedObject.FindProperty("Nodes");
            //    nodes.ClearArray();
            //    nodes.InsertArrayElementAtIndex(0);
            //    var strings = nodes.GetArrayElementAtIndex(0).FindPropertyRelative("serializedStrings.data");
            //    strings.InsertArrayElementAtIndex(0);
            //    var key = strings.GetArrayElementAtIndex(0).FindPropertyRelative("Key");
            //    //key.stringValue = root.serializedStrings.data[0].Key;
            //    var value = strings.GetArrayElementAtIndex(0).FindPropertyRelative("Value");
            //    //value.stringValue = root.serializedStrings.data[0].Value;
            //}

            //EditorGUILayout.PropertyField(serializedObject.FindProperty("tree"), new GUIContent("tree"));
            //var test = serializedObject.FindProperty("Nodes").GetArrayElementAtIndex(0);
            //if (test != null)
            //{
            //    var data = test.FindPropertyRelative("serializedStrings.data").GetArrayElementAtIndex(0);
            //    EditorGUILayout.PropertyField(data.FindPropertyRelative("Value"), new GUIContent(data.FindPropertyRelative("Key").stringValue));
            //}
            
            //serializedObject.ApplyModifiedProperties();
        }

    }
}
