using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Filtering;
using Xpand.Xpo;
using Xpand.Xpo.Parser;

namespace Xpand.ExpressApp.SystemModule {

    public interface IModelListViewPropertyPathFilters : IModelNode {
        IModelPropertyPathFilters PropertyPathFilters { get; }
    }
    [ModelNodesGenerator(typeof(ModelPropertyPathFiltersNodesGenerator))]
    public interface IModelPropertyPathFilters : IModelNode, IModelList<IModelPropertyPathFilter> {
    }

    public class ModelPropertyPathFiltersNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {

        }
    }
    public interface IModelPropertyPathFilter : IModelNode {
        [Required, ModelPersistentName("ID")]
        string Id { get; set; }
        [Required]
        string PropertyPath { get; set; }
        string PropertyPathFilter { get; set; }
        [DataSourceProperty("Application.Views")]
        [Required, Category("Appearance")]
        IModelListView PropertyPathListViewId { get; set; }
    }

    public abstract class FilterByPropertyPathViewController : ViewController<ListView> {
        private Dictionary<string, FiltersByCollectionWrapper> _filtersByPropertyPathWrappers;
        readonly SingleChoiceAction _filterSingleChoiceAction;

        public Dictionary<string, FiltersByCollectionWrapper> FiltersByPropertyPathWrappers {
            get { return _filtersByPropertyPathWrappers; }
        }

        protected FilterByPropertyPathViewController() {
            _filterSingleChoiceAction = new SingleChoiceAction(this, "_filterSingleChoiceAction",
                                                               PredefinedCategory.Search) { Caption = "Search By" };
            _filterSingleChoiceAction.Execute += createFilterSingleChoiceAction_Execute;
            _filterSingleChoiceAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            TargetViewNesting = Nesting.Root;
        }

        public SingleChoiceAction FilterSingleChoiceAction {
            get { return _filterSingleChoiceAction; }
        }

        protected override void OnActivated() {
            _filterSingleChoiceAction.Items.Clear();
            if (((IModelListViewPropertyPathFilters)View.Model).PropertyPathFilters == null)
                return;

            GetFilterWrappers();

            if (HasFilters)
                CheckIfAdditionalViewControlsModuleIsRegister();

            SetUpFilterAction(HasFilters);
            ApplyFilterString();
            Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
        }

        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs) {
            ApplyFilterString();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.TemplateViewChanged -= FrameOnTemplateViewChanged;
        }

        public bool HasFilters {
            get {
                return FiltersByPropertyPathWrappers.Any();
            }
        }

        private void SetUpFilterAction(bool active) {
            _filterSingleChoiceAction.Active["PropertyPath is valid"] = active;
            foreach (var pair in _filtersByPropertyPathWrappers) {
                if (pair.Value.BinaryOperatorLastMemberClassType != null) {
                    var caption = CaptionHelper.GetClassCaption(pair.Value.BinaryOperatorLastMemberClassType.FullName);
                    _filterSingleChoiceAction.Items.Add(new ChoiceActionItem(caption, pair.Value));
                }
            }
        }

        private void CheckIfAdditionalViewControlsModuleIsRegister() {
            bool found = false;
            for (int i = 0; i < View.Model.Application.NodeCount; i++) {
                IModelNode modelNode = View.Model.Application.GetNode(i);
                if (modelNode.GetValue<string>("Id") == "AdditionalViewControls") {
                    found = true;
                    break;
                }
            }

            if (!(found)) {
                throw new UserFriendlyException(new Exception("AdditionalViewControlsProvider module not found"));
            }
        }

        private void GetFilterWrappers() {
            _filtersByPropertyPathWrappers = new Dictionary<string, FiltersByCollectionWrapper>();
            foreach (IModelPropertyPathFilter childNode in ((IModelListViewPropertyPathFilters)View.Model).PropertyPathFilters)
                _filtersByPropertyPathWrappers.Add(childNode.Id, new FiltersByCollectionWrapper(View.ObjectTypeInfo, childNode,
                        ((XPObjectSpace)ObjectSpace).Session.GetClassInfo(View.ObjectTypeInfo.Type)));
        }


        private void ApplyFilterString() {
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
            if (frameTemplate != null && frameTemplate.ViewSiteControl != null) AddFilterPanel(text, frameTemplate.ViewSiteControl);
        }

        protected abstract void AddFilterPanel(string text, object viewSiteControl);

        private string ApplyFilterString(FiltersByCollectionWrapper filtersByCollectionWrapper) {
            View.CollectionSource.Criteria[filtersByCollectionWrapper.ID] = null;
            CriteriaOperator criteriaOperator = SetCollectionSourceCriteria(filtersByCollectionWrapper);
            return GetHintPanelText(filtersByCollectionWrapper, criteriaOperator);
        }

        private string GetHintPanelText(FiltersByCollectionWrapper filtersByCollectionWrapper, CriteriaOperator criteriaOperator) {
            if (!(ReferenceEquals(criteriaOperator, null))) {
                var wrapper = new LocalizedCriteriaWrapper(filtersByCollectionWrapper.BinaryOperatorLastMemberClassType, criteriaOperator);
                wrapper.UpdateParametersValues();
                CriteriaOperator userFriendlyFilter = CriteriaOperator.Clone(wrapper.CriteriaOperator);
                new FilterWithObjectsUserFrendlyStringProcessor(filtersByCollectionWrapper.BinaryOperatorLastMemberClassType).Process(userFriendlyFilter);
                return userFriendlyFilter.ToString();
            }
            return null;
        }

        private CriteriaOperator SetCollectionSourceCriteria(FiltersByCollectionWrapper filtersByCollectionWrapper) {
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(filtersByCollectionWrapper.PropertyPathFilter);
            if (!(ReferenceEquals(criteriaOperator, null))) {
                new FilterWithObjectsProcessor(View.ObjectSpace).Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
                var criterion = new PropertyPathParser(((XPObjectSpace)View.ObjectSpace).Session.GetClassInfo(View.ObjectTypeInfo.Type)).Parse(filtersByCollectionWrapper.PropertyPath, criteriaOperator.ToString());
                View.CollectionSource.Criteria[filtersByCollectionWrapper.ID] = criterion;
                return criteriaOperator;
            }
            return null;
        }

        private void DialogControllerOnAccepting(object sender, DialogControllerAcceptingEventArgs args) {
            View view = ((DialogController)sender).Frame.View;
            SynchronizeInfo(view);

        }

        protected virtual void SynchronizeInfo(View view) {
            throw new NotImplementedException();
        }

        private void AcceptFilter(FiltersByCollectionWrapper filtersByCollectionWrapper) {
            IModelListView nodeWrapper = GetNodeMemberSearchWrapper(filtersByCollectionWrapper);
            filtersByCollectionWrapper.PropertyPathFilter = GetActiveFilter(nodeWrapper);
            ApplyFilterString();
            View.Refresh();
        }

        protected abstract string GetActiveFilter(IModelListView modelListView);

        protected abstract void SetActiveFilter(IModelListView modelListView, string filter);

        private IModelListView GetNodeMemberSearchWrapper(FiltersByCollectionWrapper filtersByCollectionWrapper) {
            var nodeWrapper = Application.Model.Views.OfType<IModelListView>().FirstOrDefault(wrapper => wrapper.Id == filtersByCollectionWrapper.CriteriaPathListViewId);
            if (nodeWrapper == null)
                throw new ArgumentNullException(filtersByCollectionWrapper.CriteriaPathListViewId + " not found");
            return nodeWrapper;
        }

        private void createFilterSingleChoiceAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
            var filtersByCollectionWrapper = ((FiltersByCollectionWrapper)e.SelectedChoiceActionItem.Data);

            IModelListView memberSearchWrapper = GetNodeMemberSearchWrapper(filtersByCollectionWrapper);
            var classType = filtersByCollectionWrapper.BinaryOperatorLastMemberClassType;
            var objectSpace = Application.CreateObjectSpace(classType);
            CollectionSourceBase newCollectionSource = new CollectionSource(objectSpace, classType, memberSearchWrapper.UseServerMode);

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

    public class FiltersByCollectionWrapper {
        private readonly ITypeInfo objectTypeInfo;
        private readonly IModelPropertyPathFilter propertyPathFilter;
        private readonly XPClassInfo classInfo;
        private string containsOperatorXpMemberInfoName;
        private Type binaryOperatorLastMemberClassType;

        public FiltersByCollectionWrapper(ITypeInfo objectTypeInfo, IModelPropertyPathFilter propertyPathFilter, XPClassInfo classInfo) {
            this.objectTypeInfo = objectTypeInfo;
            this.propertyPathFilter = propertyPathFilter;
            this.classInfo = classInfo;
        }

        public string PropertyPath {
            get { return propertyPathFilter.PropertyPath; }
        }

        public string ID {
            get { return propertyPathFilter.Id; }
        }
        public string PropertyPathFilter {
            get { return propertyPathFilter.PropertyPathFilter; }
            set { propertyPathFilter.PropertyPathFilter = value; }
        }

        public string CriteriaPathListViewId {
            get { return propertyPathFilter.PropertyPathListViewId.Id; }
        }

        public Type BinaryOperatorLastMemberClassType {
            get {
                if (binaryOperatorLastMemberClassType == null) {
                    var xpMemberInfo = XpandReflectionHelper.GetXpMemberInfo(classInfo, PropertyPath);
                    if (xpMemberInfo != null)
                        return binaryOperatorLastMemberClassType = xpMemberInfo.IsCollection ? xpMemberInfo.CollectionElementType.ClassType : xpMemberInfo.ReferenceType.ClassType;
                }

                return binaryOperatorLastMemberClassType;
            }

        }

        public string ContainsOperatorXpMemberInfoName {
            get {
                if (containsOperatorXpMemberInfoName == null && BinaryOperatorLastMemberClassType != null) {
                    containsOperatorXpMemberInfoName = PropertyPath.IndexOf(".", StringComparison.Ordinal) > -1
                                                           ? objectTypeInfo.FindMember(PropertyPath.Substring(0, PropertyPath.IndexOf(".", StringComparison.Ordinal))).Name
                                                           : PropertyPath;
                }

                return containsOperatorXpMemberInfoName;
            }
        }
    }
}