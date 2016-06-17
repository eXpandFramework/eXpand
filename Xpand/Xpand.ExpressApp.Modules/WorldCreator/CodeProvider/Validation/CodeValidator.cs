using System;
using System.IO;
using System.Reflection;

namespace Xpand.ExpressApp.WorldCreator.CodeProvider.Validation{
    [Serializable]
    public struct ValidatorResult {

        public string Message { get; set; }

        public bool Valid => string.IsNullOrWhiteSpace(Message);

    }

    public interface ICodeValidator{
        ValidatorResult Validate(string code, byte[] strongKeyBytes);
        ICompiler Compiler { get; }
    }

    public class CodeValidator : ICodeValidator{
        private readonly ICompiler _compiler;
        private readonly IAssemblyValidator _assemblyValidator;

        public CodeValidator(ICompiler compiler,IAssemblyValidator assemblyValidator){
            _compiler = compiler;
            _assemblyValidator = assemblyValidator;
        }

        public ValidatorResult Validate(string code,  byte[] strongKeyBytes=null) {
            var assemblyName = Guid.NewGuid().ToString("N");
            var compilerResults = _compiler.Compile(code, assemblyName,strongKeyBytes);
            return GetValidatorResult(compilerResults.Errors, assemblyName);
        }

        private ValidatorResult GetValidatorResult(string errors, string assemblyName){
            var validatorResult = new ValidatorResult(){
                Message = errors
            };
            if (validatorResult.Valid){
                var assemblyPath = Path.Combine(_compiler.AssemblyPath,
                assemblyName + CodeProvider.Compiler.XpandExtension);
                validatorResult = _assemblyValidator.Validate(assemblyPath);
            }
            return validatorResult;
        }

        public ICompiler Compiler => _compiler;


        [Obsolete]
        public static ValidatorResult Validate(Assembly assembly) {
            throw new NotImplementedException();
        }

        

    }

    public interface IAssemblyValidator{
        ValidatorResult Validate(string assemblyPath);
    }
}