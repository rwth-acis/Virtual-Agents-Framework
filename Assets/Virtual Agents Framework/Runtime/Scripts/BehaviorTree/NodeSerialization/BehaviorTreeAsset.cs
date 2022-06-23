using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Asset that can be used to create behaviour trees that are saved persistently. The tree is not executable, but an executable abstract copy can be retrived.
    /// </summary>
    [CreateAssetMenu(menuName = "i5 Toolkit/Behaviour Tree")]
    public class BehaviorTreeAsset : ScriptableObject
    {
        private GraphicalNode rootNode;
        public GraphicalNode RootNode 
        {
            get 
            {
                if (rootNode == null && nodes != null && nodes.Count > 0)
                {
                    rootNode = nodes[0];
                }
                return rootNode;
            }
            private set
            {
                rootNode = value;
            }
        }

        public List<GraphicalNode> nodes = new List<GraphicalNode>();

        /// <summary>
        /// Adds a new node based on an serializable task.
        /// </summary>
        /// <param name="baseTask"></param>
        /// <returns></returns>
        public GraphicalNode AddNode(ISerializable baseTask)
        {
            GraphicalNode node = CreateInstance<GraphicalNode>();
            node.name = baseTask.GetType().Name;
            node.guid = GUID.Generate().ToString();
            node.SetSerializedType(baseTask);
            nodes.Add(node);
            AssetDatabase.AddObjectToAsset(node,this);
            return node;
        }

        /// <summary>
        /// Deletes the given node from the tree.
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteNode(GraphicalNode nodeToDelete)
        {
            nodes.Remove(nodeToDelete);
            foreach (var node in nodes)
            {
                node.children.Remove(nodeToDelete);
            }
            AssetDatabase.RemoveObjectFromAsset(nodeToDelete);
        }

        /// <summary>
        /// Generates an abstract copy of the tree that is executable through the root nodes update function.
        /// </summary>
        /// <returns></returns>
        public ITask GetExecutableTree()
        {
            rootNode = nodes[0];
            ITask root = (ITask)rootNode.GetCopyOfSerializedInterface();
            ConnectAbstractTree(rootNode, root);
            return root;
        }

        //Generates recursivly the abstract childs for the given graphical node and connects them.
        private void ConnectAbstractTree(GraphicalNode node, ITask abstractNode)
        {
            foreach (var child in node.children)
            {
                ITask abstractChild = (ITask)child.GetCopyOfSerializedInterface();
                if (abstractNode is ICompositeNode)
                {
                    (abstractNode as ICompositeNode).children.Add(abstractChild);
                }
                ConnectAbstractTree(child, abstractChild);
            }
        }
    }
}