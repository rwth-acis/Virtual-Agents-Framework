using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.BehaviourTrees.Visual;
using i5.VirtualAgents.BehaviourTrees;
using System;
using System.Linq;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Displays a behaviour tree in the behaviour tree editor and provides the means to manipulate it.
    /// </summary>
    public class BehaviourTreeView : GraphView
    {
        public Action<NodeView> OnNodeSelect;

        // Needed for the UI Builder
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        public BehaviorTreeAsset Tree;

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            //Adds the ability to zoom in on the graph, to drag and drop nodes around, to drag and drop an entire selection and to select nodes using a rectangle selction
            this.AddManipulator(new ContentZoomer()); //since AddManipulator is an extension method, it can only be called with a direct object reference (hence the "this.")
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/BehaviourTreeEditorStyleSheet.uss");
            styleSheets.Add(styleSheet);
        }

        /// <summary>
        /// Creates node views for all nodes in the tree and draws the necessary edges.
        /// </summary>
        /// <param name="tree"></param>
        public void PopulateView(BehaviorTreeAsset tree)
        {
            this.Tree = tree;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
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
            //Delete the removed nodes from the asset
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var elemToRemove in graphViewChange.elementsToRemove)
                {
                    NodeView nodeToRemove = elemToRemove as NodeView;
                    if (nodeToRemove != null)
                    {
                        Tree.DeleteNode(nodeToRemove.node);
                    }
                }
            }

            //Add new edges to the asset
            if(graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    ((NodeView)edge.output.node).node.Children.Add(((NodeView)edge.input.node).node);
                }
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
                        ISerializable task = constructor.Invoke(new object[0]) as ISerializable; //Can only use it as node if it is serializable
                        if (task != null)
                        {
                            Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
                            evt.menu.AppendAction(menuName + "/" + type.Name, (a) => CreateVisualNode(task, nodePosition));
                        }
                    }
                }
            }

            BuildContextMenuEntrysFromType<IAgentTask>("Tasks");
            BuildContextMenuEntrysFromType<ICompositeNode>("Composite Nodes");
            BuildContextMenuEntrysFromType<IDecoratorNode>("Decorator Nodes");
        }


        //Creates a NodeView for the given node
        private NodeView CreateNodeView(VisualNode node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelect = OnNodeSelect;
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
    }
}
