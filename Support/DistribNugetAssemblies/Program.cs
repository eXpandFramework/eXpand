using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DistribNugetAssemblies {
    class Program {
        static void Main(){
            foreach (var csproj in Directory.GetFiles(@"..\Xpand", "*.csproj", SearchOption.AllDirectories).Where(s => (Path.GetFileName(s)+"").StartsWith("Xpand."))) {
                if (UsesNuget(Path.GetDirectoryName(csproj))){
                    var projectDoc = XDocument.Load(csproj);
                    var packages = GetPackages(csproj);
                    foreach (var nugetAssembly in NugetAssemblies(csproj, projectDoc, packages)){
                        var distribPaths = new []{@"..\Build\Temp\",@"..\build\installer\xpand.dll\"};
                        foreach (var distribPath in distribPaths){
                            var destFileName = Path.GetFullPath(distribPath+Path.GetFileName(nugetAssembly));
                            if (File.Exists(nugetAssembly))
                                File.Copy(nugetAssembly, destFileName, true);    
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> GetPackages(string csproj){
            var packagesConfig = XDocument.Load(Path.Combine(Path.GetDirectoryName(csproj) + "", "packages.config"));
            return packagesConfig.Descendants().Where(element => element.Name.LocalName == "package")
                    .Select(element => element.Attribute("id").Value);
        }

        private static IEnumerable<string> NugetAssemblies(string csproj, XDocument projectDoc, IEnumerable<string> packages){
            var references = projectDoc.Descendants().Where(element => element.Name.LocalName=="Reference");
            var hintPaths = references.Where(element => IsPackageReference(element.Attribute("Include").Value, packages)).Select(element 
                => element.Descendants().First(xElement => xElement.Name.LocalName=="HintPath").Value);
            return hintPaths.Select(s => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(csproj) + "", s)));
        }

        private static bool IsPackageReference(string reference, IEnumerable<string> packages){
            return packages.Any(s => reference.ToLower().StartsWith(s.ToLower()));
        }

        private static bool UsesNuget(string directoryName){
            return File.Exists(Path.Combine(directoryName, "packages.config"));
        }
    }
}
