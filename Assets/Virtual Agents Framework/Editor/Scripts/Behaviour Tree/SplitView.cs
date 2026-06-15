using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor
{
    /// <summary>
    /// Exposes TwoPaneSplitView to the UI builder via UxmlElement registration.
    /// </summary>
#if UNITY_2023_2_OR_NEWER
    [UxmlElement]
    public partial class SplitView : TwoPaneSplitView
    {
    }
#else
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : TwoPaneSplitView.UxmlFactory { }
    }
#endif
}
