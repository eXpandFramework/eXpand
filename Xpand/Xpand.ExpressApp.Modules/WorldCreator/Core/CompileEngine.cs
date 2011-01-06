using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.PersistentMetaData;
using CodeDomProvider = Xpand.Persistent.Base.PersistentMetaData.CodeDomProvider;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public class CompileEngine {
        private const string STR_StrongKeys = "StrongKeys";
        public const string XpandExtension = ".Xpand";
        readonly List<Assembly> CompiledAssemblies = new List<Assembly>();





        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo, Action<CompilerParameters> action, string path) {
            Assembly loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => new AssemblyName(assembly.FullName + "").Name == persistentAssemblyInfo.Name).FirstOrDefault();
            if (loadedAssembly != null)
                return loadedAssembly.GetTypes().Where(type => typeof(ModuleBase).IsAssignableFrom(type)).Single();
            var generateCode = CodeEngine.GenerateCode(persistentAssemblyInfo);
            var codeProvider = getCodeDomProvider(persistentAssemblyInfo.CodeDomProvider);
            var compilerParams = new CompilerParameters {
                CompilerOptions = @"/target:library /lib:" + GetReferenceLocations() + GetStorngKeyParams(persistentAssemblyInfo),
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                OutputAssembly = Path.Combine(path, persistentAssemblyInfo.Name + XpandExtension),
            };
            if (action != null)
                action.Invoke(compilerParams);
            AddReferences(compilerParams, path);
            if (File.Exists(compilerParams.OutputAssembly))
                File.Delete(compilerParams.OutputAssembly);
            return CompileCore(persistentAssemblyInfo, generateCode, compilerParams, codeProvider);

        }
        public Type CompileModule(IPersistentAssemblyBuilder persistentAssemblyBuilder, string path) {
            return CompileModule(persistentAssemblyBuilder.PersistentAssemblyInfo, true, path);
        }

        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo, bool registerPersistentTypes, string path) {
            Type compileModule = CompileModule(persistentAssemblyInfo, path);
            if (registerPersistentTypes && compileModule != null)
                foreach (var type in compileModule.Assembly.GetTypes()) {
                    XafTypesInfo.Instance.RegisterEntity(type);
                }
            return compileModule;
        }

        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo, string path) {
            return CompileModule(persistentAssemblyInfo, parameters => { }, path);
        }

        string GetStorngKeyParams(IPersistentAssemblyInfo persistentAssemblyInfo) {
            if (persistentAssemblyInfo.FileData != null) {
                if (!Directory.Exists(STR_StrongKeys))
                    Directory.CreateDirectory(STR_StrongKeys);
                var newGuid = Guid.NewGuid();
                using (var fileStream = new FileStream(@"StrongKeys\" + newGuid + ".snk", FileMode.Create)) {
                    persistentAssemblyInfo.FileData.SaveToStream(fileStream);
                }
                return @" /keyfile:StrongKeys\" + newGuid + ".snk";
            }
            return null;
        }

        static System.CodeDom.Compiler.CodeDomProvider getCodeDomProvider(CodeDomProvider codeDomProvider) {
            if (codeDomProvider == CodeDomProvider.CSharp)
                return new CSharpCodeProvider();
            return new VBCodeProvider();
        }

        Type CompileCore(IPersistentAssemblyInfo persistentAssemblyInfo, string generateCode, CompilerParameters compilerParams, System.CodeDom.Compiler.CodeDomProvider codeProvider) {
            CompilerResults compileAssemblyFromSource = null;
            try {
                compileAssemblyFromSource = codeProvider.CompileAssemblyFromSource(compilerParams, generateCode);
                if (compilerParams.GenerateInMemory) {
                    Assembly compiledAssembly = compileAssemblyFromSource.CompiledAssembly;
                    CompiledAssemblies.Add(compiledAssembly);
                    return compiledAssembly.GetTypes().Where(type => typeof(ModuleBase).IsAssignableFrom(type)).Single();
                }
                return null;
            } catch (Exception e) {
                Tracing.Tracer.LogError(e);
            } finally {
                if (compileAssemblyFromSource != null) {
                    SetErrors(compileAssemblyFromSource, persistentAssemblyInfo,compilerParams);
                }
                if (Directory.Exists(STR_StrongKeys))
                    Directory.Delete(STR_StrongKeys, true);
            }
            return null;
        }

        static void SetErrors(CompilerResults compileAssemblyFromSource, IPersistentAssemblyInfo persistentAssemblyInfo, CompilerParameters compilerParams) {
            persistentAssemblyInfo.CompileErrors = null;
            persistentAssemblyInfo.CompileErrors =
                compileAssemblyFromSource.Errors.Cast<CompilerError>().Aggregate(
                    persistentAssemblyInfo.CompileErrors, (current, error) => current + Environment.NewLine + error.ToString());
            if (!string.IsNullOrEmpty(persistentAssemblyInfo.CompileErrors)) {
                Tracing.Tracer.LogSeparator("Compilization error of " + persistentAssemblyInfo.Name);
                Tracing.Tracer.LogText(persistentAssemblyInfo.CompileErrors);
                Tracing.Tracer.LogSeparator("Referenced Assemblies:");
                foreach (var reference in compilerParams.ReferencedAssemblies) {
                    Tracing.Tracer.LogVerboseText(reference);
                }   
            }
        }

        void AddReferences(CompilerParameters compilerParams, string path) {
            Func<Assembly, bool> isNotDynamic = assembly1 => !(assembly1 is AssemblyBuilder) && !CompiledAssemblies.Contains(assembly1) &&
                assembly1.EntryPoint == null && !IsCodeDomCompiled(assembly1) && assembly1.ManifestModule.Name.ToLower().IndexOf("mscorlib.resources") == -1 && !string.IsNullOrEmpty(GetAssemblyLocation(assembly1));
            Func<Assembly, string> assemblyNameSelector = assembly => new AssemblyName(assembly.FullName + "").Name + ".dll";
            compilerParams.ReferencedAssemblies.AddRange(
                AppDomain.CurrentDomain.GetAssemblies().Where(isNotDynamic).Select(assemblyNameSelector).ToArray());

            compilerParams.ReferencedAssemblies.Remove("Microsoft.VisualStudio.Debugger.Runtime.dll");
            compilerParams.ReferencedAssemblies.Remove("mscorlib.resources.dll");

            Func<Assembly, string> dynamicAssemblyNameSelector = assembly4 => Path.Combine(path, new AssemblyName(assembly4.FullName + "").Name + XpandExtension);
            compilerParams.ReferencedAssemblies.AddRange(
                AppDomain.CurrentDomain.GetAssemblies().Where(IsCodeDomCompiled).Select(
                    dynamicAssemblyNameSelector).ToArray());
        }


        bool IsCodeDomCompiled(Assembly assembly1) {
            return assembly1.ManifestModule.ScopeName.EndsWith(XpandExtension);
        }


        static string GetReferenceLocations() {
            Func<Assembly, string> locationSelector = GetAssemblyLocation;
            Func<string, bool> pathIsValid = s => s.Length > 2;
            string referenceLocations = AppDomain.CurrentDomain.GetAssemblies().Select(locationSelector).Distinct().
                Where(pathIsValid).Aggregate<string, string>(null, (current, type) => current + (type + ",")).TrimEnd(',');
            return referenceLocations;
        }

        static string GetAssemblyLocation(Assembly assembly) {
            return @"""" + ((assembly is AssemblyBuilder || assembly.GetType().FullName.Equals("System.Reflection.Emit.InternalAssemblyBuilder")) ? null : (!string.IsNullOrEmpty(assembly.Location) ? Path.GetDirectoryName(assembly.Location) : null)) + @"""";
        }


        public List<Type> CompileModules(IList<IPersistentAssemblyInfo> persistentAssemblyInfos, string path) {

            var definedModules = new List<Type>();

            foreach (IPersistentAssemblyInfo persistentAssemblyInfo in persistentAssemblyInfos.OrderByDescending(info => info.CompileOrder)) {
                string fileName = Path.Combine(path, persistentAssemblyInfo.Name);
//                                if (File.Exists(fileName + XpandExtension))
//                                    File.Delete(fileName + XpandExtension);
                persistentAssemblyInfo.CompileErrors = null;
                Type compileModule = CompileModule(persistentAssemblyInfo, path);

                if (compileModule != null) {
                    definedModules.Add(compileModule);
                } else if (File.Exists(fileName)) {
                    var fileInfo = new FileInfo(fileName);
                    fileInfo.CopyTo(fileName + XpandExtension);
                    Assembly assembly = Assembly.LoadFile(fileName + XpandExtension);
                    Type single = assembly.GetTypes().Where(type => typeof(XpandModuleBase).IsAssignableFrom(type)).Single();
                    definedModules.Add(single);
                }
            }
            return definedModules;
        }

    }

}