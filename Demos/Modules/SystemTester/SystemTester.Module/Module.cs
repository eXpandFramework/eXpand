using System;
using System.Collections.Generic;
using System.Linq;
using SystemTester.Module.FunctionalTests.RuntimeMembers;
using SystemTester.Module.FunctionalTests.SequenceGenerator;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.BaseImpl;
using Xpand.Persistent.Base.General;
using Updater = SystemTester.Module.DatabaseUpdate.Updater;

namespace SystemTester.Module {
    public sealed partial class SystemTesterModule : EasyTestModule {
        public SystemTesterModule() {
            InitializeComponent();
            RequiredModuleTypes.Add(typeof(Xpand.XAF.Modules.CloneModelView.CloneModelViewModule));
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            if (application.GetEasyTestParameter("StoreModelInDB")){
                application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
                application.CreateCustomUserModelDifferenceStore += ApplicationOnCreateCustomUserModelDifferenceStore;
            }
        }

        private void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e){
            e.Handled = true;
            e.Store=new ModelDifferenceDbStore(Application, typeof(ModelDifference),false);
        }

        private void ApplicationOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs e){
            e.Handled = true;
            e.Store = new ModelDifferenceDbStore(Application, typeof(ModelDifference), true);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo){
            base.CustomizeTypesInfo(typesInfo);
            typesInfo.RegisterEntity("SequenceGeneratorDCObject",typeof(ISequenceGeneratorObject));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters){
            base.AddGeneratorUpdaters(updaters);
            if (Application != null && Application.GetEasyTestParameter("StoreModelInDB"))
                updaters.Add(new RuntimeMemberModelDifferenceNavigationItemUpdater());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new Updater(objectSpace, versionFromDB);
            return new[] { updater,new RuntimeMembersUpdater(objectSpace, Version)  };
        }
    }

    public class RuntimeMemberModelDifferenceNavigationItemUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
        public override void UpdateNode(ModelNode node){
            
            var modelNavigationItem = ((IModelRootNavigationItems)node).Items.First(item => item.Caption == "Default").Items.AddNode<IModelNavigationItem>();
            modelNavigationItem.View = node.Application.BOModel.GetClass(typeof(ModelDifference)).DefaultListView;
        }
    }
}
