using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Provides an inspector view for a node view
    /// </summary>
    [UxmlElement]
    public partial class InspectorView : VisualElement
    {
        private UnityEditor.Editor editor;

        internal void UpdateSelection(NodeView view)
        {
            Clear();
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
            }
            editor = UnityEditor.Editor.CreateEditor(view.node);
            IMGUIContainer container = new IMGUIContainer(() => {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
