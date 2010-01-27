using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.PivotChart.ShowInAnalysis;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.PivotChart
{
    public partial class PivotedPropertyController : ViewController<DetailView>
    {
        public PivotedPropertyController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            IEnumerable<IMemberInfo> memberInfos =View.ObjectTypeInfo.Members.OfType<IMemberInfo>().Where(
                    memberInfo => memberInfo.FindAttribute<PivotedPropertyAttribute>() != null).Select(info1 => info1);
            Active["HasPivotProperty"] = memberInfos.Count() > 0;
            if (Active) {
                foreach (var memberInfo in memberInfos) {
                    BindMembers(memberInfo);
                    AttachControllers(memberInfo);
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


        protected virtual void AttachControllers(IMemberInfo memberInfo) {
            AttachAssignCustomAnalysisDataSourceDetailViewController(memberInfo);
            ActivateController<AnalysisDataBindController>();
        }

        void  ActivateController<TController>() where TController:ViewController{
            var dataBindController = Frame.GetController<TController>();
            dataBindController.Active.Clear();
            dataBindController.Active[""] = true;
            dataBindController.TargetObjectType = View.ObjectTypeInfo.Type;
        }

        void AttachAssignCustomAnalysisDataSourceDetailViewController(IMemberInfo memberInfo) {
            var pivotedPropertyAttribute = memberInfo.FindAttribute<PivotedPropertyAttribute>();
            IMemberInfo collectionMemberInfo = View.ObjectTypeInfo.FindMember(pivotedPropertyAttribute.CollectionName);
            IMemberInfo associatedMemberInfo = collectionMemberInfo.AssociatedMemberInfo;
            CriteriaOperator criteria =CriteriaOperator.Parse(string.Format("{0}.{1}=?", associatedMemberInfo.Name,
                                                                            associatedMemberInfo.Owner.KeyMember.Name),(Guid) ObjectSpace.GetKeyValue(View.CurrentObject));
            var assignCustomAnalysisDataSourceDetailViewController = new AssignCustomAnalysisDataSourceDetailViewController(criteria)
                                                                     {
                                                                         TargetObjectType =View.ObjectTypeInfo.Type
                                                                     };
            assignCustomAnalysisDataSourceDetailViewController.SetView(View);
        }
//        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
//        {
//            base.CustomizeTypesInfo(typesInfo);
//            var memberInfos = typesInfo.PersistentTypes.SelectMany(typeInfo => typeInfo.OwnMembers).Where(memberInfo => memberInfo.FindAttribute<PivotedPropertyAttribute>()!=null);
//            foreach (var memberInfo in memberInfos) {
//                memberInfo.AddAttribute(new ExpandObjectMembersAttribute(ExpandObjectMembers.InDetailView));
//            }
//        }
    }
}
