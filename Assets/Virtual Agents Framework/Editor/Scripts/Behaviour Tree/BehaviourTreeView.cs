using i5.VirtualAgents.AgentTasks;
using i5.VirtualAgents.BehaviourTrees;
using i5.VirtualAgents.BehaviourTrees.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Displays a behaviour tree in the behaviour tree editor and provides the means to manipulate it.
    /// </summary>
    public class BehaviourTreeView : GraphView
    {
        public Action<NodeView> OnNodeSelect;

        public Agent CurrentlySelectedAgent { get; set; }


        // Needed for the UI Builder
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        public BehaviourTreeAsset Tree;

        private bool readOnly = false;
        

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/BehaviourTreeEditorStyleSheet.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            if (this.Tree != null)
            {
                PopulateView(this.Tree);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Adds the ability to zoom in on the graph, to drag and drop nodes around, to drag and drop an entire selection and to select nodes using a rectangle selction
        /// </summary>
        /// <param name="readOnly"></param> If yes, the view forbits node creating, delting, connecting, and moving. The individual data from nodes (e.g. the target from a MovementTask) can however still be altered.
        public void SetupManipulators(bool readOnly = false)
        {
            this.readOnly = readOnly;
            this.AddManipulator(new ContentZoomer()); //since AddManipulator is an extension method, it can only be called with a direct object reference (hence the "this.")
            this.AddManipulator(new ContentDragger());
            if (!readOnly)
            {
                this.AddManipulator(new SelectionDragger());
                this.AddManipulator(new RectangleSelector());
            }
        }

        /// <summary>
        /// Creates node views for all nodes in the tree and draws the necessary edges.
        /// </summary>
        /// <param name="tree"></param>
        public void PopulateView(BehaviourTreeAsset tree)
        {
            this.Tree = tree;
            graphViewChanged -= OnGraphViewChanged;

            //Convert graphElements to list, since in older Unity versions UQueryState does not implement IEnumerable
            List<GraphElement> graphElementList = new List<GraphElement>();
            graphElements.ToList(graphElementList);

            DeleteElements(graphElementList);
            graphViewChanged += OnGraphViewChanged;

            //Create node views
            foreach (var node in tree.Nodes)
            {
                CreateNodeView(node);
            }

            //Create edges
            foreach (var node in tree.Nodes)
            {
                NodeView parentView = FindNodeView(node);
                foreach (var child in node.Children)
                {
                    NodeView childView = FindNodeView(child);
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                }
            }

        }

        //Finds the node view corresponding to the given visual node
        private NodeView FindNodeView(VisualNode node)
        {
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (!readOnly)
            {
                //Delete the removed nodes from the asset
                if (graphViewChange.elementsToRemove != null)
                {
                    foreach (var elemToRemove in graphViewChange.elementsToRemove)
                    {
                        if (elemToRemove is NodeView nodeToRemove && nodeToRemove.node is not IRootNode)
                        {
                            Tree.DeleteNode(nodeToRemove.node);
                        }
                        if (elemToRemove is Edge edge)
                        {
                            //Remove the child, so that the edge is not added again
                            Tree.RemoveChild(((NodeView)edge.output.node).node, ((NodeView)edge.input.node).node);
                        }
                    }
                }

                //Add new edges to the asset
                if (graphViewChange.edgesToCreate != null)
                {
                    foreach (var edge in graphViewChange.edgesToCreate)
                    {
                        Tree.AddChild(((NodeView)edge.output.node).node, ((NodeView)edge.input.node).node);
                    }
                }
            }
            else
            {
                graphViewChange.elementsToRemove?.Clear();
                graphViewChange.edgesToCreate?.Clear();
            }

            return graphViewChange;
        }

        /// <summary>
        /// Builds a context menu with options for creating nodes. Every non abstract class that (1) implements IAgentTask, ICompositeNode or IDecoratorNode, (2) additionally implements ISerialiazble
        /// and (3) has an empty constructor will automatically get its own context menu entry and can be fully used as node in the behaviour tree.
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {

            void CreateVisualNode(ISerializable node, Vector2 position)
            {
                VisualNode visualNode = Tree.AddNode(node);
                visualNode.Position = position;
                CreateNodeView(visualNode);
            }

            void BuildContextMenuEntrysFromType<T>(string menuName)
            {
                var derivedTypes = TypeCache.GetTypesDerivedFrom<T>();
                foreach (var type in derivedTypes)
                {
                    //Get the empty constructor. If no empty constructor exists, a corresponding node can't be created by the context menu.
                    var constructor = type.GetConstructor(new Type[0]);

                    if (constructor != null && !type.IsAbstract) // Can only instatiate a task, if it has an empty constructor
                    {
                        //Can only use it as node if it is serializable
                        if (constructor.Invoke(new object[0]) is ISerializable task && task is not IRootNode)
                        {
                            Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
                            evt.menu.AppendAction(menuName + "/" + type.Name, (a) => CreateVisualNode(task, nodePosition));
                        }
                    }
                }
            }

            if (!readOnly)
            {
                BuildContextMenuEntrysFromType<IAgentTask>("Tasks");
                BuildContextMenuEntrysFromType<ICompositeNode>("Composite Nodes");
                BuildContextMenuEntrysFromType<IDecoratorNode>("Decorator Nodes");
            }
        }


        //Creates a NodeView for the given node
        private NodeView CreateNodeView(VisualNode node)
        {
            NodeView nodeView = new NodeView(node)
            {
                OnNodeSelect = OnNodeSelect
            };
            AddElement(nodeView);
            return nodeView;
        }

        /// <summary>
        /// Get all ports compatible with given port. In theory this exact function is already provided by the GraphView class, but it contains an implementation error.
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }

        public void UpdateNodeStates()
        {
            if(Tree == null)
            {
                return;
            }
            if(Tree.Nodes == null)
            {
                return;
            }
            foreach (var node in Tree.Nodes)
            {
                NodeView nodeView = FindNodeView(node);
                nodeView.UpdateState(CurrentlySelectedAgent);
                
            }
        }
    }
}
