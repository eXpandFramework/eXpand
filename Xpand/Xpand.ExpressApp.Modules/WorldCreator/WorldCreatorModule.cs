using System.ComponentModel;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.Utils;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Validation;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator {

    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class WorldCreatorModule : XpandModuleBase {
        static ExistentTypesMemberCreator  _existentTypesMemberCreator;

        public WorldCreatorModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            AddToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.PersistentMetaData");
            WCTypesInfo.Instance.Register(GetAdditionalClasses(ModuleManager));
            if (_existentTypesMemberCreator == null && ((!string.IsNullOrEmpty(ConnectionString)) || (RuntimeMode && Application.ObjectSpaceProvider is DataServerObjectSpaceProvider))) {
                if (RuntimeMode) {
                    _existentTypesMemberCreator = new ExistentTypesMemberCreator();
                    var session = Application.FindModule<WorldCreatorModuleBase>().GetUnitOfWork();
                    _existentTypesMemberCreator.CreateMembers(session, typesInfo);
                }
            }
        }
    }

}

