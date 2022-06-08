using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using i5.VirtualAgents;
using i5.VirtualAgents.Editor;


public class BehaviourTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView; 

    [MenuItem("i5 Toolkit/BehaviourTreeEditor")]
    public static void ShowExample()
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
        //VisualElement labelFromUXML = visualTree.Instantiate();
        //root.Add(labelFromUXML);
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Virtual Agents Framework/Editor/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Query<BehaviourTreeView>();
        inspectorView = root.Query<InspectorView>();
        treeView.OnNodeSelect = OnNodeSelectionChanged;

        Button saveButton = root.Query<Button>("Save");
        saveButton.clicked += SaveTree;
    }

    private void OnSelectionChange()
    {
        BehaviorTreeAsset tree = Selection.activeObject as BehaviorTreeAsset;
        if (tree)
        {
            treeView.PopulateView(tree);
        }
    }

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