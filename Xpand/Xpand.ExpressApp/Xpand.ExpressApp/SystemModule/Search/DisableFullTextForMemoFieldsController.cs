using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.SystemModule.Search {
    public interface IModelClassDisableFullTextForMemoFields {
        [Category(SearchFromViewController.AttributesCategory)]
        [Description("Remove all fields marked with unlimited size attribute from full text")]
        bool DisableFullTextForMemoFields { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassDisableFullTextForMemoFields), "ModelClass")]
    public interface IModelListViewDisableFullTextForMemoFields : IModelClassDisableFullTextForMemoFields {
    }

    public class DisableFullTextForMemoFieldsController : ViewController<ListView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassDisableFullTextForMemoFields>();
            extenders.Add<IModelListView, IModelListViewDisableFullTextForMemoFields>();
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<FilterController>().CustomBuildCriteria += OnCustomBuildCriteria;
        }

        void OnCustomBuildCriteria(object sender, CustomBuildCriteriaEventArgs customBuildCriteriaEventArgs) {
            if (View != null && ((IModelListViewDisableFullTextForMemoFields)View.Model).DisableFullTextForMemoFields) {
                var filterController = Frame.GetController<FilterController>();
                var members = removeUnlimitedSizeMembers(filterController.GetFullTextSearchProperties(), View.ObjectTypeInfo);
                customBuildCriteriaEventArgs.Criteria = new SearchCriteriaBuilder(
                    View.ObjectTypeInfo,
                    members,
                    customBuildCriteriaEventArgs.SearchText, GroupOperatorType.Or, false).BuildCriteria();
                customBuildCriteriaEventArgs.Handled = true;
            }
        }


        private ICollection<string> removeUnlimitedSizeMembers(IEnumerable<string> properties, ITypeInfo typeInfo) {
            return (from property in properties
                    let attribute = typeInfo.FindMember(property).FindAttribute<SizeAttribute>()
                    where (attribute != null && attribute.Size != SizeAttribute.Unlimited) || attribute == null
                    select property).ToList();
        }
    }
}