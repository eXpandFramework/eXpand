using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class FocusControlCommand:Command{
        public const string Name = "FocusControl";
        protected override void InternalExecute(ICommandAdapter adapter){
            var testControl = adapter.CreateTestControl("Field", Parameters.MainParameter.Value);
            CheckControlEnabled(testControl);
            testControl.GetInterface<IControlAct>().Act("focus");
        }
    }
}