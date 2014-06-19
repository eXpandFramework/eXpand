using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.AuditTrail.Model;
using Xpand.ExpressApp.Logic;
using System.Linq;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public class AuditTrailRuleViewController:ViewController {
        LogicRuleViewController _logicRuleViewController;
        private static bool _editingModel;
        private bool _auditSystemChanges;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            Frame.Disposing += FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute += LogicRuleExecutorOnLogicRuleExecute;
            if (Frame.Template == Application.MainWindow){
                AuditSystemChanges();
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committed -= ObjectSpaceOnCommitted;
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            ObjectSpace.Committed += ObjectSpaceOnCommitted;
        }

        protected override void OnActivated(){
            base.OnActivated();
            BeginSessionAudit();
        }

        private void BeginSessionAudit(){
            var handledEventArgs = new HandledEventArgs();
            _logicRuleViewController.LogicRuleExecutor.Execute<IAuditTrailRule>(ExecutionContext.None, View, handledEventArgs);
            if (handledEventArgs.Handled)
                AuditTrailService.Instance.BeginSessionAudit(ObjectSpace.Session(), AuditTrailStrategy.OnObjectLoaded);
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            BeginSessionAudit();
        }

        private void AuditSystemChanges(){
            var editModelAction =Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).FirstOrDefault(@base => @base.Id == "EditModel");
            if (editModelAction != null){
                editModelAction.Executing += (sender, args) =>{
                    AuditTrailService.Instance.SaveAuditTrailData += InstanceOnSaveAuditTrailData;
                    _editingModel = true;
                };
                editModelAction.ExecuteCompleted += (sender, args) =>{
                    AuditTrailService.Instance.SaveAuditTrailData -= InstanceOnSaveAuditTrailData;
                    _editingModel = false;
                };
                _auditSystemChanges = ((IModelApplicationAudiTrail) Application.Model).AudiTrail.AuditSystemChanges;
            }
        }

        private void InstanceOnSaveAuditTrailData(object sender, SaveAuditTrailDataEventArgs args){
            args.Handled = !_auditSystemChanges && _editingModel;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute -= LogicRuleExecutorOnLogicRuleExecute;
        }

        void LogicRuleExecutorOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            var logicRuleInfo = logicRuleExecuteEventArgs.LogicRuleInfo;
            var auditTrailRule = logicRuleInfo.Rule as IAuditTrailRule;
            if (auditTrailRule != null) {
                ApplyCustomization(auditTrailRule);
                var auditingOptionsEventArgs = logicRuleInfo.EventArgs as CustomizeSessionAuditingOptionsEventArgs;
                if (auditingOptionsEventArgs != null){
                    if (auditTrailRule.AuditingMode.HasValue)
                        auditingOptionsEventArgs.ObjectAuditingMode = (DevExpress.Persistent.AuditTrail.ObjectAuditingMode) auditTrailRule.AuditingMode.Value;
                    auditingOptionsEventArgs.AuditTrailStrategy = auditTrailRule.AuditTrailStrategy;
                }
                var handledEventArgs = logicRuleInfo.EventArgs as HandledEventArgs;
                if (handledEventArgs != null) handledEventArgs.Handled = true;
            }
        }

        void ApplyCustomization(IAuditTrailRule auditTrailRule) {
            var auditTrailService = AuditTrailService.Instance;
            if (auditTrailRule.AuditingMode.HasValue) {
                auditTrailService.ObjectAuditingMode = (DevExpress.Persistent.AuditTrail.ObjectAuditingMode) auditTrailRule.AuditingMode.Value;
            }
            
            var auditTrailSettings = auditTrailService.Settings;

            var memberNames = GetMembers((AuditTrailRule) auditTrailRule);
            if (memberNames.Any()) {
                auditTrailSettings.Clear();
                var names = memberNames.Select(info => info.Name).ToArray();
                auditTrailSettings.AddType(auditTrailRule.TypeInfo.Type, auditTrailRule.IncludeRelatedTypes, names);
                foreach (var source in auditTrailRule.TypeInfo.Descendants.Where(info => info.IsPersistent&&!info.IsAbstract)) {
                    auditTrailSettings.AddType(source.Type, auditTrailRule.IncludeRelatedTypes, names);
                }
            }
        }

        IEnumerable<XPMemberInfo> GetMembers(AuditTrailRule auditTrailRule) {
            var hasContext = !string.IsNullOrEmpty(auditTrailRule.MemberContext);
            if (!hasContext && auditTrailRule.AuditMemberStrategy!=AuditMemberStrategy.None) {
                return GetAllPublicProperties(auditTrailRule.TypeInfo.Type,info => auditTrailRule.AuditMemberStrategy == AuditMemberStrategy.AllMembers?info.Members:info.OwnMembers);
            }
            if (hasContext) {
                var auditTrailMembersContexts = ((IModelApplicationAudiTrail) Application.Model).AudiTrail.AuditTrailMembersContextGroup[auditTrailRule.MemberContext];
                var membersContexts =auditTrailMembersContexts.Where(context=>context.ModelClass.TypeInfo.IsAssignableFrom(auditTrailRule.TypeInfo));
                var classInfos =membersContexts.Select(context =>new{ClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(context.ModelClass.TypeInfo.Type),context.Members});
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

        void AddMember(XPMemberInfo memberInfo, List<XPMemberInfo> result) {
            Type memberType = memberInfo.MemberType;
            XPClassInfo memberClassInfo = XpandModuleBase.Dictiorary.QueryClassInfo(memberType);
            if (memberClassInfo != null && !memberClassInfo.IsPersistent) {
                return;
            }
            result.Add(memberInfo);
        }
    }
}
