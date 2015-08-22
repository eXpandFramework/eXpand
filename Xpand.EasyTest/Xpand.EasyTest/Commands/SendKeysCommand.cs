using System;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.Utils.Automation.InputSimulator;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands{
    public class SendKeysCommand : Command{
        public const string Name = "SendKeys";


        protected override void InternalExecute(ICommandAdapter adapter){
            var sleepCommand = new SleepCommand();
            sleepCommand.Parameters.MainParameter = new MainParameter("300");
            sleepCommand.Execute(adapter);
            var simulator=new InputSimulator();
            if (!string.IsNullOrEmpty(Parameters.MainParameter.Value))
                simulator.Keyboard.TextEntry(Parameters.MainParameter.Value);
            var keysParameter = Parameters["Keys"];
            var modifiers = Parameters["Modifiers"];
            if (modifiers != null){
                simulator.Keyboard.ModifiedKeyStroke((modifiers.Value).Split(';').Select(GetVirtualKey),
                    keysParameter.Value.Split(';').Select(GetVirtualKey));
            }
            if (keysParameter != null){
                foreach (var key in keysParameter.Value.Split(';')){
                    var keyCode = GetVirtualKey(key);
                    simulator.Keyboard.KeyPress(keyCode);
                }
            }
            
        }

        private static Win32Constants.VirtualKeys GetVirtualKey(string key){
            return (Win32Constants.VirtualKeys) Enum.Parse(typeof (Win32Constants.VirtualKeys), key);
        }
    }
}