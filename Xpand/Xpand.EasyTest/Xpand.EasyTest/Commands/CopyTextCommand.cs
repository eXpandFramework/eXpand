using System.Threading;
using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.Commands.InputSimulator;

namespace Xpand.EasyTest.Commands{
    public class CopyTextCommand:Command{
        public const string Name = "CopyText";

        protected override void InternalExecute(ICommandAdapter adapter){
            SendKeysCommand sendKeysCommand;
            if (Parameters.MainParameter!=null){
                var dblClickCommand = new DblClickCommand();
                dblClickCommand.Parameters.MainParameter = Parameters.MainParameter;
                dblClickCommand.Execute(adapter);
            }
            else{
                sendKeysCommand=new SendKeysCommand();
                sendKeysCommand.Parameters.Add(new Parameter("Keys","F2",true,EndPosition));
                sendKeysCommand.Execute(adapter);
                Thread.Sleep(500);
                sendKeysCommand=new SendKeysCommand();
                sendKeysCommand.Parameters.Add(new Parameter("Modifiers", "Control", true, EndPosition));
                sendKeysCommand.Parameters.Add(new Parameter("Keys", "a", true, EndPosition));
                sendKeysCommand.Execute(adapter);
                Thread.Sleep(500);
            }
            sendKeysCommand = new SendKeysCommand();
            sendKeysCommand.Parameters.Add(new Parameter("Modifiers","Control",true,EndPosition));
            sendKeysCommand.Parameters.Add(new Parameter("Keys","c",true,EndPosition));
            sendKeysCommand.Execute(adapter);
        }
    }
}