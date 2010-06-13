using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Filtering;
using eXpand.Xpo;
using eXpand.Xpo.Parser;
using System.ComponentModel;

namespace eXpand.ExpressApp.SystemModule
{

    public interface IModelListViewPropertyPathFilters
    {
        IModelPropertyPathFilters PropertyPathFilters { get; set; }
    }

    public interface IModelPropertyPathFilters : IModelNode, IModelList<IModelPropertyPathFilter>
    {
    }

    public interface IModelPropertyPathFilter : IModelNode
    {
        [Required, ModelPersistentName("ID")]
        string Id { get; set; }
        [Required]
        string PropertyPath { get; set; }
        string PropertyPathFilter { get; set; }
        [DataSourceProperty("Application.Views")]
        [Required, Category("Appearance")]
        IModelListView PropertyPathListViewId { get; set; }
    }

    public abstract class FilterByPropertyPathViewController : ViewController<ListView>, IModelExtender
    {
        private Dictionary<string, FiltersByCollectionWrapper> _filtersByPropertyPathWrappers;
        readonly SingleChoiceAction _filterSingleChoiceAction;

        public Dictionary<string, FiltersByCollectionWrapper> FiltersByPropertyPathWrappers
        {
            get { return _filtersByPropertyPathWrappers; }
        }

        protected FilterByPropertyPathViewController()
        {
            _filterSingleChoiceAction = new SingleChoiceAction(this, "_filterSingleChoiceAction",
                                                               PredefinedCategory.Search) {Caption = "Search By"};
            _filterSingleChoiceAction.Execute+=createFilterSingleChoiceAction_Execute;
            TargetViewNesting = Nesting.Root;
        }

        public SingleChoiceAction FilterSingleChoiceAction
        {
            get { return _filterSingleChoiceAction; }
        }

