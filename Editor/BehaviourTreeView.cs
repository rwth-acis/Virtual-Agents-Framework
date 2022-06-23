using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using i5.VirtualAgents.TaskSystem;
using i5.VirtualAgents.TaskSystem.AgentTasks;
using System;
using System.Linq;

namespace i5.VirtualAgents.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public Action<NodeView> OnNodeSelect;
        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }
        public BehaviorTreeAsset tree;

        public BehaviourTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer()); //since AddManipulator is an extension method, it can only be called with a direct object reference (hence the "this.")
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Virtual Agents Framework/Editor/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);
        }

        
        internal void PopulateView(BehaviorTreeAsset tree)
        {
            this.tree = tree;
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            //Create nodes
            foreach (var node in tree.nodes)
            {
                CreateNodeView(node);
            }

            //Create edges
            foreach (var node in tree.nodes)
            {
                NodeView parentView = FindNodeView(node);
                foreach (var child in node.children)
                {
                    NodeView childView = FindNodeView(child);
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                }
            }

        }

        NodeView FindNodeView(GraphicalNode node)
        {
            return GetNodeByGuid(node.guid) as NodeView;
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
                        tree.DeleteNode(nodeToRemove.node);
                    }
                }
            }

            //Add new edges to the asset
            if(graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                    ((NodeView)edge.output.node).node.children.Add(((NodeView)edge.input.node).node);
                }
            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            void BuildContextMenuEntrysFromType<T>(string menuName) 
            {
                var derivedTypes = TypeCache.GetTypesDerivedFrom<T>();
                foreach (var type in derivedTypes)
                {
                    
                    var constructor = type.GetConstructor(new Type[0]);
                     
                    if (constructor != null) // Can only instatiate a task, if it has an empty constructor
                    {
                        ISerializable task = constructor.Invoke(new object[0]) as ISerializable; //Can only use it as node if it serializable
                        if (task != null)
                        {
                            evt.menu.AppendAction(menuName + "/" + type.Name, (a) => CreateGraphicalNode(task));
                        }
                    }
                }
            }

            BuildContextMenuEntrysFromType<IAgentTask>("Tasks");

            BuildContextMenuEntrysFromType<ICompositeNode>("Composite Nodes");
        }

        void CreateGraphicalNode(ISerializable node)
        {
            GraphicalNode graphicalNode = tree.AddNode(node);
            CreateNodeView(graphicalNode);
        }

        NodeView CreateNodeView(GraphicalNode node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelect = OnNodeSelect;
            AddElement(nodeView);
            return nodeView;
        }

        /// <summary>
        /// 
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
