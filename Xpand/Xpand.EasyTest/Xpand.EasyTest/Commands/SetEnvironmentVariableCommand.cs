using System;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class SetEnvironmentVariableCommand:Command{
        public const string Name = "SetEnvironmentVariable";
        protected override void InternalExecute(ICommandAdapter adapter){
            Environment.SetEnvironmentVariable(Parameters.MainParameter.Value, Parameters["Value"].Value, EnvironmentVariableTarget.Machine);
        }
    }
}