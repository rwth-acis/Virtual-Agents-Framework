using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using i5.VirtualAgents.Editor.BehaviourTrees;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Provides an inspector view for a node view
    /// </summary>
    public class InspectorView : VisualElement
    {
        //Needed so it can be used in the UI builder
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        UnityEditor.Editor editor;

        internal void UpdateSelection(NodeView view)
        {
            Clear();
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
            }
            editor = UnityEditor.Editor.CreateEditor(view.node);
            IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            Add(container);
        }
    }
}
