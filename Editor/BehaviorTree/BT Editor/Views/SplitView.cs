using UnityEngine.UIElements;

namespace BT.Editor
{
    // Only used to make it selectable inside UI BUilder.
    // this is an old solution and alternatives should be
    // explored
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }

        public SplitView()
        {
        }
    }
}

