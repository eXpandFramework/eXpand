using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public interface IModelListViewDisableFullTextForMemoFields
    {
        bool DisableFullTextForMemoFields { get; set; }
    }

    public partial class FilterController : ViewController<ListView>
    {
        public FilterController() { }

        public override void ExtendModelInterfaces(DevExpress.ExpressApp.Model.ModelInterfaceExtenders extenders)
        {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelListView, IModelListViewDisableFullTextForMemoFields>();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            FullTextSearchCriteriaBuilder.BuildCustomFullTextSearchCriteria +=
                FullTextSearchCriteriaBuilderOnBuildCustomFullTextSearchCriteria;
        }

        private void FullTextSearchCriteriaBuilderOnBuildCustomFullTextSearchCriteria(object sender, BuildCustomSearchCriteriaEventArgs args)
        {
            if (View != null && ((IModelListViewDisableFullTextForMemoFields)View.Model).DisableFullTextForMemoFields)
            {
                ICollection<string> properties = removeUnlimitedSizeMembers(
                    FullTextSearchCriteriaBuilder.GetProperties(
                    args.TypeInfo, args.AdditionalProperties),
                    args.TypeInfo);

                args.Criteria = new SearchCriteriaBuilder(
                    args.TypeInfo,
                    properties,
                    args.ValueToSearch, 
                    args.GroupOperatorType,
                    args.IncludeNonPersistentMembers).BuildCriteria();
            }
        }

        private ICollection<string> removeUnlimitedSizeMembers(IEnumerable<string> properties, ITypeInfo typeInfo)
        {
            return (from property in properties
                    let attribute = typeInfo.FindMember(property).FindAttribute<SizeAttribute>()
                    where (attribute != null && attribute.Size != SizeAttribute.Unlimited) || attribute == null
                    select property).ToList();
        }
    }
}