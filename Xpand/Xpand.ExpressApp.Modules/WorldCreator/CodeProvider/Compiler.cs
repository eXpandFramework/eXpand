using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CSharp;
using Mono.Cecil;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.WorldCreator.CodeProvider {
    public interface ICompilerResult{
        string Errors { get; }
        AssemblyDefinition AssemblyDefinition { get; }
        string PathToAssembly { get; }
    }

    public class CompilerResult : ICompilerResult{
        public CompilerResult(string errors, AssemblyDefinition assemblyDefinition, string pathToAssembly){
            Errors = errors;
            AssemblyDefinition = assemblyDefinition;
            PathToAssembly = pathToAssembly;
        }

        public string Errors { get; }
        public AssemblyDefinition AssemblyDefinition { get; }

        public string PathToAssembly { get; }
    }
    public interface ICompiler{
        ICompilerResult Compile(string code, string name, byte[] strongKeyBytes);
        string AssemblyPath { get;  }
    }

    public class Compiler : ICompiler{
        private const string StrongKeys = "StrongKeys";
        public const string XpandExtension = ".Xpand";

        public Compiler(string assemblyPath){
            AssemblyPath = assemblyPath;
        }

        CompilerParameters GetCompilerParameters( string name,byte[] strongKeyBytes){
            var compilerParams = new CompilerParameters{
                CompilerOptions =
                    @"/target:library /lib:" + GetReferenceLocations() + GetStorngKeyParams(strongKeyBytes),
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = false,
                OutputAssembly = Path.Combine(AssemblyPath, name + XpandExtension)
            };
            AddReferences(compilerParams);
            return compilerParams;
        }

        string GetStorngKeyParams(byte[] strongKeyBytes) {
            if (strongKeyBytes!= null&&strongKeyBytes.Length>0) {
                if (!Directory.Exists(StrongKeys))
                    Directory.CreateDirectory(StrongKeys);
                var newGuid = Guid.NewGuid();
                using (var fileStream = new FileStream(@"StrongKeys\" + newGuid + ".snk", FileMode.Create)) {
                    fileStream.Write(strongKeyBytes, 0,strongKeyBytes.Length);
                }
                return @" /keyfile:StrongKeys\" + newGuid + ".snk";
            }
            return null;
        }

        void AddReferences(CompilerParameters compilerParams) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();


            compilerParams.ReferencedAssemblies.AddRange(
                assemblies.Where(IsNotGenerated)
                    .Select(assembly => GetAssemblyName(assembly.FullName) + ".dll").ToArray());

            compilerParams.ReferencedAssemblies.Remove("Microsoft.VisualStudio.Debugger.Runtime.dll");
            compilerParams.ReferencedAssemblies.Remove("mscorlib.resources.dll");

            
            compilerParams.ReferencedAssemblies.AddRange(
                assemblies.Where(IsCodeDomCompiled).Select(
                    assembly => Path.Combine(AssemblyPath, GetAssemblyName(assembly.FullName) + XpandExtension)).ToArray());
        }

        private string GetAssemblyName(string fullName){
            return new AssemblyName(fullName).Name;
        }

        private bool IsNotGenerated(Assembly assembly){
            return !(assembly is AssemblyBuilder) && 
                   assembly.EntryPoint == null && !IsCodeDomCompiled(assembly) &&
                   assembly.ManifestModule.Name.ToLower().IndexOf("mscorlib.resources", StringComparison.Ordinal) == -1 &&
                   !string.IsNullOrEmpty(GetAssemblyLocation(assembly))&&!string.IsNullOrEmpty(assembly.FullName);
        }


        bool IsCodeDomCompiled(Assembly assembly1) {
            return assembly1.ManifestModule.ScopeName.EndsWith(XpandExtension);
        }

        string GetReferenceLocations(){
            return string.Join(",", AppDomain.CurrentDomain.GetAssemblies()
                .Select(GetAssemblyLocation)
                .Distinct()
                .Where(s => !string.IsNullOrEmpty(s) && s.Length > 2));
        }

        string GetAssemblyLocation(Assembly assembly) {
            var location = ((assembly is AssemblyBuilder ||
                             (assembly.GetType().FullName + "").Equals("System.Reflection.Emit.InternalAssemblyBuilder"))
                ? null : (!string.IsNullOrEmpty(assembly.Location) ? Path.GetDirectoryName(assembly.Location) : null));
            return location != null ? ($@"""{location}""") : null;
        }
        public ICompilerResult Compile(string code,  string name, byte[] strongKeyBytes=null){
            var compilerParameters = GetCompilerParameters(name, strongKeyBytes);
            var cSharpCodeProvider = new CSharpCodeProvider();
            if (!Directory.Exists(AssemblyPath))
                Directory.CreateDirectory(AssemblyPath);
            var compilerResults = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, code);
            var aggregateErrors = compilerResults.AggregateErrors();
            AssemblyDefinition assemblyDefinition = null;
            if (string.IsNullOrEmpty(aggregateErrors))
                assemblyDefinition = AssemblyDefinition.ReadAssembly(compilerResults.PathToAssembly);
            return new CompilerResult(aggregateErrors,assemblyDefinition,compilerResults.PathToAssembly);
        }


        public string AssemblyPath { get; }
    }

}