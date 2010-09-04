using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.ConditionalActionState.Logic;
using Xpand.Persistent.BaseImpl.ExceptionHandling;

namespace FeatureCenter.Module.Miscellaneous.ExceptionHandling {
    [ActionStateRule("Hide_Save_For_ExceptionHandlingObject", "Save", "1=1", null, ActionState.Hidden)]
    [ActionStateRule("Hide_SaveAndClose_For_ExceptionHandlingObject", "SaveAndClose", "1=1", null, ActionState.Hidden)]
    [ActionStateRule("Hide_Cancel_For_ExceptionHandlingObject", "Cancel", "1=1", null, ActionState.Hidden)]
    public class ExceptionHandlingObject:BaseObject {
        public ExceptionHandlingObject(Session session) : base(session) {
        }

        public XPCollection<ExceptionObject> Exceptions {
            get { return new XPCollection<ExceptionObject>(Session); }
        }    
    }
}