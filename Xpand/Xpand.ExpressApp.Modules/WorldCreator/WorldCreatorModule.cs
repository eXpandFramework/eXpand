using System.ComponentModel;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security;
using Xpand.ExpressApp.Validation;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.WorldCreator {

    [ToolboxItem(false)]
    public sealed class WorldCreatorModule : XpandModuleBase {
        static ExistentTypesMemberCreator  _existentTypesMemberCreator;

        public WorldCreatorModule() {
            RequiredModuleTypes.Add(typeof(XpandValidationModule));
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
        }

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            if (_existentTypesMemberCreator == null) {
                _existentTypesMemberCreator = new ExistentTypesMemberCreator();
                var reflectionDictionary = WorldCreatorModuleBase.GetReflectionDictionary(this);
                var xpoMultiDataStoreProxy = new MultiDataStoreProxy(ConnectionString, reflectionDictionary);
                var simpleDataLayer = new SimpleDataLayer(xpoMultiDataStoreProxy);
                var session = new Session(simpleDataLayer);
                _existentTypesMemberCreator.CreateMembers(session);
            }
        }
    }

}

