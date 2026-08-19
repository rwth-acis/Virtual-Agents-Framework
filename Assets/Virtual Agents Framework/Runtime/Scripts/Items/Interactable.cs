using UnityEngine;
using UnityEngine.Events;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Defines the IK mode used during agent interaction.
    /// </summary>
    public enum InteractionIKMode
    {
        None,   // No IK targeting, play animation only
        Reach,  // Reach/Touch using TwoBoneIKConstraint (for arms)
        LookAt  // Look/Point using AimAt/gaze
    }

    /// <summary>
    /// Represents an object in the scene that an agent can interact with.
    /// </summary>
    [AddComponentMenu("i5 Toolkit/Virtual Agents/Interactable")]
    public class Interactable : MonoBehaviour
    {
        [Tooltip("The default animation clip for this interaction.")]
        [SerializeField] private AnimationClip interactionClip;

        [Tooltip("The Animator layer to play the animation on.")]
        [SerializeField] private string layerName = "Right Arm";

        [Tooltip("The default duration of the interaction in seconds.")]
        [SerializeField] private float duration = 2.0f;

        [Tooltip("The default speed of the IK reach/touch transition.")]
        [SerializeField] private float animationSpeed = 2.0f;

        [Tooltip("The default IK mode for this interaction.")]
        [SerializeField] private InteractionIKMode ikMode = InteractionIKMode.Reach;

        [Space(3)]
        [Header("IK-Mode Reach Settings")]
        [Tooltip("The precise position/rotation that the agent's hand/head should target during interaction.")]
        [SerializeField] private Transform interactionTarget;
        [Tooltip("The maximum IK weight applied during the interaction (0 to 1).")]
        [Range(0f, 1f)]
        [SerializeField] private float maxWeight = 1.0f;
        [Tooltip("Optional humanoid bone on the agent to use as the IK tip/touch point during Reach (e.g., RightIndexDistal). If set to LastBone, uses touchBoneName, touchBone, or the default arm tip.")]
        [SerializeField] private HumanBodyBones touchHumanBone = HumanBodyBones.LastBone;

        [Tooltip("Optional bone name on the agent hierarchy to use as the IK tip/touch point.")]
        [SerializeField] private string touchBoneName;

        [Tooltip("Optional direct Transform reference to use as the touch bone.")]
        [SerializeField] private Transform touchBone;

        public Transform InteractionTarget => interactionTarget != null ? interactionTarget : transform;
        public AnimationClip InteractionClip => interactionClip;
        public string LayerName => layerName;
        public InteractionIKMode IkMode => ikMode;
        public float Duration => duration;
        public float AnimationSpeed => animationSpeed;
        public float MaxWeight => maxWeight;
        public HumanBodyBones TouchHumanBone => touchHumanBone;
        public string TouchBoneName => touchBoneName;
        public Transform TouchBone => touchBone;


        [Space(3)]
        [Header("Events")]
        /// <summary>
        /// Triggered when the agent starts interacting with this object.
        /// </summary>
        public UnityEvent OnInteractionStarted;

        /// <summary>
        /// Triggered when the agent reaches peak touch / contact with this object.
        /// </summary>
        public UnityEvent OnInteractionContact;

        /// <summary>
        /// Triggered when the agent finishes interacting with this object.
        /// </summary>
        public UnityEvent OnInteractionEnded;

        private void Start()
        {
            if (interactionTarget == null)
            {
                interactionTarget = transform;
            }
        }
    }
}
