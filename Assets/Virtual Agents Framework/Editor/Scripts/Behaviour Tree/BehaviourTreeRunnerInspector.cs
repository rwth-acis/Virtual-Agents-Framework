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
                nodesData.InsertArrayElementAtIndex(size);
                var entry = nodesData.GetArrayElementAtIndex(size);
                entry.FindPropertyRelative("Key").stringValue = view.node.Guid;
                nodeOverwriteData = entry.FindPropertyRelative("Value");

                // Copy the serialization data from the node to the newly created nodesData
                int counter = 0;
                SerializedProperty serializedArray = nodeOverwriteData.FindPropertyRelative("serializedVectors.data");
                SerializedProperty serializedData;
                foreach (var data in view.node.Data.serializedVectors.data)
                {
                    serializedArray.InsertArrayElementAtIndex(counter);
                    serializedData = serializedArray.GetArrayElementAtIndex(counter);
                    serializedData.FindPropertyRelative("Key").stringValue = data.Key;
                    serializedData.FindPropertyRelative("Value").vector3Value = data.Value;
                    counter++;
                }

                counter = 0;
                serializedArray = nodeOverwriteData.FindPropertyRelative("serializedFloats.data");
                foreach (var data in view.node.Data.serializedFloats.data)
                {
                    serializedArray.InsertArrayElementAtIndex(counter);
                    serializedData = serializedArray.GetArrayElementAtIndex(counter);
                    serializedData.FindPropertyRelative("Key").stringValue = data.Key;
                    serializedData.FindPropertyRelative("Value").floatValue = data.Value;
                    counter++;
                }

                counter = 0;
                serializedArray = nodeOverwriteData.FindPropertyRelative("serializedStrings.data");
                foreach (var data in view.node.Data.serializedStrings.data)
                {
                    serializedArray.InsertArrayElementAtIndex(counter);
                    serializedData = serializedArray.GetArrayElementAtIndex(counter);
                    serializedData.FindPropertyRelative("Key").stringValue = data.Key;
                    serializedData.FindPropertyRelative("Value").stringValue = data.Value;
                    counter++;
                }

                counter = 0;
                serializedArray = nodeOverwriteData.FindPropertyRelative("serializedInts.data");
                foreach (var data in view.node.Data.serializedInts.data)
                {
                    serializedArray.InsertArrayElementAtIndex(counter);
                    serializedData = serializedArray.GetArrayElementAtIndex(counter);
                    serializedData.FindPropertyRelative("Key").stringValue = data.Key;
                    serializedData.FindPropertyRelative("Value").intValue = data.Value;
                    counter++;
                }

                

            }

            // Clear old property fields
            foreach (var propertyField in propertyFieldsForCurrendNode)
            {
                propertyField.RemoveFromHierarchy();
            }
            propertyFieldsForCurrendNode.Clear();

            // Create new ones for current 

            VisualNode targetNode = view.node;

            // Needed in order to expose the data in the original order
            int vectorCounter = 0;
            int floatCounter = 0;
            int stringCounter = 0;
            int intCounter = 0;

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
