using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

namespace Xpand.ExpressApp.WorldCreator.CodeProvider.Validation {
    public static class CodeProviderValidationExtensions {
        public static ValidatorResult Validate(this IPersistentAssemblyInfo assemblyInfo, string path) {
            var codeValidator = new CodeValidator(new Compiler(path), new AssemblyValidator());
            return codeValidator.Validate(assemblyInfo.GenerateCode(), assemblyInfo.StrongKeyFileData.GetBytes());
        }

    }
}
