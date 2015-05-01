using System.IO;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandCheckFileExistsCommand : CheckFileExistsCommand {
        public const string Name = "XpandCheckFileExists";
        private TestParameters _testParameters;

        public override void ParseCommand(CommandCreationParam commandCreationParam){
            base.ParseCommand(commandCreationParam);
            _testParameters = commandCreationParam.TestParameters;
        }

        protected override void InternalExecute(ICommandAdapter adapter){
            var binPath = _testParameters.GetBinPath();
            if (this.ParameterValue<bool>("CheckInBin")){
                Parameters.MainParameter.Value = Path.Combine(binPath,Path.GetFileName(Parameters.MainParameter.Value) + "");
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