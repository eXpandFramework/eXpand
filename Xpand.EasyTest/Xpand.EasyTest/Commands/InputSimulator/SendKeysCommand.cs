using System;
using System.Linq;
using System.Threading;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.EasyTest.Commands.Window;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands.InputSimulator{
    public class SendKeysCommand : Command{
        public const string Name = "SendKeys";

        protected override void InternalExecute(ICommandAdapter adapter){
            
            var activateApplicationWindowCommand = new ActivateWindowCommand();
            activateApplicationWindowCommand.SynchWith(this);
            activateApplicationWindowCommand.Execute(adapter);
            

            var simulator = new Utils.Automation.InputSimulator.InputSimulator();
            if (!string.IsNullOrEmpty(Parameters.MainParameter.Value))
                simulator.Keyboard.TextEntry(Parameters.MainParameter.Value);
            var field = this.ParameterValue("Field","");

            if (!string.IsNullOrEmpty(field)){
                var fillFieldCommand = new FillFieldCommand();
                fillFieldCommand.Parameters.Add(new Parameter(field,"",true,EndPosition));
                fillFieldCommand.Execute(adapter);
            }

            Execute(simulator);
        }

        private void Execute(Utils.Automation.InputSimulator.InputSimulator simulator){
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
                    Thread.Sleep(300);
                }
            }
        }

        private static Win32Constants.VirtualKeys GetVirtualKey(string key){
            return (Win32Constants.VirtualKeys) Enum.Parse(typeof (Win32Constants.VirtualKeys), key,true);
        }
    }
}