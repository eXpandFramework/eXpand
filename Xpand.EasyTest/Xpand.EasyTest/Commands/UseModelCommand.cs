using System;
using System.IO;
using System.Linq;
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
            if (string.IsNullOrEmpty(Parameters.MainParameter.Value)){
                var modelFile = Directory.GetFiles(_testParameters.ScriptsPath, "*.xafml", SearchOption.TopDirectoryOnly).Single();
                CopyModel(adapter, testAlias, "Model.User.xafml", modelFile);
            }
            else{
                var modelFiles = Parameters.MainParameter.Value.Split(';');
                for (int i = 0; i < modelFiles.Length; i++){
                    var path = Path.Combine(_testParameters.ScriptsPath, modelFiles[i] + ".xafml");
                    string userXafml = "Model.User.xafml";
                    if (i>0)
                        userXafml = "Model.User" +i+ ".xafml";
                    CopyModel(adapter, testAlias, userXafml, path);
                }
            }

            EasyTestTracer.Tracer.OutProcedure(GetType().Name + ".Execute");
        }

        private void CopyModel(ICommandAdapter adapter, TestAlias testAlias, string userXafml, string modelFile){
            var copyFileCommand = new CopyFileCommand();
            string path1 = testAlias.Value;
            string destinationFile = Path.Combine(path1, userXafml);
            var deleteFileCommand = new DeleteFileCommand();
            deleteFileCommand.Parameters.MainParameter = new MainParameter(destinationFile);
            deleteFileCommand.Execute(adapter);
            copyFileCommand.Execute(adapter, _testParameters, modelFile, destinationFile);
        }

        public override void ParseCommand(CommandCreationParam commandCreationParam){
            base.ParseCommand(commandCreationParam);
            _testParameters = commandCreationParam.TestParameters;
        }
    }
}