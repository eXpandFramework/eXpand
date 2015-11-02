using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandExecuteEditorAction:ExecuteEditorActionCommand{
        public const string Name = "XpandExecuteEditorAction";
        protected override void InternalExecute(ICommandAdapter adapter){
            var testControl = adapter.CreateTestControl("Field", Parameters.MainParameter.Value);
            var controlAct = testControl.GetInterface<IControlAct>();
            controlAct.Act(Parameters.ExtraParameter.Value);
        }
    }
}