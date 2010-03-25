using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Attributes;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.PivotChart
{
    public class PivotedPropertyController : ViewController<DetailView>
    {
        public PivotedPropertyController() {
            TargetObjectType = null;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            IEnumerable<IMemberInfo> memberInfos =View.ObjectTypeInfo.Members.OfType<IMemberInfo>().Where(
                    memberInfo => memberInfo.FindAttribute<PivotedPropertyAttribute>() != null).Select(info1 => info1);
            
            Active["HasPivotProperty"] = memberInfos.Count() > 0;
            if (Active) {
                AttachControllers(memberInfos);
                foreach (var memberInfo in memberInfos) {
                    BindMembers(memberInfo);
                }
            }
        }
        void BindMembers(IMemberInfo memberInfo) {
            var pivotedPropertyAttribute = memberInfo.FindAttribute<PivotedPropertyAttribute>();
            IAnalysisInfo analysisInfo;
            if (string.IsNullOrEmpty(pivotedPropertyAttribute.Criteria)) {
                analysisInfo = (IAnalysisInfo) ObjectSpace.CreateObject(memberInfo.MemberType);
                var pivotedType = View.ObjectTypeInfo.FindMember(pivotedPropertyAttribute.CollectionName).ListElementType;
                ObjectSpace.Session.GetClassInfo(analysisInfo).GetMember(analysisInfo.GetPropertyName(x=>x.DataType)).SetValue(analysisInfo,pivotedType);
            }
            else {
                analysisInfo = ObjectSpace.FindObject(memberInfo.MemberType, CriteriaOperator.Parse(pivotedPropertyAttribute.Criteria)) as IAnalysisInfo;
                if (analysisInfo== null)
                    throw new UserFriendlyException(new Exception("Could not find a "+memberInfo.MemberType.Name+" object that can fit "+pivotedPropertyAttribute.Criteria));
            }
            memberInfo.SetValue(View.CurrentObject,analysisInfo);
        }


        protected virtual void AttachControllers(IEnumerable<IMemberInfo> memberInfos) {
            var assignCustomAnalysisDataSourceDetailViewController = AttachAssignCustomAnalysisDataSourceDetailViewController();
            assignCustomAnalysisDataSourceDetailViewController.ApplyingCollectionCriteria+=ApplyingCollectionCriteria;
            assignCustomAnalysisDataSourceDetailViewController.DatasourceCreating+=DatasourceCreating;
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
            return View.ObjectTypeInfo.FindMember(criteriaOperatorArgs.AnalysisEditorBase.MemberInfo.Name.Replace(".Self",""));
        }

        void  ActivateController<TController>() where TController:ViewController{
            var dataBindController = Frame.GetController<TController>();
            dataBindController.Active.Clear();
            dataBindController.Active[""] = true;
            dataBindController.TargetObjectType = View.ObjectTypeInfo.Type;
        }

        protected virtual AssignCustomAnalysisDataSourceDetailViewController AttachAssignCustomAnalysisDataSourceDetailViewController() {

            var assignCustomAnalysisDataSourceDetailViewController = new AssignCustomAnalysisDataSourceDetailViewController {
                                                                         TargetObjectType =View.ObjectTypeInfo.Type
                                                                     };
            assignCustomAnalysisDataSourceDetailViewController.SetView(View);
            return assignCustomAnalysisDataSourceDetailViewController; 
        }

        CriteriaOperator GetCriteria(IMemberInfo memberInfo) {
            IMemberInfo collectionMemberInfo = GetCollectionMemberInfo(memberInfo);
            if (collectionMemberInfo.AssociatedMemberInfo!= null)
                return CriteriaOperator.Parse(string.Format("{0}.{1}=?", memberInfo.Owner.Name,
                                                            collectionMemberInfo.ListElementTypeInfo.KeyMember.Name),
                                              (Guid) ObjectSpace.GetKeyValue(View.CurrentObject));
            return null;
        }

        IMemberInfo GetCollectionMemberInfo(IMemberInfo memberInfo) {
            var pivotedPropertyAttribute = memberInfo.FindAttribute<PivotedPropertyAttribute>();
            return View.ObjectTypeInfo.FindMember(pivotedPropertyAttribute.CollectionName);
        }

        IEnumerable GetOrphanCollection(IMemberInfo memberInfo) {
            var collectionMemberInfo = GetCollectionMemberInfo(memberInfo);
            if (collectionMemberInfo.AssociatedMemberInfo== null)
                return (IEnumerable)collectionMemberInfo.GetValue(View.CurrentObject);
            return null;
        }
    }
}
