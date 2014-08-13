using System;
using System.IO;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class UseModelCommand:Command{
        private TestParameters _testParameters;
        public const string Name = "UseModel";


        protected override void InternalExecute(ICommandAdapter adapter){
            EasyTestTracer.Tracer.InProcedure(GetType().Name + ".Execute");
            EasyTestTracer.Tracer.LogText(Environment.NewLine + Text);
            var testAlias = _testParameters.GetAlias("WinAppBin", "WebAppBin");
            var modelFiles = Directory.GetFiles(_testParameters.ScriptsPath, "*.xafml", SearchOption.AllDirectories);
            foreach (var modelFile in modelFiles) {
                var copyFileCommand = new CopyFileCommand();
                var path1 = testAlias.Value;
                var destinationFile = Path.Combine(path1, "Model.User.xafml" );
                copyFileCommand.Execute(adapter, _testParameters, modelFile, destinationFile);
            }

            EasyTestTracer.Tracer.OutProcedure(GetType().Name + ".Execute");
        }

        public override void ParseCommand(CommandCreationParam commandCreationParam){
            base.ParseCommand(commandCreationParam);
            _testParameters = commandCreationParam.TestParameters;
        }
    }
}