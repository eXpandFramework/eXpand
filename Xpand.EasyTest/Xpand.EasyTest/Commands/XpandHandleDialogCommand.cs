using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandHandleDialogCommand : Command{
        public const string Name = "XpandHandleDialog";

        protected override void InternalExecute(ICommandAdapter adapter){
            var helper = new MultiLineComparisionHelper();
            string messageText = null;
            foreach (Parameter parameter in Parameters){
                if (parameter.Name == "Message"){
                    if (messageText == null){
                        ITestControl testControl = adapter.CreateTestControl(TestControlType.Dialog, "");
                        messageText = testControl.GetInterface<IControlReadOnlyText>().Text;
                    }
                    string compareResult = helper.Compare("HandleDialog", parameter, messageText, "dialog message");
                    if (!string.IsNullOrEmpty(compareResult)){
                        throw new AdapterOperationException(compareResult);
                    }
                }
            }
            if (Parameters["Caption"] != null){
                ITestControl testControl = adapter.CreateTestControl(TestControlType.Dialog, "");
                string caption = testControl.GetInterface<ITestWindow>().Caption;
                string compareResult = helper.Compare("HandleDialog", Parameters["Caption"], caption, "dialog caption");
                if (!string.IsNullOrEmpty(compareResult)){
                    throw new AdapterOperationException(compareResult);
                }
            }
            if (Parameters["Respond"] != null){
                new ActionCommand().DoAction(adapter, Parameters["Respond"].Value, null);
            }
            if (Parameters["Close"] != null && Parameters["Close"].Value == "True"){
                ITestControl testControl = adapter.CreateTestControl(TestControlType.Dialog, "");
                testControl.GetInterface<ITestWindow>().Close();
            }
        }
    }
}