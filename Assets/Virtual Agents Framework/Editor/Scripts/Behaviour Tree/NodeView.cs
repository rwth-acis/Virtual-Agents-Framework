using i5.VirtualAgents.BehaviourTrees;
using i5.VirtualAgents.BehaviourTrees.Visual;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
            Undo.RecordObject(node, "Behavior Tree (Node Moved)");
            node.Position.x = newPos.xMin;
            node.Position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
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
                if (node.GetCopyOfSerializedInterface() is SelectorNode)
                {
                    AddToClassList("selectorNode");
                }
                else if (node.GetCopyOfSerializedInterface() is SequencerNode)
                {
                    AddToClassList("sequencerNode");
                }
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
            //Every node, except the root, has one input
            if (!(node.GetCopyOfSerializedInterface() is IRootNode))
            {
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                input.portName = "";
                //input.style.flexDirection = FlexDirection.Column; // Styling that can't be done in UXML because the port is added at runtime
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
                //output.style.flexDirection = FlexDirection.ColumnReverse; // Styling that can't be done in UXML because the port is added at runtime
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

        public void UpdateState()
        {
            RemoveFromClassList("success");
            RemoveFromClassList("failure");
            RemoveFromClassList("running");
            RemoveFromClassList("waiting");

            if (Application.isPlaying)
            {
                if (this.node.CorrespondingTask != null)
                {
                    switch (this.node.CorrespondingTask.State)
                    {
                        case TaskState.Success:
                            AddToClassList("success");
                            break;
                        case TaskState.Failure:
                            AddToClassList("failure");
                            break;
                        case TaskState.Running:
                            AddToClassList("running");
                            break;
                        case TaskState.Waiting:
                            AddToClassList("waiting");
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
