using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;

namespace eXpand.ExpressApp.Core
{
    public class ApplicationModulesManager : DevExpress.ExpressApp.ApplicationModulesManager
    {
        readonly ITypesInfo _typesInfo;

        public ApplicationModulesManager() {
        }

        public ApplicationModulesManager(ControllersManager controllersManager, string assembliesPath,ITypesInfo typesInfo) : base(controllersManager, assembliesPath) {
            _typesInfo = typesInfo;
        }

        protected override void LoadTypesInfo(IList<DevExpress.ExpressApp.ModuleBase> modules) {
            foreach (DevExpress.ExpressApp.ModuleBase module in modules){
                allDomainComponents.AddRange(module.BusinessClasses);
                allDomainComponents.AddRange(module.BusinessClassAssemblies.GetBusinessClasses());
            }
            if (Security != null){
                allDomainComponents.AddRange(Security.GetBusinessClasses());
            }
            foreach (Type type in allDomainComponents){
                _typesInfo.RegisterEntity(type);
            }

        }
    }
}
