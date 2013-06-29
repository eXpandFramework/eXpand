using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider {
    [ToolboxBitmap(typeof(AdditionalViewControlsModule))]
    [ToolboxItem(false)]
    public sealed class AdditionalViewControlsModule : LogicModuleBase<IAdditionalViewControlsRule, AdditionalViewControlsRule,IModelAdditionalViewControlsRule,IModelApplicationAdditionalViewControls,IModelLogicAdditionalViewControls>{
        #region IModelExtender Members

        public override void CustomizeTypesInfo(DevExpress.ExpressApp.DC.ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<PessimisticLockingMessageAttribute>() != null);
            foreach (var typeInfo in typeInfos) {
                var memberInfo = typeInfo.FindMember("LockedUserMessage");
                if (memberInfo == null) {
                    var xpClassInfo = Dictiorary.GetClassInfo(typeInfo.Type);
                    var lockedUserMessageXpMemberInfo = new LockedUserMessageXpMemberInfo(xpClassInfo);
                    lockedUserMessageXpMemberInfo.AddAttribute(new BrowsableAttribute(false));
                    XafTypesInfo.Instance.RefreshInfo(typeInfo);
                }
            }
        }

        public override List<ExecutionContext> ExecutionContexts {
            get { return new List<ExecutionContext>{ ExecutionContext.ViewChanged }; }
        }

        public override LogicRulesNodeUpdater<IAdditionalViewControlsRule, IModelAdditionalViewControlsRule, IModelApplicationAdditionalViewControls> LogicRulesNodeUpdater {
            get { return new AdditionalViewControlsRulesNodeUpdater(); }
        }
        #endregion
        public override IModelLogicAdditionalViewControls GetModelLogic(IModelApplicationAdditionalViewControls modelApplicationAdditionalViewControls) {
            return modelApplicationAdditionalViewControls.AdditionalViewControls;
        }
    }
}
