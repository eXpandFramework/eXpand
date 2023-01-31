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
using DevExpress.ExpressApp.DC;
using Xpand.Extensions.XAF.Xpo;
using Xpand.Persistent.Base.AuditTrail;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail.Logic {
    public class AuditTrailRuleViewController:ViewController {
        private static bool _editingModel;
        private bool _auditSystemChanges;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.GetController<LogicRuleViewController>(controller => controller.LogicRuleExecutor.LogicRuleExecute += LogicRuleExecutorOnLogicRuleExecute);
            Frame.Disposing += FrameOnDisposing;
            
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
            Frame.GetController<LogicRuleViewController>(
                controller => controller.LogicRuleExecutor.Execute<IAuditTrailRule>(ExecutionContext.None, View,
                    handledEventArgs));
            
            if (handledEventArgs.Handled)
                AuditTrailService.GetService(Site).BeginSessionAudit(ObjectSpace.Session(), AuditTrailStrategy.OnObjectLoaded);
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            BeginSessionAudit();
        }

        private void AuditSystemChanges(){
            var editModelAction =Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).FirstOrDefault(@base => @base.Id == "EditModel");
            if (editModelAction != null){
                editModelAction.Executing += (_, _) =>{
                    AuditTrailService.GetService(Site).SaveAuditTrailData += InstanceOnSaveAuditTrailData;
                    _editingModel = true;
                };
                editModelAction.ExecuteCompleted += (_, _) =>{
                    AuditTrailService.GetService(Site).SaveAuditTrailData -= InstanceOnSaveAuditTrailData;
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
            Frame.GetController<LogicRuleViewController>(controller => controller.LogicRuleExecutor.LogicRuleExecute -= LogicRuleExecutorOnLogicRuleExecute);
        }

        void LogicRuleExecutorOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs e) {
            var logicRuleInfo = e.LogicRuleInfo;
            if (logicRuleInfo.Rule is IAuditTrailRule auditTrailRule&&logicRuleInfo.Active) {
                ApplyCustomization(auditTrailRule);
                if (logicRuleInfo.EventArgs is CustomizeSessionAuditingOptionsEventArgs auditingOptionsEventArgs){
                    if (auditTrailRule.AuditingMode.HasValue)
                        auditingOptionsEventArgs.ObjectAuditingMode = (DevExpress.Persistent.AuditTrail.ObjectAuditingMode) auditTrailRule.AuditingMode.Value;
                    auditingOptionsEventArgs.AuditTrailStrategy = auditTrailRule.AuditTrailStrategy;
                }

                if (logicRuleInfo.EventArgs is HandledEventArgs args) args.Handled = true;
            }
        }

        void ApplyCustomization(IAuditTrailRule auditTrailRule) {
            var auditTrailService = AuditTrailService.GetService(Site);
            if (auditTrailRule.AuditingMode.HasValue) {
                auditTrailService.ObjectAuditingMode = (DevExpress.Persistent.AuditTrail.ObjectAuditingMode) auditTrailRule.AuditingMode.Value;
            }
            
            var auditTrailSettings = auditTrailService.Settings;

            var memberNames = GetMembers((AuditTrailRule) auditTrailRule).ToArray();
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
                return GetAllPublicProperties(auditTrailRule.TypeInfo,info => auditTrailRule.AuditMemberStrategy == AuditMemberStrategy.AllMembers?info.Members:info.OwnMembers);
            }
            if (hasContext) {
                var auditTrailMembersContexts = ((IModelApplicationAudiTrail) Application.Model).AudiTrail.AuditTrailMembersContextGroup[auditTrailRule.MemberContext];
                var membersContexts =auditTrailMembersContexts.Where(context=>context.ModelClass.TypeInfo.IsAssignableFrom(auditTrailRule.TypeInfo));
                var classInfos =membersContexts.Select(context =>new{ClassInfo = context.ModelClass.TypeInfo.QueryXPClassInfo(), context.Members});
                return classInfos.SelectMany(arg => arg.Members.Select(member => arg.ClassInfo.FindMember(member.ModelMemberName)));
            }
            return Enumerable.Empty<XPMemberInfo>();
        }
        private bool IsCollection(XPMemberInfo memberInfo) {
            return memberInfo.IsCollection || memberInfo.IsAssociationList;
        }

        IEnumerable<XPMemberInfo> GetAllPublicProperties(ITypeInfo typeInfo,Func<XPClassInfo,ICollection<XPMemberInfo>> func ) {
            var result = new List<XPMemberInfo>();
            var currentClassInfo = typeInfo.QueryXPClassInfo();
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
            var memberClassInfo = XafTypesInfo.CastTypeToTypeInfo(memberInfo.MemberType).QueryXPClassInfo();
            if (memberClassInfo is { IsPersistent: false }) {
                return;
            }
            result.Add(memberInfo);
        }
    }
}
