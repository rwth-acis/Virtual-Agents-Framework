using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using i5.VirtualAgents.BehaviourTrees.Visual;
using i5.VirtualAgents.TaskSystem;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Exposes the data that was serialized via the ISerializable interface in the original serialization order
    /// </summary>
    [CustomEditor(typeof(VisualNode))]
    public class VisualNodeInspector : UnityEditor.Editor
    {

        // Creates a property field of the provided type for the serized data saved in the array with the name propertyName
        private void CreatePropertyField(string propertyName, ref int counter, SerializableType type, VisualNode targetNode)
        {
            // Retrive the serialized array
            SerializedProperty baseProperty = serializedObject.FindProperty(propertyName + ".data");
            // Create the property field for the element with index counter
            EditorGUILayout.PropertyField(baseProperty.GetArrayElementAtIndex(counter).FindPropertyRelative("value"), new GUIContent(targetNode.GetKeyByIndex(counter, type)));
            counter++;
        }
        public override void OnInspectorGUI()
        {
            VisualNode targetNode = target as VisualNode;

            // Needed in order to expose the data in the original order
            int vectorCounter = 0;
            int floatCounter = 0;
            int stringCounter = 0;
            int intCounter = 0;

            SerializableType[] serializationOrder = new SerializableType[targetNode.serializationOrder.Count];

            targetNode.serializationOrder.CopyTo(serializationOrder);

            foreach (var type in serializationOrder)
            {

                switch (type)
                {
                    case SerializableType.VECTOR3:
                        CreatePropertyField("serializedVectors", ref vectorCounter, SerializableType.VECTOR3, targetNode);
                        break;
                    case SerializableType.FLOAT:
                        CreatePropertyField("serializedFloats", ref floatCounter, SerializableType.FLOAT, targetNode);
                        break;
                    case SerializableType.STRING:
                        CreatePropertyField("serializedStrings", ref stringCounter, SerializableType.STRING, targetNode);
                        break;
                    case SerializableType.INT:
                        CreatePropertyField("serializedInts", ref intCounter, SerializableType.INT, targetNode);
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
