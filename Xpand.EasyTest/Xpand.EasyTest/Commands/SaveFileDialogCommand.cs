using System.IO;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class SaveFileDialogCommand:Command{
        public const string Name = "SaveFileDialog";

        protected override void InternalExecute(ICommandAdapter adapter){
            var deleteFileCommand = new XpandDeleteFileCommand();
            deleteFileCommand.Parameters.MainParameter=Parameters.MainParameter;
            deleteFileCommand.Parameters.Add(new Parameter(XpandDeleteFileCommand.InBin,"True",true,EndPosition));
            deleteFileCommand.Execute(adapter);
            var binPath = this.GetBinPath();
            var fillFormCommand = new XpandFillFormCommand();
            fillFormCommand.Parameters.Add(new Parameter("File name:",Path.Combine(binPath,Parameters.MainParameter.Value),true,EndPosition));
            fillFormCommand.Execute(adapter);
            var handleDialogCommand = new XpandHandleDialogCommand();
            handleDialogCommand.Parameters.Add(new Parameter("Respond","Save",true,EndPosition));
            handleDialogCommand.Execute(adapter);
            var sleepCommand = new SleepCommand();
            sleepCommand.Parameters.MainParameter = new MainParameter("1000");
            sleepCommand.Execute(adapter);
        }
    }
}