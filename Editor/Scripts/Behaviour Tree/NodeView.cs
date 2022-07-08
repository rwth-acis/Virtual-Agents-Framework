using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using i5.VirtualAgents.BehaviourTrees.Visual;
using i5.VirtualAgents.BehaviourTrees;

namespace i5.VirtualAgents.Editor.BehaviourTrees
{
    /// <summary>
    /// Displays a Behaviour Tree node in the visual editor
    /// </summary>
    public class NodeView : Node
    {
        public Action<NodeView> OnNodeSelect;
        public VisualNode node;
        public Port input;
        public Port output;
        public NodeView(VisualNode node)
        {
            this.node = node;
            title = node.name;
            viewDataKey = node.guid;

            // Restore the position saved in the serialized data
            style.left = node.position.x;
            style.top = node.position.y;

            // Create the ports
            CreateInputPorts();
            CreateOutputPorts();
        }

        /// <summary>
        /// Set node position
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
        }

        // Create the ports for input edges
        private void CreateInputPorts()
        {
            //Every node has one input
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            input.portName = "";
            inputContainer.Add(input);
        }

        // Create the ports for output edges
        private void CreateOutputPorts()
        {
            //Action Nodes/Tasks are leaves => no output

            //Composite Nodes can have multiple children/outputs
            if (node.GetCopyOfSerializedInterface() is ICompositeNode)
            {
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            if (output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }

        /// <summary>
        /// Delegate the OnSelect event to display this node in the node inspector
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelect != null)
            {
                OnNodeSelect.Invoke(this);
            }
        }
    }
}
