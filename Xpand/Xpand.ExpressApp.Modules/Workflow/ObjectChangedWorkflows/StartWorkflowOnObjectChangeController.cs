using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using System.Linq;
using DevExpress.Persistent.Base;


namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public class StartWorkflowOnObjectChangeController : ViewController<ObjectView> {
        readonly List<ObjectChangedEventArgs> _objectChangedEventArgses = new List<ObjectChangedEventArgs>();
        void CreateServerRequest(ObjectChangedEventArgs objectChangedEventArgs, ObjectChangedWorkflow objectChangedWorkflow, object targetObjectKey, ITypeInfo typeInfo) {
            var request = ObjectSpace.CreateObject<ObjectChangedXpoStartWorkflowRequest>();
            request.TargetWorkflowUniqueId = objectChangedWorkflow.GetUniqueId();
            request.TargetObjectType = typeInfo.Type;
            request.TargetObjectKey = targetObjectKey;
            request.PropertyName = objectChangedEventArgs.PropertyName;
            request.OldValue = GetOldValue(objectChangedEventArgs);
        }

        void InvokeOnClient(ObjectChangedEventArgs objectChangedEventArgs, ObjectChangedWorkflow objectChangedWorkflow, object targetObjectKey) {
            Activity activity = ActivityXamlServices.Load(new StringReader(objectChangedWorkflow.Xaml));
            var dictionary = ObjectChangedStartWorkflowService.Dictionary(targetObjectKey, objectChangedEventArgs.PropertyName, objectChangedEventArgs.OldValue);
            WorkflowInvoker.Invoke(activity, dictionary);
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (TypeHasWorkflows()) {
                ObjectSpace.ObjectChanged += PopulateObjectChangedEventArgs;
                ObjectSpace.Committing += StartWorkFlows;
            }
        }

        void StartWorkFlow(ObjectChangedEventArgs objectChangedEventArgs, ObjectChangedWorkflow objectChangedWorkflow) {
            var o = objectChangedEventArgs.Object;
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(o.GetType());
            object targetObjectKey = typeInfo.KeyMember.GetValue(o);
            if (objectChangedWorkflow.ExecutionDomain == ExecutionDomain.Server) {
                CreateServerRequest(objectChangedEventArgs, objectChangedWorkflow, targetObjectKey, typeInfo);
            } else {
                InvokeOnClient(objectChangedEventArgs, objectChangedWorkflow, targetObjectKey);
            }
        }

        void StartWorkFlows(object sender, CancelEventArgs cancelEventArgs) {
            var objectChangedWorkflows = GetObjectChangedWorkflows().Select(workflow => new{workflow, Args = GetObjectChangedEventArgs(workflow)}).Where(arg => arg.Args!=null).ToList();
            _objectChangedEventArgses.Clear();
            foreach (var objectChangedWorkflow in objectChangedWorkflows) {
                StartWorkFlow(objectChangedWorkflow.Args, objectChangedWorkflow.workflow);
            }   
        }

        ObjectChangedEventArgs GetObjectChangedEventArgs(ObjectChangedWorkflow objectChangedWorkflow) {
            ObjectChangedEventArgs objectChangedEventArgs = _objectChangedEventArgses.FirstOrDefault(args => args.PropertyName == objectChangedWorkflow.PropertyName && args.Object.GetType() == objectChangedWorkflow.TargetObjectType);
            return objectChangedEventArgs;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ObjectSpace.ObjectChanged -= PopulateObjectChangedEventArgs;
            ObjectSpace.Committing -= StartWorkFlows;
        }

        bool TypeHasWorkflows() {
            try {
                return ObjectSpace.GetObjectsCount(typeof(ObjectChangedWorkflow), CriteriaOperator.Parse("TargetObjectType=?", View.ObjectTypeInfo.Type)) > 0;
            } catch (Exception e) {
                Tracing.Tracer.LogError(e);
                return false;
            }
        }

        void PopulateObjectChangedEventArgs(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            if (!string.IsNullOrEmpty(objectChangedEventArgs.PropertyName)) {
                var changedEventArgs = _objectChangedEventArgses.FirstOrDefault(args => args.Object == objectChangedEventArgs.Object && args.PropertyName == objectChangedEventArgs.PropertyName);
                if (changedEventArgs != null) {
                    _objectChangedEventArgses.Remove(changedEventArgs);
                    _objectChangedEventArgses.Add(new ObjectChangedEventArgs(changedEventArgs.Object, changedEventArgs.PropertyName, changedEventArgs.OldValue, objectChangedEventArgs.NewValue));
                } else
                    _objectChangedEventArgses.Add(objectChangedEventArgs);
            }
        }

        object GetOldValue(ObjectChangedEventArgs objectChangedEventArgs) {
            if (objectChangedEventArgs.OldValue == null)
                return null;
            IMemberInfo memberInfo = XafTypesInfo.Instance.FindTypeInfo(objectChangedEventArgs.Object.GetType()).FindMember(objectChangedEventArgs.PropertyName);
            return memberInfo.MemberTypeInfo.IsPersistent ? memberInfo.MemberTypeInfo.KeyMember.GetValue(objectChangedEventArgs.OldValue) : objectChangedEventArgs.OldValue;
        }

        IEnumerable<ObjectChangedWorkflow> GetObjectChangedWorkflows() {
            var groupOperator = new GroupOperator(GroupOperatorType.Or);
            foreach (var objectChangedEventArgs in _objectChangedEventArgses) {
                groupOperator.Operands.Add(CriteriaOperator.Parse("TargetObjectType=?", objectChangedEventArgs.Object.GetType(), objectChangedEventArgs.PropertyName));
            }
            return ObjectSpace.GetObjects<ObjectChangedWorkflow>(groupOperator);
        }

    }
}
