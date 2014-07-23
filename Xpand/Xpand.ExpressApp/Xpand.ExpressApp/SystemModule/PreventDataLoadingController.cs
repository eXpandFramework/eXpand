using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Fasterflect;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelClassPreventDataLoading : IModelNode {
        [Category("eXpand")]
        [Description("Listview records are loaded only when a filter is present")]
        [DefaultValue(PreventLoadingData.Never)]
        PreventLoadingData PreventLoadingData { get; set; }
    }

    public enum PreventLoadingData{
        Never,
        SearchAndFiltersAndCriteriaEmpty,
        Always,
        [Description("Will prevent loading data when the IModelListView Filter property is empty. This means no data will be loaded if the user hasn't search or filtered using the FilterEditor or AutofilterRow. Even if a filter is selected from the filters dropdown data still won't be loaded without the user searching or filtering data")]
        SearchAndFiltersEmpty,
        [Description("Will prevent loading data when the a filter condition isn't applied using the Filters dropdown and user hasn't filtered using the FilterEditor or AutofilterRow and user hasn't searched. This means searching or filtering in any way including selecting a filter with a filter criteria from the filters dropdown will allow data to be loaded into the list.")]
        SearchAndFilterAndCurrentFilterCriteriaEmpty ,
        Criteria,

    }

    [ModelInterfaceImplementor(typeof(IModelClassPreventDataLoading), "ModelClass")]
    public interface IModelListViewPreventLoadingData : IModelClassPreventDataLoading {
    }

    public abstract class PreventDataLoadingController : ViewController<ListView> {
        private FilterController _filterController;
        protected const string PreventDataLoadingKey = "PreventLoadingData";

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (IsReady()) {
                PreventDataLoading();
                _filterController = Frame.GetController<FilterController>();
                _filterController.FullTextFilterAction.Execute += FullTextFilterAction_Execute;
            }
        }

        protected void PreventDataLoading(CriteriaOperator criteriaOperator){
            if (ReferenceEquals(criteriaOperator, null)) {
                PreventDataLoading();
            }
            else {
                ClearPreventLoadingDataCriteria();
            }
        }

        protected void PreventDataLoading(){
            View.CollectionSource.Criteria[PreventDataLoadingKey] = GetPreventLoadingDataCriteria();
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (IsReady())
                _filterController.FullTextFilterAction.Execute-=FullTextFilterAction_Execute;
            
        }

        protected bool IsReady() {
            var modelListViewGridViewOptions = ((IModelListViewPreventLoadingData)View.Model);
            if (modelListViewGridViewOptions.PreventLoadingData==PreventLoadingData.SearchAndFiltersAndCriteriaEmpty)
                return !FiltersAreEmpty() || !string.IsNullOrEmpty(View.Model.Criteria);
            if (modelListViewGridViewOptions.PreventLoadingData==PreventLoadingData.SearchAndFiltersEmpty)
                return !FiltersAreEmpty();
            if (modelListViewGridViewOptions.PreventLoadingData==PreventLoadingData.Criteria)
                return !string.IsNullOrEmpty(View.Model.Criteria);
            return modelListViewGridViewOptions.PreventLoadingData==PreventLoadingData.Always;
        }

        private bool FiltersAreEmpty(){
            return (string.IsNullOrEmpty(View.Model.Filter))&& ((IModelListViewFilter) View.Model).Filters.Any();
        }

        void FullTextFilterAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
            if (string.IsNullOrEmpty(e.ParameterCurrentValue as string))
                View.CollectionSource.Criteria[PreventDataLoadingKey] = GetPreventLoadingDataCriteria();
            else
                ClearPreventLoadingDataCriteria();
        }

        private BinaryOperator GetPreventLoadingDataCriteria() {
            var memberInfo = View.ObjectTypeInfo.KeyMember;
            var memberType = memberInfo.MemberType;
            var o = memberType.IsValueType ? memberType.CreateInstance() : null;
            return new BinaryOperator(memberInfo.Name, o);
        }

        protected void ClearPreventLoadingDataCriteria() {
            if (!ReferenceEquals(View.CollectionSource.Criteria[PreventDataLoadingKey], null))
                View.CollectionSource.Criteria[PreventDataLoadingKey] = null;
        }

    }
}
