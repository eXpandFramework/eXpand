using System;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.Utils.Automation.InputSimulator;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands{
    public class SendKeysCommand : Command{
        public const string Name = "SendKeys";
        protected override void InternalExecute(ICommandAdapter adapter){
            if (adapter.IsWinAdapter()){
                var focusWindowCommand=new FocusWindowCommand();
                focusWindowCommand.Execute(adapter);
            }
            var sleepCommand = new SleepCommand();
            sleepCommand.Parameters.MainParameter = new MainParameter("300");
            sleepCommand.Execute(adapter);
            var simulator=new InputSimulator();
            if (!string.IsNullOrEmpty(Parameters.MainParameter.Value))
                simulator.Keyboard.TextEntry(Parameters.MainParameter.Value);
            var keysParameter = Parameters["Keys"];
            if (keysParameter != null){
                foreach (var key in keysParameter.Value.Split(';')){
                    var keyCode = (Win32Constants.VirtualKeys)Enum.Parse(typeof(Win32Constants.VirtualKeys), key);
                    simulator.Keyboard.KeyPress(keyCode);
                }
            }
            
        }
    }
}