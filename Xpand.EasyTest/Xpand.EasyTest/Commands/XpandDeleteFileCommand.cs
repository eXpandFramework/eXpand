using System.IO;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class XpandDeleteFileCommand:Command{
        private TestParameters _testParameters;
        public const string Name = "XpandDeleteFile";

        public override void ParseCommand(CommandCreationParam commandCreationParam) {
            base.ParseCommand(commandCreationParam);
            _testParameters = commandCreationParam.TestParameters;
        }

        protected override void InternalExecute(ICommandAdapter adapter){
            if (this.ParameterValue<bool>("InBin")){
                var testAlias = _testParameters.GetAlias("WinAppBin", "WebAppBin");
                var path = Path.Combine(testAlias.Value,Parameters.MainParameter.Value);
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}