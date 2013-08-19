using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.AuditTrail;
using DevExpress.ExpressApp.Model;
using DevExpress.Utils;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.AuditTrail.Model.Member;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.RuntimeMembers;

namespace Xpand.ExpressApp.AuditTrail {
    [ToolboxBitmap(typeof(AuditTrailModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandAuditTrailModule :XpandModuleBase {
        public XpandAuditTrailModule() {
            RequiredModuleTypes.Add(typeof (AuditTrailModule));
            LogicInstallerManager.RegisterInstaller(new AuditTrailLogicInstaller(this));
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            RuntimeMemberBuilder.CustomCreateMember+=RuntimeMemberBuilderOnCustomCreateMember;
        }

        void RuntimeMemberBuilderOnCustomCreateMember(object sender, CustomCreateMemberArgs customCreateMemberArgs) {
            var modelMemberAuditTrail = customCreateMemberArgs.ModelMemberEx as IModelMemberAuditTrail;
            if (modelMemberAuditTrail != null) {
                RuntimeMemberBuilder.CustomCreateMember-=RuntimeMemberBuilderOnCustomCreateMember;
                XPClassInfo owner = Dictiorary.GetClassInfo(modelMemberAuditTrail.ModelClass.TypeInfo.Type);
                new AuditTrailCollectionMemberInfo(owner, modelMemberAuditTrail.Name, modelMemberAuditTrail.CollectionType.TypeInfo.Type);
                customCreateMemberArgs.Handled = true;
            }
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelApplication,IModelApplicationAudiTrail>();
        }

    }
}