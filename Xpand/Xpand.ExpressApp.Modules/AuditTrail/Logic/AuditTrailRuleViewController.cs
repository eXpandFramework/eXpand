using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.Logic;
using System.Linq;
using Xpand.Persistent.Base.General;

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
            auditTrailService.Settings.Clear();
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
            
            var memberNames = GetMembers((AuditTrailRule) auditTrailRule);
            if (memberNames.Any()) {
                auditTrailSettings.Clear();
                var names = memberNames.Select(info => info.Name).ToArray();
                auditTrailSettings.AddType(auditTrailRule.TypeInfo.Type, auditTrailRule.IncludeRelatedTypes, names);
            }
        }

        IEnumerable<XPMemberInfo> GetMembers(AuditTrailRule auditTrailRule) {
            var hasContext = !string.IsNullOrEmpty(auditTrailRule.MemberContext);
            if (!hasContext && auditTrailRule.AuditMemberStrategy!=AuditMemberStrategy.None) {
                return GetAllPublicProperties(auditTrailRule.TypeInfo.Type,info => auditTrailRule.AuditMemberStrategy == AuditMemberStrategy.AllMembers?info.Members:info.OwnMembers);
            }
            if (hasContext) {
                var membersContexts =((IModelApplicationAudiTrail) Application.Model).AudiTrail.AuditTrailMembersContextGroup[
                        auditTrailRule.MemberContext].Where(context=>context.ModelClass.TypeInfo.IsAssignableFrom(auditTrailRule.TypeInfo));
                var classInfos =membersContexts.Select(context =>
                        new{ClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(context.ModelClass.TypeInfo.Type),context.Members});
                return classInfos.SelectMany(arg => arg.Members.Select(member => arg.ClassInfo.FindMember(member.ModelMemberName)));
            }
            return Enumerable.Empty<XPMemberInfo>();
        }
        private bool IsCollection(XPMemberInfo memberInfo) {
            return memberInfo.IsCollection || memberInfo.IsAssociationList;
        }

        IEnumerable<XPMemberInfo> GetAllPublicProperties(Type type,Func<XPClassInfo,ICollection<XPMemberInfo>> func ) {
            var result = new List<XPMemberInfo>();
            XPClassInfo currentClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(type);
            while (currentClassInfo != null && currentClassInfo.ClassType.Assembly != typeof (XPObject).Assembly) {
                foreach (XPMemberInfo memberInfo in func.Invoke(currentClassInfo)) {
                    if (memberInfo.IsPublic && (IsCollection(memberInfo) || !memberInfo.IsReadOnly)) {
                        AddMember(memberInfo, result);
                    }
                }
                currentClassInfo = currentClassInfo.BaseClass;
            }
            return result;
        }

        static void AddMember(XPMemberInfo memberInfo, List<XPMemberInfo> result) {
            Type memberType = memberInfo.MemberType;
            XPClassInfo memberClassInfo = XpandModuleBase.Dictiorary.QueryClassInfo(memberType);
            if (memberClassInfo != null && !memberClassInfo.IsPersistent) {
                return;
            }
            result.Add(memberInfo);
        }
    }

}
