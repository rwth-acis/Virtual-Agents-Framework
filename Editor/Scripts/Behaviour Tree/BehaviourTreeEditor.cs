using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using i5.VirtualAgents;
using i5.VirtualAgents.Editor;
using i5.VirtualAgents.BehaviourTrees.Visual;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Provides a visual behaviour tree editor.
    /// </summary>
    public class BehaviourTreeEditor : EditorWindow
    {
        BehaviourTreeView treeView;
        InspectorView inspectorView;
        Label treeViewOccludeLabel;

        [MenuItem("i5 Toolkit/BehaviourTreeEditor")]
        public static void ShowWindow()
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        }

        public void CreateGUI()
        {
            //Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            //Import UXML. The UXML was generated by the Unity UIBuilder and contains which vsialual elements in which configuration comprise the BehaviourTreeEditor.
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/BehaviourTreeEditor.uxml");
            visualTree.CloneTree(root);

            //A stylesheet can be added to a VisualElement.
            //The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/BehaviourTreeEditorStyleSheet.uss");
            root.styleSheets.Add(styleSheet);

            //Fetch and initialise objects from window
            treeView = root.Query<BehaviourTreeView>();
            treeView.SetEnabled(false); //Disable, until a tree is selected
            inspectorView = root.Query<InspectorView>();
            treeView.OnNodeSelect = OnNodeSelectionChanged;
            treeViewOccludeLabel = root.Query<Label>("treeViewOccludeLabel");

            //Check if a tree is already selected
            LoadSelectedTree();

            //Setup the save button
            Button saveButton = root.Query<Button>("Save");
            saveButton.clicked += SaveTree;
        }

        //Changes the currently edited tree to the one selected in the unity project tab
        private void OnSelectionChange()
        {
            LoadSelectedTree();
        }

        private void LoadSelectedTree()
        {
            BehaviorTreeAsset tree = Selection.activeObject as BehaviorTreeAsset;
            if (tree != null)
            {
                if (treeViewOccludeLabel != null)
                {
                    treeViewOccludeLabel.RemoveFromHierarchy();
                }
                treeView.SetEnabled(true);
                treeView.PopulateView(tree);
            }
        }

        //Displays the inspector for the currently selected node
        private void OnNodeSelectionChanged(NodeView view)
        {
            inspectorView.UpdateSelection(view);
        }

        //Saves all changes made to the currently selected behaviour tree to the disc
        private void SaveTree()
        {
            EditorUtility.SetDirty(treeView.tree);
            AssetDatabase.SaveAssets();
        }
    } 
}