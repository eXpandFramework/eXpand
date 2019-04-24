using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandCompareFilesCommand:CompareFilesCommand{
        public const string Name = "XpandCompareFiles";
        public const string ActualInBin = "XpandCompareFiles";
        protected override void InternalExecute(ICommandAdapter adapter){
            if (this.ParameterValue<bool>(ActualInBin)){
                var actual = this.ParameterValue<string>("Actual");
                if (actual.Contains("*"))
                    actual = Directory.GetFiles(Path.GetDirectoryName(actual) + "", Path.GetFileName(actual)).First();
                Parameters["Actual"].Value = Path.Combine(this.GetBinPath(), actual);
            }
            base.InternalExecute(adapter);
        }
    }
}