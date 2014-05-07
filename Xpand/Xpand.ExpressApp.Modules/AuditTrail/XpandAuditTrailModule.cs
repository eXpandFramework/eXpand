using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Data.PLinq.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.AuditTrail;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.ExpressApp.AuditTrail.BusinessObjects;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.AuditTrail.Model.Member;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.AuditTrail {
    [ToolboxBitmap(typeof(AuditTrailModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandAuditTrailModule :XpandModuleBase {
        private bool _loggedOn;

        public XpandAuditTrailModule() {
            RequiredModuleTypes.Add(typeof (AuditTrailModule));
            LogicInstallerManager.RegisterInstaller(new AuditTrailLogicInstaller(this));
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            RuntimeMemberBuilder.CustomCreateMember+=RuntimeMemberBuilderOnCustomCreateMember;
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            AuditTrailService.Instance.SaveAuditTrailData+=OnSaveAuditTrailData;
            AuditTrailService.Instance.CustomCreateObjectAuditProcessorsFactory+=OnCustomCreateObjectAuditProcessorsFactory;
            var auditTrailModule = application.FindModule<AuditTrailModule>();
            application.Disposed += application_Disposed;
            application.LoggedOff+=ApplicationOnLoggedOff;
            auditTrailModule.AuditDataItemPersistentType=typeof(XpandAuditDataItemPersistent);
            application.SetupComplete+=ApplicationOnSetupComplete;
            application.LoggingOff += ApplicationOnLoggingOff;
            
        }

        private void OnCustomCreateObjectAuditProcessorsFactory(object sender, CustomCreateObjectAuditProcessorsFactoryEventArgs e){
            e.Factory=new XpandObjectAuditProcessorsFactory();
            e.Handled = true;
        }

        private void application_Disposed(object sender, EventArgs e) {
            ((XafApplication) sender).Disposed -= application_Disposed;
            if (AuditTrailService.Instance != null){
                AuditTrailService.Instance.SaveAuditTrailData -= OnSaveAuditTrailData;
                AuditTrailService.Instance.CustomCreateObjectAuditProcessorsFactory += OnCustomCreateObjectAuditProcessorsFactory;
            }
            var application = sender as XafApplication;
            if (application != null) {
                application.LoggedOn -= XafApplicationOnLoggedOn;
                application.LoggingOff-=ApplicationOnLoggingOff;
            }
        }

        private void ApplicationOnLoggedOff(object sender, EventArgs eventArgs){
            _loggedOn = false;
        }

        private void OnSaveAuditTrailData(object sender, SaveAuditTrailDataEventArgs saveAuditTrailDataEventArgs){
            if (Application.MainWindow == null && Application.Model != null)
                saveAuditTrailDataEventArgs.Handled =!((IModelApplicationAudiTrail) Application.Model).AudiTrail.AuditSystemChanges;
        }

        private void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs){
            _loggedOn = false;
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs eventArgs){
            var xafApplication = ((XafApplication)sender);
            xafApplication.LoggedOn+=XafApplicationOnLoggedOn;
        }

        private void XafApplicationOnLoggedOn(object sender, LogonEventArgs logonEventArgs){
            _loggedOn = true;
        }


        protected override void Dispose(bool disposing) {
            RuntimeMemberBuilder.CustomCreateMember -= RuntimeMemberBuilderOnCustomCreateMember;
            base.Dispose(disposing);
        }


        void RuntimeMemberBuilderOnCustomCreateMember(object sender, CustomCreateMemberArgs customCreateMemberArgs) {
            var modelMemberAuditTrail = customCreateMemberArgs.ModelMemberEx as IModelMemberAuditTrail;
            if (modelMemberAuditTrail != null) {
                XPClassInfo owner = Dictiorary.GetClassInfo(modelMemberAuditTrail.ModelClass.TypeInfo.Type);
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
    public class XpandObjectAuditProcessorsFactory : ObjectAuditProcessorsFactory {
        public override bool IsSuitableAuditProcessor(ObjectAuditProcessor processor, ObjectAuditingMode mode){
            var isNoAuditMode = mode == (ObjectAuditingMode) Logic.ObjectAuditingMode.None;
            if (isNoAuditMode){
                return processor is NoAuditProccesor;
            }
            return base.IsSuitableAuditProcessor(processor, mode);
        }

        public override ObjectAuditProcessor CreateAuditProcessor(ObjectAuditingMode mode, Session session, AuditTrailSettings settings){
            var auditTrailSettings = new AuditTrailSettings();
            auditTrailSettings.SetXPDictionary(XpandModuleBase.Dictiorary);
            foreach (var auditTrailClassInfo in settings.TypesToAudit){
                var auditTrailMemberInfos = auditTrailClassInfo.Properties;
                auditTrailSettings.AddType(auditTrailClassInfo.ClassInfo.ClassType,auditTrailMemberInfos.Select(info => info.Name).ToArray());
            }
            return mode == (ObjectAuditingMode) Logic.ObjectAuditingMode.None ? new NoAuditProccesor(session, auditTrailSettings) : base.CreateAuditProcessor(mode, session, auditTrailSettings);
        }
    }

    public class NoAuditProccesor : ObjectAuditProcessor {
        public NoAuditProccesor(Session session, AuditTrailSettings settings)
            : base(session, settings) {
        }

        public override bool IsObjectAudited(object obj) {
            return false;
        }
    }
}