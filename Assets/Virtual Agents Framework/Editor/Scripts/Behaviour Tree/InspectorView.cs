using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Provides an inspector view for a node view
    /// </summary>
#if UNITY_2023_2_OR_NEWER
    [UxmlElement]
    public partial class InspectorView : VisualElement
    {
#else
    public class InspectorView : VisualElement
    {
        // Unity 2022 legacy factory so UI Builder can see it
        public new class UxmlFactory : UxmlFactory<InspectorView> { }
#endif

        private UnityEditor.Editor editor;

        internal void UpdateSelection(NodeView view)
        {
            Clear();
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
            }
            editor = UnityEditor.Editor.CreateEditor(view.node);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
