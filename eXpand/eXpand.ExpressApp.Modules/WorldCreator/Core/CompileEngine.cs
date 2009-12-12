using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData;
using Microsoft.CSharp;
using Microsoft.JScript;
using Microsoft.VisualBasic;
using CodeDomProvider = eXpand.Persistent.Base.PersistentMetaData.CodeDomProvider;

namespace eXpand.ExpressApp.WorldCreator.Core {
    public class CompileEngine
    {
        static readonly List<Assembly> CompiledAssemblies=new List<Assembly>();
        

        public Type CompileModule(IPersistentAssemblyInfo persistentAssemblyInfo)
        {
            var generateCode = CodeEngine.GenerateCode(persistentAssemblyInfo) ;
            generateCode += getModuleCode(persistentAssemblyInfo.Name) + Environment.NewLine;
            var codeProvider = getCodeDomProvider(persistentAssemblyInfo.CodeDomProvider);
            var compilerParams = new CompilerParameters
                                 {
                                     CompilerOptions = @"/target:library /lib:" + GetReferenceLocations(),
                                     GenerateExecutable = false,
                                     GenerateInMemory = true,                                     
                                     IncludeDebugInformation = false,
                                     OutputAssembly = persistentAssemblyInfo.Name
                                 };

            addReferences(compilerParams);
            return compile(persistentAssemblyInfo, generateCode, compilerParams, codeProvider);
        }

        static System.CodeDom.Compiler.CodeDomProvider getCodeDomProvider(CodeDomProvider codeDomProvider)
        {
            if (codeDomProvider==CodeDomProvider.CSharp)
                return new CSharpCodeProvider();
            if (codeDomProvider==CodeDomProvider.VB)
                return new VBCodeProvider();
            return new JScriptCodeProvider();
        }

        static Type compile(IPersistentAssemblyInfo persistentAssemblyInfo, string generateCode, CompilerParameters compilerParams, System.CodeDom.Compiler.CodeDomProvider codeProvider) {
            CompilerResults compileAssemblyFromSource = null;
            try{
                compileAssemblyFromSource = codeProvider.CompileAssemblyFromSource(compilerParams, generateCode);
                Assembly compiledAssembly = compileAssemblyFromSource.CompiledAssembly;
                CompiledAssemblies.Add(compiledAssembly);
                return compiledAssembly.GetTypes().Where(type => typeof(ModuleBase).IsAssignableFrom(type)).Single();
            }
            catch (Exception){
                if (compileAssemblyFromSource != null){
                    persistentAssemblyInfo.CompileErrors =
                        compileAssemblyFromSource.Errors.Cast<CompilerError>().Aggregate(
                            persistentAssemblyInfo.CompileErrors, (current, error) => current +Environment.NewLine+ error.ToString());
                }
            }
            return null;
        }

        static void addReferences(CompilerParameters compilerParams) {
            Func<Assembly, bool> isNotDynamic = assembly1 => {
                if ((assembly1.FullName+"").IndexOf("Solution3.Win") > -1)
                    Debug.Print("");
                return !(assembly1 is AssemblyBuilder) && !CompiledAssemblies.Contains(assembly1)&&assembly1.EntryPoint==null;
            };
            Func<Assembly, string> assemblyNameSelector = assembly => new AssemblyName(assembly.FullName + "").Name + ".dll";
            compilerParams.ReferencedAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(isNotDynamic).Select(assemblyNameSelector).ToArray());
            
        }


        static string GetReferenceLocations() {
            Func<Assembly, string> locationSelector =assembly =>getAssemblyLocation(assembly);
            Func<string, bool> pathIsValid = s => s.Length > 2;
            string referenceLocations = AppDomain.CurrentDomain.GetAssemblies().Select(locationSelector).Distinct().
                Where(pathIsValid).Aggregate<string, string>(null, (current, type) => current + (type + ",")).TrimEnd(',');
            return referenceLocations;
        }

        static string getAssemblyLocation(Assembly assembly) {
            return @"""" +((assembly is AssemblyBuilder)? null: (!string.IsNullOrEmpty(assembly.Location) ? Path.GetDirectoryName(assembly.Location) : null)) +@"""";
        }

        internal static string getModuleCode(string assemblyName)
        {
            return "namespace " +assemblyName+ "{public class Dynamic" + assemblyName + "Module:DevExpress.ExpressApp.ModuleBase{}}";
        }

        public List<Type> CompileModules(IList<IPersistentAssemblyInfo> persistentAssemblyInfos) {

            var definedModules = new List<Type>();
            
            foreach (IPersistentAssemblyInfo persistentAssemblyInfo in persistentAssemblyInfos) {
                string path = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath),persistentAssemblyInfo.Name);
                if (File.Exists(path+".dll"))
                    File.Delete(path+".dll");
                persistentAssemblyInfo.CompileErrors = null;
                Type compileModule = CompileModule(persistentAssemblyInfo);    
                
                if (compileModule != null) {
                    definedModules.Add(compileModule);
                }
                else if (File.Exists(path)) {
                    var fileInfo=new FileInfo(path);
                    fileInfo.CopyTo(path+".dll");
                    Assembly assembly = Assembly.LoadFile(path+".dll");
                    Type single = assembly.GetTypes().Where(type => typeof(ModuleBase).IsAssignableFrom(type)).Single();
                    definedModules.Add(single);
                }
            }
            return definedModules;
        }
    }
}