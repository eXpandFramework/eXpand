using System.IO;

namespace Xpand.Test.WorldCreator.TestArtifacts{
    public abstract class BaseSpecs{
        public static string AssemblyPath;

        static BaseSpecs(){
            AssemblyPath = Path.Combine(Path.GetDirectoryName(typeof(BaseSpecs).Assembly.Location) + "",
                "WCTestAssemblies");
            if (Directory.Exists(AssemblyPath)){
                Directory.Delete(AssemblyPath, true);
                Directory.CreateDirectory(AssemblyPath);
            }
        }
    }
}