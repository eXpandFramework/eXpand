using System.Diagnostics;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class ProcessCommand:Command{
        public const string Name = "Process";
        protected override void InternalExecute(ICommandAdapter adapter){
            var fileName = this.ParameterValue<string>("FileName");
            string arguments=this.ParameterValue<string>("Arguments");
            var process = new Process(){StartInfo = new ProcessStartInfo(fileName,arguments)};
            process.Start();
        }
    }
}