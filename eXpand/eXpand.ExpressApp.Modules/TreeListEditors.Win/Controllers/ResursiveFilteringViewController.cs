using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using eXpand.ExpressApp.Win.SystemModule;
using FilterControl=DevExpress.XtraEditors.FilterControl;
using LookupEdit=DevExpress.ExpressApp.Win.Editors.LookupEdit;

namespace eXpand.ExpressApp.TreeListEditors.Win.Controllers
{
    public partial class ResursiveFilteringViewController : ViewController
    {
        private static bool recursive;
        private static bool lookUpQueryPopUp;
        public ResursiveFilteringViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            RecursiveFilterPopLookUpTreeSelectionSimpleAction.Active["is " + typeof(ITreeNode).Name] = typeof(ITreeNode).IsAssignableFrom(View.ObjectTypeInfo.Type);
            if (!lookUpQueryPopUp)
                RecursiveFilterPopLookUpTreeSelectionSimpleAction.Active["Default"] = false;
            lookUpQueryPopUp = false;

            var filterControlListViewController = Frame.GetController<FilterControlListViewController>();
            filterControlListViewController.FilterActivated +=FilterControlListViewControllerOnFilterActivated;
        }

        private void FilterControlListViewControllerOnFilterActivated(object sender, EventArgs args)
        {
            var filterControlListViewController = Frame.GetController<FilterControlListViewController>();
            setRecursiveActionActiveState(filterControlListViewController.FilterControl);
            if (((ListView)View).Editor is CategorizedListEditor)
                filterControlListViewController.FilterControl.SourceControl = ((CategorizedListEditor)((ListView)View).Editor).Grid;
            filterControlListViewController.FilterControl.FilterChanged += FilterOnFilterChanged;

        }

        private List<ITreeNode> GetAllChildTreeNodes(InOperator inOperator, string propertyName)
        {
            var allTreeNodes = new List<ITreeNode>();
            XPCollection treeNodes = GetTreeNodes(propertyName, inOperator);
            if (treeNodes != null)
                allTreeNodes = treeNodes.Cast<ITreeNode>().ToList().GetAllTreeNodes();
            return allTreeNodes.Distinct().ToList();
        }
        private XPCollection GetTreeNodes(string propertyName, InOperator inOperator)
        {

            Type memberType = View.ObjectTypeInfo.FindMember(propertyName).MemberType;
            string keyName = XafTypesInfo.Instance.FindTypeInfo(memberType).KeyMember.Name;
            InOperator clone = inOperator.Clone();
            ((OperandProperty)clone.LeftOperand).PropertyName = keyName;
            return new XPCollection(ObjectSpace.Session, memberType, clone);
        }

        private void FilterOnFilterChanged(object sender, FilterChangedEventArgs args)
        {
            var clauseType = ((ClauseNode)(args.CurrentNode)).Operation;
            if (recursive && clauseType == ClauseType.AnyOf && args.Action == FilterChangedAction.ValueChanged)
            {
                recursive = false;
                string propertyName = ((ClauseNode)args.CurrentNode).FirstOperand.PropertyName;
                var inOperator = (InOperator)FilterControlHelpers.ToCriteria(args.CurrentNode);
                List<ITreeNode> allChildTreeNodes = GetAllChildTreeNodes(inOperator, propertyName);
                if (allChildTreeNodes.Count > 0)
                    ((FilterControl)sender).FilterCriteria = new InOperator(propertyName, allChildTreeNodes);
                RecursiveFilterPopLookUpTreeSelectionSimpleAction.Active[ClauseType.AnyOf.ToString()] = true;
            }
        }

        private void setRecursiveActionActiveState(ExpressApp.Win.Editors.FilterControl filter)
        {
            filter.EditorActivated +=
                edit =>
                {
                    if (edit is LookupEdit)
                        ((LookupEdit)edit).QueryPopUp +=
                            (o, args) =>
                            {
                                RecursiveFilterPopLookUpTreeSelectionSimpleAction.Active["Default"] =
                                    true;
                                lookUpQueryPopUp = true;
                            };
                };
        }

        private void RecursiveFilterPopLookUpTreeSelectionSimpleAction_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            recursive = true;
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.DoExecute();
        }

    }
}
