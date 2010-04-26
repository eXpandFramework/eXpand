using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using System.Linq;

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
            _searchAbleMemberInfos = new DetailViewInfoNodeWrapper(View.Info).Editors.Items.Where(wrapper => wrapper.Node.GetAttributeEnumValue(SearchModeAttributeName, SearchMemberMode.Unknown) == SearchMemberMode.Include).Select(nodeWrapper => View.ObjectTypeInfo.FindMember(nodeWrapper.PropertyName));
            _searchAction.Active["HasSearchAbleMembers"] = _searchAbleMemberInfos.Count()>0;
        }
        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            GroupOperator groupOperator = GetCriteria();
            var count = (int)View.ObjectSpace.Session.Evaluate(View.ObjectTypeInfo.Type, CriteriaOperator.Parse("Count()"), groupOperator);
            var objects = ObjectSpace.GetObjects(View.ObjectTypeInfo.Type, groupOperator);
            CreateOrderProviderSource(objects);
            if (count > 0) {
                ChangeObject(objects[0]);
            }
        }

        void CreateOrderProviderSource(IList objects) {
            var standaloneOrderProvider = new StandaloneOrderProvider(ObjectSpace, objects);
            var orderProviderSource = new OrderProviderSource {OrderProvider = standaloneOrderProvider};
            Frame.GetController<RecordsNavigationController>().OrderProviderSource=orderProviderSource;
        }

        GroupOperator GetCriteria() {
            var memberInfos = new DetailViewInfoNodeWrapper(View.Info).Editors.Items.Where(wrapper => wrapper.Node.GetAttributeEnumValue(SearchModeAttributeName, SearchMemberMode.Unknown) == SearchMemberMode.Include).Select(nodeWrapper => View.ObjectTypeInfo.FindMember(nodeWrapper.PropertyName));
            var groupOperator = new GroupOperator(GroupOperatorType.Or);
            foreach (var memberInfo in memberInfos) {
                var value = memberInfo.GetValue(View.CurrentObject);
                if (value is string)
                    value = "%" + value + "%";
                groupOperator.Operands.Add(new BinaryOperator(memberInfo.Name,value,value is string?BinaryOperatorType.Like : BinaryOperatorType.Equal));
            }
            return groupOperator;
        }

        protected virtual void ChangeObject(object findObject) {
            View.CurrentObject = findObject;
        }

        protected override ModelElement GetModelElement() {
            return ModelElement.DetailViewPropertyEditors;
        }
    }
}