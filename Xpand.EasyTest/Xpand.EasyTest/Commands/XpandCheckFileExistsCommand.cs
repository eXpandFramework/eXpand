using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandCheckFileExistsCommand : CheckFileExistsCommand {
        public const string Name = "XpandCheckFileExists";

        protected override void InternalExecute(ICommandAdapter adapter){
            EasyTestTracer.Tracer.InProcedure("XpandCheckFileExistsCommand_InternalExecute");
            var binPath = this.GetBinPath();
            if (this.ParameterValue<bool>("CheckInBin")){
                Parameters.MainParameter.Value = Path.Combine(binPath,Parameters.MainParameter.Value);
                EasyTestTracer.Tracer.LogText("MainParameter",Parameters.MainParameter.Value);
                if (Parameters.MainParameter.Value.Contains("*")){
                    var path = Path.GetDirectoryName(Parameters.MainParameter.Value)+"";
                    var searchPattern = Path.GetFileName(Parameters.MainParameter.Value);
                    if (!Directory.GetFiles(path,searchPattern).Any())
                        throw new TestException("No files ("+searchPattern+") found at "+path);
                    return;
                }
            }
            if (!ExpectException)
                base.InternalExecute(adapter);
            else{
                string fullFileName = GetFullFileName(Parameters.MainParameter.Value);
                if (!File.Exists(fullFileName))
                    throw new CommandException(string.Format("File exists. {0}", fullFileName),StartPosition);
            }
        }
    }
}