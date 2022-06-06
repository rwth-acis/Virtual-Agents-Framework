using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace i5.VirtualAgents.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
    }
}
