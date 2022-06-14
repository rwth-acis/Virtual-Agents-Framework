using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using i5.VirtualAgents.TaskSystem.AgentTasks;

namespace i5.VirtualAgents
{
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

        public TaskState rootState { get; set; }

        public List<GraphicalNode> nodes = new List<GraphicalNode>();

        public GraphicalNode AddAndCreateGrapicalNode(ISerializable baseTask)
        {
            GraphicalNode node = CreateInstance<GraphicalNode>();
            node.name = baseTask.GetType().Name;
            node.guid = GUID.Generate().ToString();
            node.serializedTask = baseTask;
            nodes.Add(node);
            AssetDatabase.AddObjectToAsset(node,this);
            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(GraphicalNode node)
        {
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public ITask GetAbstractCopy()
        {
            rootNode = nodes[0];
            ITask root = (ITask)rootNode.GetCopyOfSerializedInterface();
            ConnectAbstractTree(rootNode, root);
            return root;
        }

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