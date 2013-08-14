using System;

namespace Xpand.Persistent.Base.Logic {
    [Flags]
    public enum FrameTemplateContext {
        All,
        PopupWindow=1,
        LookupControl=PopupWindow*2,
        LookupWindow=LookupControl*2,
        ApplicationWindow=LookupWindow*2,
        NestedFrame=ApplicationWindow*2,
        View=NestedFrame*2,
    }
}