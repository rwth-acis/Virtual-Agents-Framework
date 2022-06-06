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
                if (rootNode == null)
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

        public GraphicalNode AddAndCreateGrapicalNode(ISerializable baseTask)
        {
            GraphicalNode node = CreateInstance<GraphicalNode>();
            node.name = baseTask.GetType().Name;
            node.guid = GUID.Generate();
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
    }
}