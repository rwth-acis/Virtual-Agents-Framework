using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor
{
    /// <summary>
    /// Exposes the two TwoPaneSplitView to the UI builder by implementing the necessary UxmlFactory.
    /// </summary>
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
    }
}
