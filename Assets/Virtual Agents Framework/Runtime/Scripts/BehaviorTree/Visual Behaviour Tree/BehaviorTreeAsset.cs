using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using i5.VirtualAgents.ScheduleBasedExecution;
using i5.VirtualAgents.AgentTasks;

namespace i5.VirtualAgents.BehaviourTrees.Visual
{
    /// <summary>
    /// Asset that can be used to create behaviour trees that are saved persistently. The tree is not executable, but an executable abstract copy can be retrived.
    /// </summary>
    [CreateAssetMenu(menuName = "Virtual Agents Framework/Behaviour Tree")]
    public class BehaviorTreeAsset : ScriptableObject
    {
        private VisualNode rootNode;
        public VisualNode RootNode 
        {
            get 
            {
                if (rootNode == null && Nodes != null && Nodes.Count > 0)
                {
                    rootNode = Nodes[0];
                }
                return rootNode;
            }
            private set
            {
                rootNode = value;
            }
        }

        public List<VisualNode> Nodes = new List<VisualNode>();

        /// <summary>
        /// Adds a new node based on an serializable task
        /// </summary>
        /// <param name="baseTask"></param>
        /// <returns></returns>
        public VisualNode AddNode(ISerializable baseTask)
        {
            VisualNode node = CreateInstance<VisualNode>();
            node.name = baseTask.GetType().Name;
            node.Guid = GUID.Generate().ToString();
            node.SetSerializedType(baseTask);
            Nodes.Add(node);
            AssetDatabase.AddObjectToAsset(node,this);
            return node;
        }

        /// <summary>
        /// Deletes the given node from the tree
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteNode(VisualNode nodeToDelete)
        {
            Nodes.Remove(nodeToDelete);
            foreach (var node in Nodes)
            {
                node.Children.Remove(nodeToDelete);
            }
            AssetDatabase.RemoveObjectFromAsset(nodeToDelete);
        }

        /// <summary>
        /// Generates an abstract copy of the tree that is executable through the root nodes FullUpdate() function
        /// </summary>
        /// <returns></returns>
        public ITask GetExecutableTree(NodesOverwriteData nodesOverwriteData = null)
        {
            rootNode = Nodes[0];
            SerializationDataContainer rootNodeData = null;
            if (nodesOverwriteData != null && nodesOverwriteData.KeyExists(rootNode.Guid))
            {
                rootNodeData = nodesOverwriteData.Get(rootNode.Guid);
            }
            ITask root = (ITask)rootNode.GetCopyOfSerializedInterface(rootNodeData);
            ConnectAbstractTree(rootNode, root, nodesOverwriteData);
            return root;
        }

        // Recursively generates the abstract childs for the given graphical node and connects them
        private void ConnectAbstractTree(VisualNode node, ITask abstractNode, NodesOverwriteData nodesOverwriteData)
        {
            foreach (var child in node.Children)
            {
                SerializationDataContainer nodeData = null;
                if (nodesOverwriteData.KeyExists(child.Guid))
                {
                    nodeData = nodesOverwriteData.Get(child.Guid);
                }
                ITask abstractChild = (ITask)child.GetCopyOfSerializedInterface(nodeData);
                if (abstractNode is ICompositeNode)
                {
                    (abstractNode as ICompositeNode).Children.Add(abstractChild);
                }
                ConnectAbstractTree(child, abstractChild, nodesOverwriteData);
            }
        }
    }
}