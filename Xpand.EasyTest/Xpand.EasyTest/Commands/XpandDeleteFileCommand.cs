using System.IO;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class XpandDeleteFileCommand:Command{
        public const string InBin = "InBin";
        public const string Name = "XpandDeleteFile";

        protected override void InternalExecute(ICommandAdapter adapter){
            if (this.ParameterValue<bool>(InBin)){
                var binPath = this.GetBinPath();
                var path = Path.Combine(Path.GetFullPath(binPath), Parameters.MainParameter.Value.TrimStart(@"\".ToCharArray()));
                if (path.Contains("*")){
                    var directoryName = Path.GetDirectoryName(path) + "";
                    if (Directory.Exists(directoryName)){
                        var files = Directory.GetFiles(directoryName, Path.GetFileName(path));
                        foreach (var file in files){
                            File.Delete(file);
                        }
                    }
                }
                else if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}