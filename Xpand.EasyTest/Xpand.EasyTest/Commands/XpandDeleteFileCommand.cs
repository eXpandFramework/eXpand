using System.IO;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class XpandDeleteFileCommand:Command{
        public const string InBin = "InBin";
        public const string Name = "XpandDeleteFile";

        protected override void InternalExecute(ICommandAdapter adapter){
            if (this.ParameterValue<bool>(InBin)){
                var binPath = this.GetBinPath();
                var path = Path.Combine(binPath, Parameters.MainParameter.Value);
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}