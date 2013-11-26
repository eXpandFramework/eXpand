using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;

namespace Xpand.Persistent.Base.General {
    public class TypesInfoBuilder {
        string _moduleName;

        public static TypesInfoBuilder Create() {
            return new TypesInfoBuilder();
        }

        public TypesInfoBuilder FromModule(string moduleName) {
            _moduleName = moduleName;
            return this;
        }

        public ITypesInfo Build(bool tryToUseCurrentTypesInfo) {
            return tryToUseCurrentTypesInfo
                       ? (UseCurrentTypesInfo() ? XafTypesInfo.Instance : GetTypesInfo())
                       : GetTypesInfo();
        }

        bool UseCurrentTypesInfo() {
            return _moduleName == XpandModuleBase.ManifestModuleName;
        }

        TypesInfo GetTypesInfo() {
            var typesInfo = new TypesInfo();
            typesInfo.AddEntityStore(new NonPersistentEntityStore(typesInfo));
            var xpoSource = new XpoTypeInfoSource(typesInfo);
            typesInfo.Source = xpoSource;
            typesInfo.AddEntityStore(xpoSource);
            return typesInfo;
        }

        public class TypesInfo : DevExpress.ExpressApp.DC.TypesInfo {
            public XpoTypeInfoSource Source { get; set; }
        }

    }
}
