using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.AuditTrail;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Utils;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.AuditTrail.BusinessObjects;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.AuditTrail.Model.Member;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Persistent.Base.Security;
using Xpand.XAF.Modules.ModelViewInheritance;

namespace Xpand.ExpressApp.AuditTrail {
    [ToolboxBitmap(typeof(AuditTrailModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandAuditTrailModule :XpandModuleBase,ISecurityModuleUser {
        public XpandAuditTrailModule() {
            AuditTrailService.Instance.CustomCreateObjectAuditProcessorsFactory += OnCustomCreateObjectAuditProcessorsFactory;
            RequiredModuleTypes.Add(typeof (AuditTrailModule));
            LogicInstallerManager.RegisterInstaller(new AuditTrailLogicInstaller(this));
            RequiredModuleTypes.Add(typeof(ModelViewInheritanceModule));
        }
        
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            this.AddSecurityObjectsToAdditionalExportedTypes("Xpand.Persistent.BaseImpl.AuditTrail");
            RuntimeMemberBuilder.CustomCreateMember+=RuntimeMemberBuilderOnCustomCreateMember;
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs eventArgs){
            AuditTrailService.Instance.SaveAuditTrailData += OnSaveAuditTrailData;
            AuditTrailService.Instance.AuditDataStore = new XpandAuditDataStore();
            
            var auditTrailModule = Application.FindModule<AuditTrailModule>();
            auditTrailModule.AuditDataItemPersistentType = typeof(XpandAuditDataItemPersistent);
        }

        private void OnCustomCreateObjectAuditProcessorsFactory(object sender, CustomCreateObjectAuditProcessorsFactoryEventArgs e){
            e.Factory=new XpandObjectAuditProcessorsFactory();
            e.Handled = true;
        }

        private void OnSaveAuditTrailData(object sender, SaveAuditTrailDataEventArgs saveAuditTrailDataEventArgs){
            if (Application.MainWindow == null && Application.Model != null)
                saveAuditTrailDataEventArgs.Handled =!((IModelApplicationAudiTrail) Application.Model).AudiTrail.AuditSystemChanges;
        }

        protected override void Dispose(bool disposing) {
            if (AuditTrailService.Instance != null) {
                AuditTrailService.Instance.SaveAuditTrailData -= OnSaveAuditTrailData;
                AuditTrailService.Instance.CustomCreateObjectAuditProcessorsFactory -= OnCustomCreateObjectAuditProcessorsFactory;
            }
            RuntimeMemberBuilder.CustomCreateMember -= RuntimeMemberBuilderOnCustomCreateMember;
            base.Dispose(disposing);
        }

        void RuntimeMemberBuilderOnCustomCreateMember(object sender, CustomCreateMemberArgs customCreateMemberArgs) {
            var modelMemberAuditTrail = customCreateMemberArgs.ModelMemberEx as IModelMemberAuditTrail;
            if (modelMemberAuditTrail != null) {
                XPClassInfo owner = modelMemberAuditTrail.ModelClass.TypeInfo.QueryXPClassInfo();
                if (owner.FindMember(modelMemberAuditTrail.Name)==null) {
                    new AuditTrailCollectionMemberInfo(owner, modelMemberAuditTrail.Name,modelMemberAuditTrail.CollectionType.TypeInfo.Type);
                }
                customCreateMemberArgs.Handled = true;
            }
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationAudiTrail>();
        }


        
    }
}