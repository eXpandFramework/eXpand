using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using Mono.Cecil;
using Xpand.ExpressApp.WorldCreator.BusinessObjects;
using Xpand.ExpressApp.WorldCreator.CodeProvider;
using Xpand.ExpressApp.WorldCreator.CodeProvider.Validation;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.System{
    public interface IAssemblyManager {
        IPersistentAssemblyInfo[] ValidateAssemblyInfos();
        IList<Assembly> LoadAssemblies();
        ICodeValidator CodeValidator { get; }
    }

    public class AssemblyManager : IAssemblyManager{
        private readonly IObjectSpace _objectSpace;

        public AssemblyManager(IObjectSpace objectSpace, ICodeValidator codeValidator){
            _objectSpace = objectSpace;
            CodeValidator = codeValidator;
        }

        private IPersistentAssemblyInfo[] AssemblyInfos => _objectSpace.QueryObjects<IPersistentAssemblyInfo>().ToArray();
        public IList<Assembly> LoadAssemblies(){
            return ValidateAssemblyInfos().Select(CompileFile)
                    .Concat(AssemblyInfos.Where(info => !NeedsCompilation(info))
                    .Select(info => Assembly.LoadFile(GetAssemblyFile(info)))).ToArray();
        }

        private Assembly CompileFile(IPersistentAssemblyInfo assemblyInfo){
            var compilerResults = CodeValidator.Compiler.Compile(assemblyInfo.GenerateCode(), assemblyInfo.Name,
                assemblyInfo.StrongKeyFileData.GetBytes());
            return Assembly.LoadFile(compilerResults.PathToAssembly);
        }

        private bool CanLoad(ValidatorResult result){
            return result.Valid;
        }

        public ICodeValidator CodeValidator { get; }

        bool NeedsCompilation(IPersistentAssemblyInfo info){
            if (AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.GetName().Name == info.Name))
                return false;
            var assemblyFile = GetAssemblyFile(info);
            if (!File.Exists(assemblyFile))
                return true;
            var fileVersion = AssemblyDefinition.ReadAssembly(assemblyFile).Name.Version;
            var infoVersion = info.Version();
            return ((infoVersion > fileVersion)||(infoVersion < fileVersion)) ;
        }

        private string GetAssemblyFile(IPersistentAssemblyInfo info){
            var assemblyFile = Path.Combine(CodeValidator.Compiler.AssemblyPath, info.Name + Compiler.XpandExtension);
            return assemblyFile;
        }

        public IPersistentAssemblyInfo[] ValidateAssemblyInfos(){
            var infos = (from info in AssemblyInfos.Where(ShouldValidate)
                let result = Validate(info)
                where CanLoad(result)
                select info).ToArray();
            _objectSpace.CommitChanges();
            return infos;
        }

        private ValidatorResult Validate(IPersistentAssemblyInfo info){
            var strongKeyBytes = info.StrongKeyFileData.GetBytes();
            var code = info.GeneratedCode;
            var validatorResult = CodeValidator.Validate(code, strongKeyBytes);
            info.Errors = validatorResult.Message;
            if (!validatorResult.Valid){
                Tracing.Tracer.LogSeparator("Validation for "+ info.Name);
                Tracing.Tracer.LogText(validatorResult.Message);
            }
            return validatorResult;
        }

        private bool ShouldValidate(IPersistentAssemblyInfo info){
            return !info.DoNotCompile && NeedsCompilation(info );
        }
    }
}