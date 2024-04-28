using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using i5.VirtualAgents.BehaviourTrees.Visual;
using i5.VirtualAgents.BehaviourTrees;
using UnityEngine.UIElements;

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
        public NodeView(VisualNode node) : base("Assets/Virtual Agents Framework/Editor/UI Builder/Behaviour Tree/NodeView.uxml")
        {
            
            this.node = node;
            title = node.name;
            viewDataKey = node.Guid;

            // Restore the position saved in the serialized data
            style.left = node.Position.x;
            style.top = node.Position.y;

            // Create the ports
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

        /// <summary>
        /// Set node position
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.Position.x = newPos.xMin;
            node.Position.y = newPos.yMin;
        }


        private void SetupClasses()
        {
            // Set the class of the node
            if (node.GetCopyOfSerializedInterface() is IRootNode)
            {
                AddToClassList("rootNode");
            }
            else if (node.GetCopyOfSerializedInterface() is ICompositeNode)
            {
                AddToClassList("compositeNode");
            }
            else if (node.GetCopyOfSerializedInterface() is IDecoratorNode)
            {
                AddToClassList("decoratorNode");
            }
            else
            {
                AddToClassList("actionNode");
            }
        }
        // Create the ports for input edges
        private void CreateInputPorts()
        {
            //Every node, exept the root, has one input
            if (!(node.GetCopyOfSerializedInterface() is IRootNode))
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column; // Styling that can't be done in UXML because the port is added at runtime
                inputContainer.Add(input);
            }
        }

        // Create the ports for output edges
        private void CreateOutputPorts()
        {
            //Action Nodes/Tasks are leaves => no output

            //Composite Nodes can have multiple children/outputs
            if (node.GetCopyOfSerializedInterface() is ICompositeNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            //Decorator Nodes have one childe/output
            if (node.GetCopyOfSerializedInterface() is IDecoratorNode || node.GetCopyOfSerializedInterface() is IRootNode)
            {
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            }

            if (output != null)
            {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse; // Styling that can't be done in UXML because the port is added at runtime
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
