using System;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandFillFormCommand : Command{
        public const string Name = "XpandFillForm";
        public const string SendKeys = "SendKeys";
        protected override void InternalExecute(ICommandAdapter adapter){
            var sendKeysParameter = Parameters[SendKeys];
            if (sendKeysParameter != null && (adapter.IsWinAdapter())) {
                var sendKeysCommand = new SendKeysCommand().SynchWith(this);
                sendKeysCommand.Parameters.MainParameter = new MainParameter(Parameters.First().Value);
                sendKeysCommand.Execute(adapter);
            }
            else {
                var processRecordCommand = new FillFormCommand();
                processRecordCommand.SynchWith(this);
                if (sendKeysParameter != null)
                    processRecordCommand.Parameters.Remove(sendKeysParameter);
                try{
                    processRecordCommand.Execute(adapter);
                }
                catch (Exception){
                    if (ExpectException)
                        throw;
                    if (this.ParameterValue("Throw", true))
                        throw;
                }
            }
        }
    }
}