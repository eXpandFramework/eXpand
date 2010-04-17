using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class SearchFromDetailViewController:SearchFromViewController {
        IEnumerable<IMemberInfo> _searchAbleMemberInfos;
        readonly SimpleAction _searchAction;

        public IEnumerable<IMemberInfo> SearchAbleMemberInfos {
            get { return _searchAbleMemberInfos; }
        }

        protected SearchFromDetailViewController() {
            _searchAction = new SimpleAction(this,"Search",PredefinedCategory.Search);
            _searchAction.Execute+=SimpleActionOnExecute;
            TargetViewType=ViewType.DetailView;
        }

        public SimpleAction SearchAction {
            get { return _searchAction; }
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            _searchAbleMemberInfos = (this.View as DetailView).Model.Items.Cast<IModelViewPropertySearchMode>().Where(wrapper => wrapper.SearchMemberMode == SearchMemberMode.Include).Select(nodeWrapper => View.ObjectTypeInfo.FindMember(((IModelPropertyEditor)nodeWrapper).PropertyName));
            _searchAction.Active["HasSearchAbleMembers"] = _searchAbleMemberInfos.Count()>0;
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var memberInfos = (this.View as DetailView).Model.Items.Cast<IModelViewPropertySearchMode>().Where(wrapper => wrapper.SearchMemberMode == SearchMemberMode.Include).Select(nodeWrapper => View.ObjectTypeInfo.FindMember(((IModelPropertyEditor)nodeWrapper).PropertyName));
            var groupOperator = new GroupOperator(GroupOperatorType.Or);
            foreach (var memberInfo in memberInfos) {
                var value = memberInfo.GetValue(View.CurrentObject);
                if (value is string)
                    value = "%" + value + "%";
                groupOperator.Operands.Add(new BinaryOperator(memberInfo.Name,value,value is string?BinaryOperatorType.Like : BinaryOperatorType.Equal));
            }

            var findObject = View.ObjectSpace.FindObject(View.ObjectTypeInfo.Type, groupOperator);
            if (findObject != null) ChangeObject(findObject);
        }

        protected virtual void ChangeObject(object findObject) {
            View.CurrentObject = findObject;
        }

        public override void ExtendModelInterfaces(DevExpress.ExpressApp.Model.ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelPropertyEditor, IModelViewPropertySearchMode>();
        }
    }
}