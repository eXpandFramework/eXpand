using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.SystemModule;
using eXpand.Persistent.BaseImpl;
using eXpand.Xpo;
using eXpand.Xpo.Parser;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public partial class FilterByPropertyPathViewController : BaseViewController
    {
        private DictionaryNode filterByCollectionNode;
        public const string PropertyPath = "PropertyPath";
        public const string PropertyPathFilters = "PropertyPathFilters";
        public const string Filter = "Filter";
        public const string PropertyPathFilter = "PropertyPathFilter";
        public const string PropertyPathListViewId = "PropertyPathListViewId";
        private Dictionary<string, FiltersByCollectionWrapper> _filtersByPropertyPathWrappers;

        public Dictionary<string, FiltersByCollectionWrapper> FiltersByPropertyPathWrappers{
            get { return _filtersByPropertyPathWrappers; }
        }

        public FilterByPropertyPathViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Root;
            
        }

        public SingleChoiceAction FilterSingleChoiceAction{
            get { return _filterSingleChoiceAction; }
        }

        protected override void OnActivated()
        {
            if (View.Info.FindChildNode(PropertyPathFilters) == null)
                return;

            getFilterWrappers();
            _filterSingleChoiceAction.Items.Clear();
            if (HasFilters)
                checkIfAdditionalViewControlsModuleIsRegister();
            setUpFilterAction(HasFilters);
            ApplyFilterString();
        }

        public bool HasFilters{
            get{
                return FiltersByPropertyPathWrappers.Count() > 0;
            }
        }

        private void setUpFilterAction(bool active){
            _filterSingleChoiceAction.Active[PropertyPath+" is valid"] = active;
            foreach (var pair in _filtersByPropertyPathWrappers)
            {
                var caption = CaptionHelper.GetClassCaption(
                    pair.Value.BinaryOperatorLastMemberClassType.FullName);
                _filterSingleChoiceAction.Items.Add(new ChoiceActionItem(caption,pair.Value));
            }
        }

        private void checkIfAdditionalViewControlsModuleIsRegister(){
            ClassInfoNodeWrapper wrapper = GetClassInfoNodeWrapper();
            DictionaryNode node = wrapper.Node.FindChildNode("AdditionalViewControls");
            if (node== null){
                throw new UserFriendlyException(new Exception("AdditionalViewControlsProvider module not found"));
            }
        }

        private void getFilterWrappers(){
            filterByCollectionNode = View.Info.GetChildNode(PropertyPathFilters);
            _filtersByPropertyPathWrappers = new Dictionary<string, FiltersByCollectionWrapper>();

            foreach (DictionaryNode childNode in filterByCollectionNode.ChildNodes)
                _filtersByPropertyPathWrappers.Add(childNode.GetAttributeValue("ID"),
                                                new FiltersByCollectionWrapper(View.ObjectTypeInfo, childNode,
                                                                               ObjectSpace.Session.GetClassInfo(
                                                                                   View.ObjectTypeInfo.Type)));
        }


        public override Schema GetSchema()
        {
            string CommonTypeInfos = @"<Element Name=""Application"">
                    <Element Name=""Views"" >
                        <Element Name=""ListView"" >
                            <Element Name=""" + PropertyPathFilters + @""">
                                <Element Name=""" + Filter + @""" Multiple=""True"" KeyAttribute=""ID"">
                                    <Attribute Name=""ID"" Required=""True"" />
                                    <Attribute Name=""" + PropertyPath + @"""  Required=""True""/>
                                    <Attribute IsInvisible=""True"" Name=""" + PropertyPathFilter + @"""/>
                                    <Attribute Name=""" + PropertyPathListViewId + @"""  Required=""True"" RefNodeName=""{" + typeof(ViewIdRefNodeProvider).FullName + @"};ViewType=All|ListView"" />
                                </Element>
                            </Element>
                        </Element>
                    </Element>
                </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(CommonTypeInfos));
        }


        private void ApplyFilterString(){
            

            string text = null;
            foreach (var pair in _filtersByPropertyPathWrappers)
            {
                var filterString = ApplyFilterString(pair.Value);
                if (!string.IsNullOrEmpty(filterString))
                    text += filterString + Environment.NewLine;
            }
            const string delimeter = ") AND (";
            text = (text + "").Replace(Environment.NewLine, delimeter);
            if (text.EndsWith(delimeter))
                text = text.Substring(0, text.Length - delimeter.Length);
            if (!string.IsNullOrEmpty(text) )
                text = "(" + text + ")";
            
            ClassInfoNodeWrapper wrapper = GetClassInfoNodeWrapper();
            DictionaryNode dictionaryNode = wrapper.Node.FindChildNode("AdditionalViewControls");
            dictionaryNode.SetAttribute("Message",text);
        }

        private string ApplyFilterString(FiltersByCollectionWrapper filtersByCollectionWrapper)
        {
            ((ListView)View).CollectionSource.Criteria[filtersByCollectionWrapper.ID] = null;
            CriteriaOperator criteriaOperator = SetCollectionSourceCriteria(filtersByCollectionWrapper);
            return GetHintPanelText(filtersByCollectionWrapper, criteriaOperator);
        }

        private string GetHintPanelText(FiltersByCollectionWrapper filtersByCollectionWrapper, CriteriaOperator criteriaOperator)
        {
            if (criteriaOperator != null)
            {
                var wrapper = new LocalizedCriteriaWrapper(filtersByCollectionWrapper.BinaryOperatorLastMemberClassType, criteriaOperator);
                wrapper.UpdateParametersValues();
                CriteriaOperator userFriendlyFilter = CriteriaOperator.Clone(wrapper.CriteriaOperator);
                new FilterWithObjectsUserFrendlyStringProcessor(filtersByCollectionWrapper.BinaryOperatorLastMemberClassType).Process(userFriendlyFilter);
                return userFriendlyFilter.ToString();
            }
            return null;
        }

        private CriteriaOperator SetCollectionSourceCriteria(FiltersByCollectionWrapper filtersByCollectionWrapper)
        {
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(filtersByCollectionWrapper.PropertyPathFilter);
            if (criteriaOperator!= null)
            {
                new FilterWithObjectsProcessor(View.ObjectSpace).Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
                ((ListView) View).CollectionSource.Criteria[filtersByCollectionWrapper.ID] =new PropertyPathParser(View.ObjectSpace.Session.GetClassInfo(View.ObjectTypeInfo.Type)).Parse(filtersByCollectionWrapper.PropertyPath, criteriaOperator.ToString());
                    new ContainsOperator(filtersByCollectionWrapper.ContainsOperatorXpMemberInfoName,criteriaOperator);
                return criteriaOperator;
            }
            return criteriaOperator;
        }

        


        private void DialogControllerOnAccepting(object sender, DialogControllerAcceptingEventArgs args)
        {
            View view = ((DialogController) sender).Frame.View;
            view.SynchronizeInfo();
        }





        private void AcceptFilter(FiltersByCollectionWrapper filtersByCollectionWrapper)
        {
            ListViewInfoNodeWrapper nodeWrapper = GetNodeMemberSearchWrapper(filtersByCollectionWrapper);


            string attributeValue = nodeWrapper.Node.GetAttributeValue(GridListEditor.ActiveFilterString);
            filtersByCollectionWrapper.PropertyPathFilter = attributeValue;
            ApplyFilterString();
            View.Refresh();
        }

        private ListViewInfoNodeWrapper GetNodeMemberSearchWrapper(FiltersByCollectionWrapper filtersByCollectionWrapper)
        {
            var nodeWrapper =
                (ListViewInfoNodeWrapper)
                (new ApplicationNodeWrapper(Application.Info).Views.Items.Where(
                    wrapper => wrapper.Id == filtersByCollectionWrapper.CriteriaPathListViewId)).
                    FirstOrDefault();
            if (nodeWrapper== null)
                throw new ArgumentNullException(filtersByCollectionWrapper.CriteriaPathListViewId+" not found");
            return nodeWrapper;
        }

        private void createFilterSingleChoiceAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var filtersByCollectionWrapper = ((FiltersByCollectionWrapper) e.SelectedChoiceActionItem.Data);
            
            ListViewInfoNodeWrapper memberSearchWrapper = GetNodeMemberSearchWrapper(filtersByCollectionWrapper);
            var objectSpace = Application.CreateObjectSpace();
            var classType = filtersByCollectionWrapper.BinaryOperatorLastMemberClassType;
            CollectionSourceBase newCollectionSource = !memberSearchWrapper.UseServerMode
                                                           ? (CollectionSourceBase)
                                                             new CollectionSource(objectSpace, classType)
                                                           : new ServerCollectionSource(objectSpace, classType);

            memberSearchWrapper.Node.SetAttribute(GridListEditor.ActiveFilterString, filtersByCollectionWrapper.PropertyPathFilter);

            ListView listView = Application.CreateListView(memberSearchWrapper.Id,newCollectionSource,false);

            e.ShowViewParameters.CreatedView = listView;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute += (sender1, e1) => AcceptFilter(filtersByCollectionWrapper);
            dialogController.Accepting += DialogControllerOnAccepting;
            e.ShowViewParameters.Controllers.Add(dialogController);

        }

        public class FiltersByCollectionWrapper
        {
            private readonly ITypeInfo objectTypeInfo;
            private readonly DictionaryNode childNode;
            private readonly XPClassInfo xpClassInfo;
            private string containsOperatorXpMemberInfoName;
            private Type binaryOperatorLastMemberClassType;

            public FiltersByCollectionWrapper(ITypeInfo objectTypeInfo, DictionaryNode childNode,XPClassInfo xpClassInfo)
            {
                this.objectTypeInfo = objectTypeInfo;
                this.childNode = childNode;
                this.xpClassInfo = xpClassInfo;
            }

            public string PropertyPath
            {
                get { return childNode.GetAttributeValue(FilterByPropertyPathViewController.PropertyPath); }
                set { childNode.SetAttribute(FilterByPropertyPathViewController.PropertyPath, value); }
            }

            public string ID
            {
                get { return childNode.GetAttributeValue("ID"); }
            }
            public string PropertyPathFilter
            {
                get { return childNode.GetAttributeValue(FilterByPropertyPathViewController.PropertyPathFilter); }
                set { childNode.SetAttribute(FilterByPropertyPathViewController.PropertyPathFilter, value); }
            }


            public string CriteriaPathListViewId
            {
                get { return childNode.GetAttributeValue(PropertyPathListViewId); }
                set { childNode.SetAttribute(PropertyPathListViewId, value); }
            }

            
            public Type BinaryOperatorLastMemberClassType
            {
                get
                {
                    if (binaryOperatorLastMemberClassType== null)
                    {
                        var xpMemberInfo = ReflectorHelper.GetXpMemberInfo(xpClassInfo,
                                                                           PropertyPath);
                        binaryOperatorLastMemberClassType =xpMemberInfo.IsCollection?xpMemberInfo.CollectionElementType.ClassType: xpMemberInfo.ReferenceType.ClassType;
                    }
                    return binaryOperatorLastMemberClassType;
                }
                
            }

            public string ContainsOperatorXpMemberInfoName
            {
                get
                {
                    if (containsOperatorXpMemberInfoName == null && BinaryOperatorLastMemberClassType != null){
                        containsOperatorXpMemberInfoName = PropertyPath.IndexOf(".") > -1
                                                               ?objectTypeInfo.FindMember(PropertyPath.Substring(0,PropertyPath.IndexOf("."))).Name
                                                               : PropertyPath;
                    }
                    return containsOperatorXpMemberInfoName;
                }
            }
        }

    }
}