using System.IO;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class AssignModelToUserCommand:Command{
        private TestParameters _testParameters;
        public const string Name = "AssignModelToUser";


        protected override void InternalExecute(ICommandAdapter adapter){
            var testAlias = _testParameters.GetAlias("WinAppBin","WebAppBin");
            var modelFiles = Directory.GetFiles(_testParameters.ScriptsPath, "*.xafml", SearchOption.AllDirectories);
            foreach (var modelFile in modelFiles) {
                var copyFileCommand = new CopyFileCommand();
                var destinationFile = Path.Combine(testAlias.Value,"Model.User.xafml");
                copyFileCommand.Execute(adapter,_testParameters, modelFile, destinationFile);
            }
        }

        public override void ParseCommand(CommandCreationParam commandCreationParam){
            base.ParseCommand(commandCreationParam);
            _testParameters = commandCreationParam.TestParameters;
        }
    }
}