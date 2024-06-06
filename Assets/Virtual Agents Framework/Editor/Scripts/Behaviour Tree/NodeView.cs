using i5.VirtualAgents.BehaviourTrees;
using i5.VirtualAgents.BehaviourTrees.Visual;
using System;
using System.Linq;
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
        private Label descriptionLabel;
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
            SetupDescription();
        }

        /// <summary>
        /// Set node position
        /// </summary>
        /// <param name="newPos"></param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (Node Moved)");
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
            if (node.GetCopyOfSerializedInterface() is not IRootNode)
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
            OnNodeSelect?.Invoke(this);
        }

        /// <summary>
        /// Adds a description to the node view based on the node type to be shown in the inspector
        /// The description can be overwritten by changing the description field of the task (inherited from BaseTask)
        /// </summary>
        public void SetupDescription()
        {
            descriptionLabel = this.Q<Label>("description");

            var ser = node.GetCopyOfSerializedInterface();

            // First check if the node has a specific custom description
            if (ser is BaseTask task && !string.IsNullOrEmpty(task.description))
            {
                Debug.Log("Task description: " + task.description);
                descriptionLabel.text = task.description;
            }
            else if (ser is SequencerNode)
            {
                descriptionLabel.text = "Executes its children from left to right, until one <b>fails</b>. (AND-Operation)";
            }
            else if (ser is SelectorNode)
            {
                descriptionLabel.text = "Executes its children from left to right, until one <b>succeeds</b>. (OR-Operation)";
            }
            else if (ser is AlwaysSucceedNode)
            {
                descriptionLabel.text = "Executes its child, and always successes when the child finishes.";
            }
            else if (ser is RootNode)
            {
                descriptionLabel.text = "Starts by executing its child.";
            }
            else if (ser is InverterNode)
            {
                descriptionLabel.text = "Inverts the result of its child.";
            }
            else if (ser is RepeatUntilSuccessNode)
            {
                descriptionLabel.text = "Repeats its child until it succeeds.";
            }
            else if (ser is RandomNode)
            {
                descriptionLabel.text = "Randomly chooses a child to execute.";
            }
            else if (ser is TimeOutNode)
            {
                descriptionLabel.text = "Automatically fails when the child doesn't finish in time.";
            }
            else
            {
                descriptionLabel.text = "Executes a specific task.";
            }
        }

        public void UpdateState(Agent currentlySelectedAgent)
        {
            RemoveFromClassList("success");
            RemoveFromClassList("failure");
            RemoveFromClassList("running");
            RemoveFromClassList("waiting");

            if (Application.isPlaying)
            {
                if (this.node.CorrespondingTask != null)
                {
                    ITask task;
                    if (currentlySelectedAgent == null)
                    {
                        // If no agent is selected, use the first agent in the dictionary
                        task = this.node.CorrespondingTask.Values.ToList().FirstOrDefault();
                    }
                    else {
                        this.node.CorrespondingTask.TryGetValue(currentlySelectedAgent, out task);
                    }
                    if (task == null)
                    {
                        return;
                    }
                    switch (task.State)
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
