using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace i5.VirtualAgents.Editor
{
    [CustomEditor(typeof(GraphicalNode))]
    public class VisualNodeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GraphicalNode targetNode = target as GraphicalNode;

            int vectorCounter = 0;
            int floatCounter = 0;
            int stringCounter = 0;
            int intCounter = 0;

            SerializableType[] serializationOrder = new SerializableType[targetNode.serializationOrder.Count];

            targetNode.serializationOrder.CopyTo(serializationOrder);

            foreach (var type in serializationOrder)
            {
                void CreatePropertyField(string propertyName, ref int counter, SerializableType type)
                {
                    SerializedProperty baseProperty = serializedObject.FindProperty(propertyName + ".data"); ;
                    EditorGUILayout.PropertyField(baseProperty.GetArrayElementAtIndex(counter).FindPropertyRelative("value"), new GUIContent(targetNode.GetKeyByIndex(counter, type)));
                    counter++;
                }
                switch (type)
                {
                    case SerializableType.VECTOR3:
                        CreatePropertyField("serializedVectors", ref vectorCounter, SerializableType.VECTOR3);
                        break;
                    case SerializableType.FLOAT:
                        CreatePropertyField("serializedFloats", ref floatCounter, SerializableType.FLOAT);
                        break;
                    case SerializableType.STRING:
                        CreatePropertyField("serializedStrings", ref stringCounter, SerializableType.STRING);
                        break;
                    case SerializableType.INT:
                        CreatePropertyField("serializedInts", ref intCounter, SerializableType.INT);
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
