using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using i5.VirtualAgents;
using i5.VirtualAgents.Editor;


/// <summary>
/// Provides a visual behaviour tree editor.
/// </summary>
public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView; 

    [MenuItem("i5 Toolkit/BehaviourTreeEditor")]
    public static void ShowWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Virtual Agents Framework/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Virtual Agents Framework/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Query<BehaviourTreeView>();
        inspectorView = root.Query<InspectorView>();
        treeView.OnNodeSelect = OnNodeSelectionChanged;

        //Setup the save button
        Button saveButton = root.Query<Button>("Save");
        saveButton.clicked += SaveTree;
    }

    //Changes the currently edited tree to the one selected in the unity project tab
    private void OnSelectionChange()
    {
        BehaviorTreeAsset tree = Selection.activeObject as BehaviorTreeAsset;
        if (tree)
        {
            treeView.PopulateView(tree);
        }
    }

    //Displays the inspector for the currently selected node
    void OnNodeSelectionChanged(NodeView view)
    {
        inspectorView.UpdateSelection(view);
    }

    void SaveTree()
    {
        EditorUtility.SetDirty(treeView.tree);
        AssetDatabase.SaveAssets();
    }
}