        protected override void OnActivated()
        {
            _filterSingleChoiceAction.Items.Clear();
            if (((IModelListViewPropertyPathFilters)View.Model).PropertyPathFilters == null)
                return;

            getFilterWrappers();

            if (HasFilters)
                checkIfAdditionalViewControlsModuleIsRegister();

            setUpFilterAction(HasFilters);
            ApplyFilterString();
            Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
        }

        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs)
        {
            ApplyFilterString();
        }

        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.TemplateViewChanged -= FrameOnTemplateViewChanged;
        }

        public bool HasFilters
        {
            get
            {
                return FiltersByPropertyPathWrappers.Count() > 0;
            }
        }

        private void setUpFilterAction(bool active)
        {
            _filterSingleChoiceAction.Active["PropertyPath is valid"] = active;
            foreach (var pair in _filtersByPropertyPathWrappers)
            {
                if (pair.Value.BinaryOperatorLastMemberClassType != null)
                {
                    var caption = CaptionHelper.GetClassCaption(pair.Value.BinaryOperatorLastMemberClassType.FullName);
                    _filterSingleChoiceAction.Items.Add(new ChoiceActionItem(caption, pair.Value));
                }
            }
        }

        private void checkIfAdditionalViewControlsModuleIsRegister()
        {
            bool found = false;
            for (int i = 0; i < View.Model.Application.NodeCount; i++) {
                IModelNode modelNode = View.Model.Application.GetNode(i);
                if (modelNode.GetValue<string>("Id") == "AdditionalViewControls") {
                    found = true;
                    break;
                }
            }

            if (!(found)){
                throw new UserFriendlyException(new Exception("AdditionalViewControlsProvider module not found"));
            }
        }

        private void getFilterWrappers()
        {
            _filtersByPropertyPathWrappers = new Dictionary<string, FiltersByCollectionWrapper>();
            foreach (IModelPropertyPathFilter childNode in ((IModelListViewPropertyPathFilters)View.Model).PropertyPathFilters)
                _filtersByPropertyPathWrappers.Add(childNode.Id,new FiltersByCollectionWrapper(View.ObjectTypeInfo, childNode,
                        ObjectSpace.Session.GetClassInfo(View.ObjectTypeInfo.Type)));
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewPropertyPathFilters>();
        }

        private void ApplyFilterString()
        {
            string text = _filtersByPropertyPathWrappers
                .Select(pair => ApplyFilterString(pair.Value))
                .Where(filterString => !string.IsNullOrEmpty(filterString))
                .Aggregate<string, string>(null, (current, filterString) => current + (filterString + Environment.NewLine));

            const string delimeter = ") AND (";
            text = (text + "").Replace(Environment.NewLine, delimeter);
            if (text.EndsWith(delimeter))
                text = text.Substring(0, text.Length - delimeter.Length);
            if (!string.IsNullOrEmpty(text))
                text = "(" + text + ")";

            var frameTemplate = Frame.Template as IViewSiteTemplate;
            if (frameTemplate != null && !string.IsNullOrEmpty(text)) AddFilterPanel(text, frameTemplate.ViewSiteControl);
        }

        protected abstract void AddFilterPanel(string text, object viewSiteControl);

        private string ApplyFilterString(FiltersByCollectionWrapper filtersByCollectionWrapper)
        {
            View.CollectionSource.Criteria[filtersByCollectionWrapper.ID] = null;
            CriteriaOperator criteriaOperator = SetCollectionSourceCriteria(filtersByCollectionWrapper);
            return GetHintPanelText(filtersByCollectionWrapper, criteriaOperator);
        }

        private string GetHintPanelText(FiltersByCollectionWrapper filtersByCollectionWrapper, CriteriaOperator criteriaOperator)
        {
            if (!(ReferenceEquals(criteriaOperator, null))){
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
            if (!(ReferenceEquals(criteriaOperator, null))){
                new FilterWithObjectsProcessor(View.ObjectSpace).Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
                var criterion = new PropertyPathParser(View.ObjectSpace.Session.GetClassInfo(View.ObjectTypeInfo.Type)).Parse(filtersByCollectionWrapper.PropertyPath, criteriaOperator.ToString());
                View.CollectionSource.Criteria[filtersByCollectionWrapper.ID] = criterion;
                return criteriaOperator;
            }
            return criteriaOperator;
        }

        private void DialogControllerOnAccepting(object sender, DialogControllerAcceptingEventArgs args)
        {
            View view = ((DialogController)sender).Frame.View;
            SynchronizeInfo(view);
        }

        protected virtual void SynchronizeInfo(View view)
        {
            throw new NotImplementedException();
        }

        private void AcceptFilter(FiltersByCollectionWrapper filtersByCollectionWrapper)
        {
            IModelListView nodeWrapper = GetNodeMemberSearchWrapper(filtersByCollectionWrapper);
            filtersByCollectionWrapper.PropertyPathFilter = GetActiveFilter(nodeWrapper);
            ApplyFilterString();
            View.Refresh();
        }

        protected abstract string GetActiveFilter(IModelListView modelListView);

        protected abstract void SetActiveFilter(IModelListView modelListView, string filter);

        private IModelListView GetNodeMemberSearchWrapper(FiltersByCollectionWrapper filtersByCollectionWrapper)
        {
            var nodeWrapper = Application.Model.Views.OfType<IModelListView>().Where(
                wrapper => wrapper.Id == filtersByCollectionWrapper.CriteriaPathListViewId).FirstOrDefault();
            if (nodeWrapper == null)
                throw new ArgumentNullException(filtersByCollectionWrapper.CriteriaPathListViewId + " not found");
            return nodeWrapper;
        }

        private void createFilterSingleChoiceAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e)
        {
            var filtersByCollectionWrapper = ((FiltersByCollectionWrapper)e.SelectedChoiceActionItem.Data);

            IModelListView memberSearchWrapper = GetNodeMemberSearchWrapper(filtersByCollectionWrapper);
            var objectSpace = Application.CreateObjectSpace();
            var classType = filtersByCollectionWrapper.BinaryOperatorLastMemberClassType;
            CollectionSourceBase newCollectionSource = !memberSearchWrapper.UseServerMode
                                                           ? new CollectionSource(objectSpace, classType, false)
                                                           : new CollectionSource(objectSpace, classType, true);

            SetActiveFilter(memberSearchWrapper, filtersByCollectionWrapper.PropertyPathFilter);

            ListView listView = Application.CreateListView(memberSearchWrapper.Id, newCollectionSource, false);

            e.ShowViewParameters.CreatedView = listView;
            e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            dialogController.AcceptAction.Execute += (sender1, e1) => AcceptFilter(filtersByCollectionWrapper);
            dialogController.Accepting += DialogControllerOnAccepting;
            e.ShowViewParameters.Controllers.Add(dialogController);

        }
    }

    public class FiltersByCollectionWrapper
    {
        private readonly ITypeInfo objectTypeInfo;
        private readonly IModelPropertyPathFilter propertyPathFilter;
        private readonly XPClassInfo classInfo;
        private string containsOperatorXpMemberInfoName;
        private Type binaryOperatorLastMemberClassType;

        public FiltersByCollectionWrapper(ITypeInfo objectTypeInfo, IModelPropertyPathFilter propertyPathFilter, XPClassInfo classInfo)
        {
            this.objectTypeInfo = objectTypeInfo;
            this.propertyPathFilter = propertyPathFilter;
            this.classInfo = classInfo;
        }

        public string PropertyPath
        {
            get { return propertyPathFilter.PropertyPath; }
        }

        public string ID
        {
            get { return propertyPathFilter.Id; }
        }
        public string PropertyPathFilter
        {
            get { return propertyPathFilter.PropertyPathFilter; }
            set { propertyPathFilter.PropertyPathFilter = value; }
        }

        public string CriteriaPathListViewId
        {
            get { return propertyPathFilter.PropertyPathListViewId.Id; }
        }

        public Type BinaryOperatorLastMemberClassType
        {
            get
            {
                if (binaryOperatorLastMemberClassType == null)
                {
                    var xpMemberInfo = ReflectorHelper.GetXpMemberInfo(classInfo, PropertyPath);
                    if (xpMemberInfo != null)
                        return binaryOperatorLastMemberClassType = xpMemberInfo.IsCollection ? xpMemberInfo.CollectionElementType.ClassType : xpMemberInfo.ReferenceType.ClassType;
                }

                return binaryOperatorLastMemberClassType;
            }

        }

        public string ContainsOperatorXpMemberInfoName
        {
            get
            {
                if (containsOperatorXpMemberInfoName == null && BinaryOperatorLastMemberClassType != null)
                {
                    containsOperatorXpMemberInfoName = PropertyPath.IndexOf(".") > -1
                                                           ? objectTypeInfo.FindMember(PropertyPath.Substring(0, PropertyPath.IndexOf("."))).Name
                                                           : PropertyPath;
                }

                return containsOperatorXpMemberInfoName;
            }
        }
    }
}