using UnityEngine;

namespace i5.VirtualAgents
{
    /// <summary>
    /// Represents a chair item with seated and standing alignment points.
    /// </summary>
    public class Chair : Item
    {
        /// <summary>
        /// The horizontal distance between the character's feet while seated.
        /// </summary>
        [Header("Chair Properties")]
        [Space]
        [Tooltip("The horizontal distance between the character's feet while seated.")]
        public float distanceBetweenFeet = 0.16f;

        /// <summary>
        /// The position used to align the feet while standing.
        /// </summary>
        [field: Tooltip("The position used to align the feet while standing.")]
        [field: SerializeField]
        public Transform StandingFeetPosition { get; protected set; }

        /// <summary>
        /// The optional position used to align the feet while seated.
        /// Falls back to <see cref="StandingFeetPosition"/> when not assigned.
        /// </summary>
        [Tooltip("The optional position used to align the feet while seated. Falls back to StandingFeetPosition when not assigned.")]
        [SerializeField]
        private Transform seatedFeetPosition;

        /// <summary>
        /// Gets the feet rest position for this chair.
        /// Returns the seated feet position when available, otherwise the standing feet position.
        /// </summary>
        public Transform SeatedFeetPosition
        {
            get => seatedFeetPosition ? seatedFeetPosition : StandingFeetPosition;
            protected set => seatedFeetPosition = value;
        }

        /// <summary>
        /// The position used to align the character's hips while seated.
        /// </summary>
        [field: Tooltip("The position used to align the character's hips while seated.")]
        [field: SerializeField]
        public Transform SeatedHipPosition { get; protected set; }
        
        /// <summary>
        /// Indicates whether this chair has the required alignment points configured.
        /// </summary>
        public bool HasValidConfiguration => StandingFeetPosition != null && SeatedHipPosition != null;
    }
}
