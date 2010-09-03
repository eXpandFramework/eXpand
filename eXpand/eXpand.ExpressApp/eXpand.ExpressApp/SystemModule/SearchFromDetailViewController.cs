using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.SystemModule
{
    public class SearchFromDetailViewController : SearchFromViewController
    {
        IEnumerable<IMemberInfo> _searchAbleMemberInfos;
        readonly SimpleAction _searchAction;

        public IEnumerable<IMemberInfo> SearchAbleMemberInfos
        {
            get { return _searchAbleMemberInfos; }
        }

        public SearchFromDetailViewController()
        {
            _searchAction = new SimpleAction(this, "Search", PredefinedCategory.Search);
            _searchAction.Execute += SimpleActionOnExecute;
            TargetViewType = ViewType.DetailView;
        }

        public SimpleAction SearchAction
        {
            get { return _searchAction; }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var detailView = ((XpandDetailView)View);
            _searchAbleMemberInfos = detailView.Model.Items.OfType<IModelPropertyEditorSearchMode>().Where(
                mode => mode.SearchMemberMode == SearchMemberMode.Include).Select(searchMode => View.ObjectTypeInfo.FindMember(((IModelPropertyEditor)searchMode).ModelMember.Name));
            _searchAction.Active["HasSearchAbleMembers"] = _searchAbleMemberInfos.Count() > 0;
        }
        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs)
        {
            GroupOperator groupOperator = GetCriteria();
            var count = (int)View.ObjectSpace.Session.Evaluate(View.ObjectTypeInfo.Type, CriteriaOperator.Parse("Count()"), groupOperator);
            var objects = ObjectSpace.GetObjects(View.ObjectTypeInfo.Type, groupOperator);
            CreateOrderProviderSource(objects);
            if (count > 0)
            {
                ChangeObject(objects[0]);
            }
        }

        void CreateOrderProviderSource(IList objects)
        {
            var standaloneOrderProvider = new StandaloneOrderProvider(ObjectSpace, objects);
            var orderProviderSource = new OrderProviderSource { OrderProvider = standaloneOrderProvider };
            Frame.GetController<RecordsNavigationController>().OrderProviderSource = orderProviderSource;
        }

        GroupOperator GetCriteria()
        {
            var groupOperator = new GroupOperator(GroupOperatorType.Or);
            foreach (var memberInfo in _searchAbleMemberInfos)
            {
                var value = memberInfo.GetValue(View.CurrentObject);
                if (value is string)
                    value = "%" + value + "%";
                groupOperator.Operands.Add(new BinaryOperator(memberInfo.Name, value, value is string ? BinaryOperatorType.Like : BinaryOperatorType.Equal));
            }
            return groupOperator;
        }

        protected virtual void ChangeObject(object findObject)
        {
            View.CurrentObject = findObject;
        }

    }
}