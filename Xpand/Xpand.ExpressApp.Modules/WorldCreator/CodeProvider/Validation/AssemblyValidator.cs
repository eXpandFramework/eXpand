using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AppDomainToolkit;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.ExpressApp.WorldCreator.CodeProvider.Validation{
    [Serializable]
    public class AssemblyValidator: IAssemblyValidator {
        public ValidatorResult Validate(string assemblyPath){
            var setupInfo = new AppDomainSetup{ApplicationName = "WCValidationDomain"};
            var rootDir = Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath);
            setupInfo.PrivateBinPath = rootDir;
            setupInfo.ApplicationBase = rootDir;
            using (var context = AppDomainContext.Create(setupInfo)){
                return RemoteFunc.Invoke(context.Domain,assemblyPath, ValidateCore);
            }
        }

        private ValidatorResult ValidateCore(string assemblyPath){
            var validatorResult = new ValidatorResult();
            try{
                using (var typesInfo = new TypesInfo()){
                    typesInfo.AddEntityStore(new XpoTypeInfoSource(typesInfo));
                    TypesInfoValidation(assemblyPath, typesInfo);
                    DataStoreValidation(typesInfo);
                }
            }
            catch (Exception e){
                validatorResult.Message=e.ToString();
            }
            return validatorResult;
        }

        private void TypesInfoValidation(string assemblyFile, TypesInfo typesInfo) {
            var applicationModulesManager = new T409498ApplicationModulesManager();
            var types = Assembly.LoadFile(assemblyFile).GetTypes();
            applicationModulesManager.AddModule(types.First(type => typeof(ModuleBase).IsAssignableFrom(type)));
            applicationModulesManager.Load(typesInfo);
        }

        private void DataStoreValidation(TypesInfo typesInfo) {
            using (var objectSpaceProvider = new XPObjectSpaceProvider(new MemoryDataStoreProvider(), typesInfo, typesInfo.EntityStores.OfType<XpoTypeInfoSource>().First())) {
                using (var objectSpace = objectSpaceProvider.CreateObjectSpace()) {
                    foreach (var persistentType in typesInfo.PersistentTypes.Where(info => info.IsPersistent)) {
                        objectSpace.CreateObject(persistentType.Type);
                        objectSpace.FindObject(persistentType.Type, null);
                    }
                }
            }
        }
    }
}