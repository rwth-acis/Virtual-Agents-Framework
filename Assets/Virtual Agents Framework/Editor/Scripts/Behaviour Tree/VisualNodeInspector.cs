using UnityEngine;
using UnityEditor;
using i5.VirtualAgents.BehaviourTrees.Visual;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Exposes the data that was serialized via the ISerializable interface in the original serialization order
    /// </summary>
    [CustomEditor(typeof(VisualNode))]
    public class VisualNodeInspector : UnityEditor.Editor
    {

        // Creates a property field of the provided type for the serialized data saved in the array with the name propertyName
        private int CreatePropertyField(SerializableType type, int counter)
        {
            VisualNode targetNode = target as VisualNode;
            // Retrieve the serialized array
            string propertyPath = SerializationDataContainer.TypeToPath(type);
            SerializedProperty baseProperty = serializedObject.FindProperty("Data." + propertyPath + ".data");
            // Create the property field for the element with index counter
            EditorGUILayout.PropertyField(baseProperty.GetArrayElementAtIndex(counter).FindPropertyRelative("Value"), new GUIContent(targetNode.Data.GetKeyByIndex(counter, type)));
            return 0;
        }
        public override void OnInspectorGUI()
        {
            VisualNode targetNode = target as VisualNode;
            if(!targetNode.CheckIntegrity())
            {
                targetNode.ReSerialize();
            }
            targetNode.Data.MapOverData(CreatePropertyField);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
