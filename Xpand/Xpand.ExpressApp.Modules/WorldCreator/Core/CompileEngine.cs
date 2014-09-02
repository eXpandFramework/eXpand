using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.Persistent.Base;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using CodeDomProvider = Xpand.Persistent.Base.PersistentMetaData.CodeDomProvider;

namespace Xpand.ExpressApp.WorldCreator.Core {
    public class CompileEngine {
        private const string StrongKeys = "StrongKeys";
        public const string XpandExtension = ".Xpand";
        readonly List<Assembly> _compiledAssemblies = new List<Assembly>();

        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo, Action<CompilerParameters> action, string path) {
            return CompileModule(persistentAssemblyInfo, CodeEngine.GenerateCode, action, path);
        }

        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo, Func<IPersistentAssemblyInfo, string> codeGenerator, Action<CompilerParameters> action, string path) {
            Assembly loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => new AssemblyName(assembly.FullName + "").Name == persistentAssemblyInfo.Name);
            if (loadedAssembly != null)
                return loadedAssembly.GetTypes().Single(type => typeof(ModuleBase).IsAssignableFrom(type));
            var generateCode = codeGenerator.Invoke(persistentAssemblyInfo);
            var codeProvider = GetCodeDomProvider(persistentAssemblyInfo.CodeDomProvider);
            var compilerParams = new CompilerParameters {
                CompilerOptions = @"/target:library /lib:" + GetReferenceLocations() + GetStorngKeyParams(persistentAssemblyInfo),
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                OutputAssembly = Path.Combine(path, persistentAssemblyInfo.Name + XpandExtension),
            };
            AddReferences(compilerParams, path);
            if (action != null)
                action.Invoke(compilerParams);

            if (File.Exists(compilerParams.OutputAssembly))
                File.Delete(compilerParams.OutputAssembly);
            return CompileCore(persistentAssemblyInfo, generateCode, compilerParams, codeProvider);

        }
        public Type CompileModule(IPersistentAssemblyBuilder persistentAssemblyBuilder, string path) {
            return CompileModule(persistentAssemblyBuilder.PersistentAssemblyInfo, true, path);
        }


        static void RegisterPersistentTypes(Type compileModule) {
            foreach (var type in compileModule.Assembly.GetTypes()) {
                XafTypesInfo.Instance.RegisterEntity(type);
            }
        }
        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo, bool registerPersistentTypes, string path) {
            Type compileModule = CompileModule(persistentAssemblyInfo, path);
            if (registerPersistentTypes && compileModule != null)
                RegisterPersistentTypes(compileModule);
            return compileModule;
        }

        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo, string path) {
            return CompileModule(persistentAssemblyInfo, parameters => { }, path);
        }

        string GetStorngKeyParams(IPersistentAssemblyInfo persistentAssemblyInfo) {
            if (persistentAssemblyInfo.FileData != null) {
                if (!Directory.Exists(StrongKeys))
                    Directory.CreateDirectory(StrongKeys);
                var newGuid = Guid.NewGuid();
                using (var fileStream = new FileStream(@"StrongKeys\" + newGuid + ".snk", FileMode.Create)) {
                    persistentAssemblyInfo.FileData.SaveToStream(fileStream);
                }
                return @" /keyfile:StrongKeys\" + newGuid + ".snk";
            }
            return null;
        }

        System.CodeDom.Compiler.CodeDomProvider GetCodeDomProvider(CodeDomProvider codeDomProvider) {
            return codeDomProvider == CodeDomProvider.CSharp
                ? (System.CodeDom.Compiler.CodeDomProvider)new CSharpCodeProvider()
                : new VBCodeProvider();
        }

        Type CompileCore(IPersistentAssemblyInfo persistentAssemblyInfo, string generateCode, CompilerParameters compilerParams, System.CodeDom.Compiler.CodeDomProvider codeProvider) {
            CompilerResults compileAssemblyFromSource = null;
            Type compileCore = null;
            try {
                compileAssemblyFromSource = codeProvider.CompileAssemblyFromSource(compilerParams, generateCode);
                if (compilerParams.GenerateInMemory) {
                    Assembly compiledAssembly = compileAssemblyFromSource.CompiledAssembly;
                    _compiledAssemblies.Add(compiledAssembly);
                    compileCore = compiledAssembly.GetTypes().Single(type => typeof(ModuleBase).IsAssignableFrom(type));
                }
            }
            catch (Exception e) {
                Tracing.Tracer.LogError(e);
            }
            finally {
                if (compileAssemblyFromSource != null) {
                    SetErrors(compileAssemblyFromSource, persistentAssemblyInfo, compilerParams);
                }
                if (string.IsNullOrEmpty(persistentAssemblyInfo.CompileErrors) && compileCore != null) {
                    if (!ValidateBOModel(persistentAssemblyInfo, compileCore))
                        compileCore = null;
                }
                if (Directory.Exists(StrongKeys))
                    Directory.Delete(StrongKeys, true);
            }
            return compileCore;
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

        bool ValidateBOModel(IPersistentAssemblyInfo persistentAssemblyInfo, Type compileCore) {
            if (persistentAssemblyInfo.ValidateModelOnCompile) {
                var instance = XafTypesInfo.Instance;
                try {
                    var typesInfo = new TypesInfoBuilder.TypesInfo();
                    typesInfo.AddEntityStore(new NonPersistentEntityStore(typesInfo));
                    typesInfo.AddEntityStore(new XpoTypeInfoSource(typesInfo));

                    typesInfo.AssignAsInstance();
                    typesInfo.LoadTypes(compileCore.Assembly);
                    var applicationModulesManager = new ApplicationModulesManager();
                    applicationModulesManager.AddModule(compileCore);
                    applicationModulesManager.Load(typesInfo, true);
                }
                catch (Exception exception) {
                    persistentAssemblyInfo.CompileErrors = exception.ToString();
                    return false;
                }
                finally {
                    instance.AssignAsInstance();
                }
            }
            return true;
        }

        void AddReferences(CompilerParameters compilerParams, string path) {
            Func<Assembly, bool> isNotDynamic = assembly1 => !(assembly1 is AssemblyBuilder) && !_compiledAssemblies.Contains(assembly1) &&
                assembly1.EntryPoint == null && !IsCodeDomCompiled(assembly1) && assembly1.ManifestModule.Name.ToLower().IndexOf("mscorlib.resources", StringComparison.Ordinal) == -1 && !string.IsNullOrEmpty(GetAssemblyLocation(assembly1));
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
            Func<string, bool> pathIsValid = s => !string.IsNullOrEmpty(s) && s.Length > 2;
            string referenceLocations = AppDomain.CurrentDomain.GetAssemblies().Select(locationSelector).Distinct().
                Where(pathIsValid).Aggregate<string, string>(null, (current, type) => current + (type + ",")).TrimEnd(',');
            return referenceLocations;
        }

        static string GetAssemblyLocation(Assembly assembly) {
            var location = ((assembly is AssemblyBuilder || (assembly.GetType().FullName + "").Equals("System.Reflection.Emit.InternalAssemblyBuilder")) ? null : (!string.IsNullOrEmpty(assembly.Location) ? Path.GetDirectoryName(assembly.Location) : null));
            return location != null ? (String.Format(@"""{0}""", location)) : null;
        }


        public List<Type> CompileModules(IList<IPersistentAssemblyInfo> persistentAssemblyInfos, string path) {

            var definedModules = new List<Type>();

            foreach (IPersistentAssemblyInfo persistentAssemblyInfo in persistentAssemblyInfos.OrderByDescending(info => info.CompileOrder)) {
                string fileName = Path.Combine(path, persistentAssemblyInfo.Name);
                persistentAssemblyInfo.CompileErrors = null;
                Type compileModule = CompileModule(persistentAssemblyInfo, path);

                if (compileModule != null) {
                    definedModules.Add(compileModule);
                }
                else if (File.Exists(fileName)) {
                    var fileInfo = new FileInfo(fileName);
                    fileInfo.CopyTo(fileName + XpandExtension);
                    Assembly assembly = Assembly.LoadFile(fileName + XpandExtension);
                    Type single = assembly.GetTypes().Single(type => typeof(XpandModuleBase).IsAssignableFrom(type));
                    definedModules.Add(single);
                }
            }
            return definedModules;
        }

    }

}