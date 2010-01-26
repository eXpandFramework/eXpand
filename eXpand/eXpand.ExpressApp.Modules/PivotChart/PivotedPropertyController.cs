using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.PivotChart;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.PivotChart.ShowInAnalysis;

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
                    AttachControllers(memberInfo);
                }
            }
        }

        protected virtual void AttachControllers(IMemberInfo memberInfo) {
            AttachAssignCustomAnalysisDataSourceDetailViewController(memberInfo);
            AttachController<AnalysisDataBindController>();
        }

        protected void  AttachController<TController>() where TController:ViewController{
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
    }
}
