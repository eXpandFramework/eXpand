using System;
using System.Collections.Generic;
using DCSecurityDemo.Module.BusinessObjects;
using DCSecurityDemo.Module.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using SecurityDemo.Module;
using DevExpress.ExpressApp.Model.Core;

namespace DCSecurityDemo.Module {
    public sealed partial class DCSecurityDemoModule : ModuleBase {
        public DCSecurityDemoModule() {
            InitializeComponent();
            AdditionalControllerTypes.Add(typeof(ShowHintController));
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            ITypesInfo typesInfo = application.TypesInfo;
            typesInfo.RegisterEntity("DCUser", typeof(IDCUser));
            typesInfo.RegisterEntity("DCRole", typeof(IDCRole));
            typesInfo.RegisterEntity("DCTypePermissions", typeof(IDCTypePermissions));
            typesInfo.RegisterEntity("DCMemberPermissions", typeof(IDCMemberPermissions));
            typesInfo.RegisterEntity("DCObjectPermissions", typeof(IDCObjectPermissions));

            typesInfo.RegisterEntity("FullAccessObject", typeof(IFullAccessObject));
            typesInfo.RegisterEntity("ProtectedContentObject", typeof(IProtectedContentObject));
            typesInfo.RegisterEntity("ReadOnlyObject", typeof(IReadOnlyObject));
            typesInfo.RegisterEntity("IrremovableObject", typeof(IIrremovableObject));
            typesInfo.RegisterEntity("UncreatableObject", typeof(IUncreatableObject));
            typesInfo.RegisterEntity("MemberLevelSecurityObject", typeof(IMemberLevelSecurityObject));
            typesInfo.RegisterEntity("MemberLevelReferencedObject1", typeof(IMemberLevelReferencedObject1));
            typesInfo.RegisterEntity("MemberLevelReferencedObject2", typeof(IMemberLevelReferencedObject2));
            typesInfo.RegisterEntity("ObjectLevelSecurityObject", typeof(IObjectLevelSecurityObject));
        }
        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ModelImageSourcesUpdater());
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
    }
    public class ModelImageSourcesUpdater : ModelNodesGeneratorUpdater<ImageSourceNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            string assemblyName = typeof(SecurityDemoModule).Assembly.GetName().Name;
            IModelAssemblyResourceImageSource customImages = (IModelAssemblyResourceImageSource)node[assemblyName];
            if(customImages == null) {
                customImages = node.AddNode<IModelAssemblyResourceImageSource>(assemblyName);
                customImages.Index = node.NodeCount - 1;
            }
            customImages.Folder = "Images";
        }
    }
}
