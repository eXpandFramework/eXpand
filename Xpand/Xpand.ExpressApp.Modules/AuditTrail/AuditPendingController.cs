using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.AuditTrail.BusinessObjects;
using Xpand.ExpressApp.AuditTrail.Logic;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.AuditTrail {
    [ModelAbstractClass]
    public interface IModelClassAuditPending:IModelClass{
        IModelAuditPending AuditPending { get; }
    }

    public interface IModelAuditPending:IModelNode{
        [DataSourceProperty("PendingMembers")]
        IModelMember PendingMember { get; }
        [Browsable(false)]
        IEnumerable<IModelMember> PendingMembers { get; }
        [DataSourceProperty("CreatorMembers")]
        IModelMember CreatorMember { get; }
        [Browsable(false)]
        IEnumerable<IModelMember> CreatorMembers { get; }
    }
    [DomainLogic(typeof(IModelAuditPending))]
    public class ModelClassPendingMemberDomainLogic{
        public static IEnumerable<IModelMember> Get_CreatorMembers(IModelAuditPending classPendingMember) {
            return ((IModelClass) classPendingMember.Parent).AllMembers.Where(member => typeof(SecuritySystemUserBase).IsAssignableFrom(member.Type));
        }

        public static IModelMember Get_CreatorMember(IModelAuditPending classPendingMember) {
            return Get_CreatorMembers(classPendingMember).FirstOrDefault(member => member.Name == "Creator");
        }
        public static IEnumerable<IModelMember> Get_PendingMembers(IModelAuditPending classPendingMember) {
            return ((IModelClass)classPendingMember.Parent).AllMembers.Where(member => member.Type == typeof(bool));
        }

        public static IModelMember Get_PendingMember(IModelAuditPending classPendingMember) {
            return Get_PendingMembers(classPendingMember).FirstOrDefault(member => member.Type == typeof (bool)&&member.Name=="Pending");
        }
    }
    public class AuditPendingController:ViewController,IModelExtender {
        public const string ApproveAudits = "ApproveAudits";
        private bool _auditPending;
        private readonly SimpleAction _approveAuditsAction;

        public AuditPendingController(){
            _approveAuditsAction = new SimpleAction(this,ApproveAudits,PredefinedCategory.ObjectsCreation);
            _approveAuditsAction.Execute+=ApproveAuditsActionOnExecute;
            _approveAuditsAction.TargetObjectType = typeof (IBaseAuditDataItemPersistent);
        }

        public SimpleAction ApproveAuditsAction{
            get { return _approveAuditsAction; }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            var logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            logicRuleViewController.LogicRuleExecutor.LogicRuleExecuted -= LogicRuleExecutorOnLogicRuleExecuted;
            ObjectSpace.CustomCommitChanges -= CustomCommitChanges;
        }

        protected override void OnActivated(){
            base.OnActivated();
            var logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            logicRuleViewController.LogicRuleExecutor.LogicRuleExecuted+=LogicRuleExecutorOnLogicRuleExecuted;
            ObjectSpace.CustomCommitChanges+=CustomCommitChanges;
            var listView = View as ListView;
            if (listView != null){
                var modelAuditPending = ((IModelClassAuditPending) listView.Model.ModelClass).AuditPending;
                var pendingMember = modelAuditPending.PendingMember;
                var creatorMember = modelAuditPending.CreatorMember;
                if (pendingMember != null && creatorMember!=null){
//                    var @operator =CriteriaOperator.Parse("(([" + creatorMember.Name + "." + creatorMember.ModelClass.KeyProperty +"] = CurrentUserId() And [" 
//                        + pendingMember.Name + "] = True) Or [" +pendingMember.Name + "] = False)");
//                    listView.CollectionSource.Criteria[typeof(AuditPendingController).Name] = @operator;
                }
            }
        }

        private void LogicRuleExecutorOnLogicRuleExecuted(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs){
            var logicRuleInfo = logicRuleExecuteEventArgs.LogicRuleInfo;
            var auditTrailRule = logicRuleInfo.Rule as IAuditTrailRule;
            if (auditTrailRule != null){
                if (auditTrailRule.AuditPending.HasValue){
                    _auditPending = auditTrailRule.AuditPending.Value;
                }
            }
        }

        private void CustomCommitChanges(object sender, HandledEventArgs handledEventArgs){
            _auditPending = false;
            Frame.GetController<LogicRuleViewController>().LogicRuleExecutor.Execute<IAuditTrailRule>(ExecutionContext.None, EventArgs.Empty, View);
            if (_auditPending){
                var objectSpace = ((IObjectSpace) sender);
                if (!objectSpace.IsNewObject(View.CurrentObject)) {
                    AuditTrailService.Instance.SaveAuditData(objectSpace.Session());
                    objectSpace.RollbackSilent();
                    const string template =
                        "Your changeds are audited and pending for approval. You will be notified by email if approved or not. The object is reset to its origial state";
                    throw new UserFriendlyException(template);
                }
                var auditPending = ((IModelClassAuditPending)View.Model.AsObjectView.ModelClass).AuditPending;
                auditPending.PendingMember.MemberInfo.SetValue(View.CurrentObject, true);
                var creator = objectSpace.GetObject(SecuritySystem.CurrentUser);
                auditPending.CreatorMember.MemberInfo.SetValue(View.CurrentObject, creator);
            }
        }

        private void ApproveAuditsActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            var auditDataItemPersistents = simpleActionExecuteEventArgs.SelectedObjects.Cast<XpandAuditDataItemPersistent>().Where(persistent => persistent.Pending).ToList();
            foreach (var auditDataItemPersistent in auditDataItemPersistents) {
                var weakReference = auditDataItemPersistent.AuditedObject;
                var typeInfo = Application.TypesInfo.FindTypeInfo(weakReference.TargetTypeName);
                var objectByKey = ObjectSpace.GetObjectByKey(typeInfo.Type, auditDataItemPersistent.AuditedObject.GuidId);
                typeInfo.FindMember(auditDataItemPersistent.PropertyName).SetValue(objectByKey, auditDataItemPersistent.NewValue);
                if (auditDataItemPersistent.OperationType == AuditOperationType.ObjectChanged.ToString() && !string.IsNullOrEmpty(auditDataItemPersistent.PropertyName)) {
                    typeInfo.FindMember(auditDataItemPersistent.PropertyName).SetValue(objectByKey,auditDataItemPersistent.NewValue);
                    auditDataItemPersistent.Pending = false;
                }
                else if (auditDataItemPersistent.OperationType == AuditOperationType.ObjectCreated.ToString()){
                    var member = typeInfo.FindMember(((IModelClassAuditPending) typeInfo.ModelClass()).AuditPending.PendingMember.Name);
                    member.SetValue(objectByKey, false);
                    auditDataItemPersistent.Pending = false;
                }
            }
            ObjectSpace.CommitChanges();
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass, IModelClassAuditPending>();
        }
    }
}
