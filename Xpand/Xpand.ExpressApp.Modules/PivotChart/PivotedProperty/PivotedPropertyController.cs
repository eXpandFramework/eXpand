using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.PivotChart.PivotedProperty {
    public class PivotedPropertyController : ViewController<DetailView> {

        protected override void OnActivated() {
            base.OnActivated();
            var memberInfos = GetMemberInfos().ToArray();
            if (memberInfos.Any()) {
                AttachControllers();
            }

            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var memberInfos = GetMemberInfos();
            foreach (var memberInfo in memberInfos) {
                BindMember(memberInfo);
            }
            var analysisEditorBases = View.GetItems<ISupportValueReading>();
            foreach (var analysisEditorBase in analysisEditorBases) {
                analysisEditorBase.ValueReading += AnalysisEditorBaseOnValueReading;
            }
        }

        void AnalysisEditorBaseOnValueReading(object sender, EventArgs eventArgs) {
            var analysisEditorBase = ((AnalysisEditorBase)sender);
            if (analysisEditorBase.PropertyValue == null) {
                IMemberInfo memberInfo = analysisEditorBase.MemberInfo.GetPath().FirstOrDefault(info => info.Name != nameof(IAnalysisInfo.Self));
                if (memberInfo!=null)
                    BindMember(memberInfo);
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
        }
        void ViewOnCurrentObjectChanged(object sender, EventArgs eventArgs) {
            var memberInfos = GetMemberInfos();
            foreach (var memberInfo in memberInfos) {
                BindMember(memberInfo);
            }
        }

        IEnumerable<IMemberInfo> GetMemberInfos() {
            return View.ObjectTypeInfo.Members.Where(
                memberInfo => memberInfo.FindAttribute<PivotedPropertyAttribute>() != null).Select(info1 => info1);
        }

        void BindMember(IMemberInfo memberInfo) {
            var pivotedPropertyAttribute = memberInfo.FindAttribute<PivotedPropertyAttribute>();
            IAnalysisInfo analysisInfo;
            if (string.IsNullOrEmpty(pivotedPropertyAttribute.AnalysisCriteria)) {
                analysisInfo = (IAnalysisInfo)ObjectSpace.CreateObject(memberInfo.MemberType);
                var pivotedType = View.ObjectTypeInfo.FindMember(pivotedPropertyAttribute.CollectionName).ListElementType;
                ((XPObjectSpace)ObjectSpace).Session.GetClassInfo(analysisInfo).GetMember(analysisInfo.GetPropertyName(x => x.DataType)).SetValue(analysisInfo, pivotedType);
            } else {
                analysisInfo = ObjectSpace.FindObject(memberInfo.MemberType, CriteriaOperator.Parse(pivotedPropertyAttribute.AnalysisCriteria)) as IAnalysisInfo;
                if (analysisInfo == null)
                    throw new UserFriendlyException(new Exception("Could not find a " + memberInfo.MemberType.Name + " object that can fit " + pivotedPropertyAttribute.AnalysisCriteria));
            }
            memberInfo.SetValue(View.CurrentObject, analysisInfo);
        }


        protected virtual void AttachControllers() {
            var assignCustomAnalysisDataSourceDetailViewController = AttachAssignCustomAnalysisDataSourceDetailViewController();
            assignCustomAnalysisDataSourceDetailViewController.ApplyingCollectionCriteria += ApplyingCollectionCriteria;
            assignCustomAnalysisDataSourceDetailViewController.DatasourceCreating += DatasourceCreating;

            ActivateController<AnalysisDataBindController>();
        }

        void DatasourceCreating(object sender, AnalysisEditorArgs analysisEditorArgs) {
            analysisEditorArgs.DataSource = GetOrphanCollection(GetMemberInfo(analysisEditorArgs));
            analysisEditorArgs.Handled = analysisEditorArgs.DataSource != null;
        }

        void ApplyingCollectionCriteria(object sender, CriteriaOperatorArgs criteriaOperatorArgs) {
            criteriaOperatorArgs.Criteria = GetCriteria(GetMemberInfo(criteriaOperatorArgs));
        }

        IMemberInfo GetMemberInfo(AnalysisEditorArgs criteriaOperatorArgs) {
            var analysisEditorBase = criteriaOperatorArgs.AnalysisEditorBase;
            return analysisEditorBase.View.ObjectTypeInfo.FindMember(analysisEditorBase.MemberInfo.Name.Replace(".Self", ""));
        }

        void ActivateController<TController>() where TController : ViewController {
            Frame.GetController<TController>(controller => {
                controller.Active.Clear();
                controller.Active[""] = true;
                controller.TargetObjectType = View.ObjectTypeInfo.Type;
            });
        }

        protected virtual AssignCustomAnalysisDataSourceDetailViewController AttachAssignCustomAnalysisDataSourceDetailViewController() {

            var assignCustomAnalysisDataSourceDetailViewController = new AssignCustomAnalysisDataSourceDetailViewController {
                TargetObjectType = View.ObjectTypeInfo.Type
            };
            assignCustomAnalysisDataSourceDetailViewController.SetView(View);
            Frame.RegisterController(assignCustomAnalysisDataSourceDetailViewController);
            return assignCustomAnalysisDataSourceDetailViewController;
        }

        CriteriaOperator GetCriteria(IMemberInfo memberInfo) {
            IMemberInfo collectionMemberInfo = GetCollectionMemberInfo(memberInfo);
            return collectionMemberInfo?.AssociatedMemberInfo != null
                       ? CriteriaOperator.Parse(
                    $"{memberInfo.Owner.Name}.{collectionMemberInfo.ListElementTypeInfo.KeyMember.Name}=?",
                                                (Guid) ObjectSpace.GetKeyValue(View.CurrentObject))
                       : null;
        }

        IMemberInfo GetCollectionMemberInfo(IMemberInfo memberInfo) {
            var pivotedPropertyAttribute = memberInfo.FindAttribute<PivotedPropertyAttribute>();
            return View?.ObjectTypeInfo.FindMember(pivotedPropertyAttribute.CollectionName);
        }

        IEnumerable GetOrphanCollection(IMemberInfo memberInfo) {
            var collectionMemberInfo = GetCollectionMemberInfo(memberInfo);
            return collectionMemberInfo != null && collectionMemberInfo.AssociatedMemberInfo == null
                       ? (IEnumerable) collectionMemberInfo.GetValue(View.CurrentObject)
                       : null;
        }
    }
}
