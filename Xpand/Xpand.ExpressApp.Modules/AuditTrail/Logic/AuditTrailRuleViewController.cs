using System;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.AuditTrail;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.Logic;
using System.Linq;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public class AuditTrailRuleViewController:ViewController {
        LogicRuleViewController _logicRuleViewController;
        ObjectAuditingMode _oldObjectAuditingMode;
        ReadOnlyCollection<AuditTrailClassInfo> _oldTypesToAudit;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing+=FrameOnDisposing;
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute+=LogicRuleExecutorOnLogicRuleExecute;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute -= LogicRuleExecutorOnLogicRuleExecute;
        }

        void LogicRuleExecutorOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            var logicRuleInfo = logicRuleExecuteEventArgs.LogicRuleInfo;
            var auditTrailRule = logicRuleInfo.Rule as IAuditTrailRule;
            if (auditTrailRule != null) {
                if (!logicRuleInfo.InvertCustomization) {
                    ApplyCustomization(auditTrailRule);
                } else {
                    InvertCustomization(auditTrailRule);
                }
            }
        }

        void InvertCustomization(IAuditTrailRule auditTrailRule) {
            var auditTrailService = AuditTrailService.Instance;
            if (auditTrailRule.AuditingMode.HasValue)
                auditTrailService.ObjectAuditingMode = _oldObjectAuditingMode;
            foreach (var info in _oldTypesToAudit) {
                auditTrailService.Settings.AddType(info.ClassInfo.ClassType,info.Properties.Select(memberInfo => memberInfo.Name).ToArray());    
            }
        }

        void ApplyCustomization(IAuditTrailRule auditTrailRule) {
            var auditTrailService = AuditTrailService.Instance;
            if (auditTrailRule.AuditingMode.HasValue) {
                _oldObjectAuditingMode = auditTrailService.ObjectAuditingMode;
                auditTrailService.ObjectAuditingMode = auditTrailRule.AuditingMode.Value;
            }
            var auditTrailSettings = auditTrailService.Settings;
            _oldTypesToAudit = auditTrailSettings.TypesToAudit;
            
            auditTrailSettings.Clear();
            var memberNames = GetMemberNames(auditTrailRule);
            auditTrailSettings.AddType(auditTrailRule.TypeInfo.Type,auditTrailRule.IncludeRelatedTypes,memberNames);
        }

        string[] GetMemberNames(IAuditTrailRule auditTrailRule) {
            var modelAuditTrailRule = (IModelAuditTrailRule) LogicInstallerManager.Instance[typeof (IAuditTrailRule)].GetModelLogic().Rules.First(rule => rule.Id == auditTrailRule.Id);
            if (string.IsNullOrEmpty(modelAuditTrailRule.AuditTrailMembersContext))
                return auditTrailRule.AuditAllMembers
                       ? auditTrailRule.TypeInfo.Members.Select(info => info.Name).ToArray()
                       : auditTrailRule.TypeInfo.OwnMembers.Select(info => info.Name).ToArray();
            var membersContexts = ((IModelApplicationAudiTrail) Application.Model).AudiTrail.AuditTrailMembersContextGroup[modelAuditTrailRule.AuditTrailMembersContext];
            return membersContexts.Where(context => context.ModelClass.TypeInfo.IsAssignableFrom(auditTrailRule.TypeInfo)).SelectMany(context 
                => context.Members).Select(member => member.ModelMemberName).Distinct().ToArray();
        }
    }

}
