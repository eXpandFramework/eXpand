using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.SystemModule
{
    public partial class FilterController : ViewController
    {
        public const string DisableFullTextForMemoFields = "DisableFullTextForMemoFields";

        public FilterController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        public override Schema GetSchema()
        {
            const string s =
                @"<Element Name=""Application"">;
                            <Element Name=""Views"">
                                <Element Name=""ListView"">;
                                    <Attribute Name=""" +DisableFullTextForMemoFields +@""" Choice=""False,True""/>
                                </Element>
                            </Element>
                    </Element>";
            return new Schema(new DictionaryXmlReader().ReadFromString(s));
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            FullTextSearchCriteriaBuilder.BuildCustomFullTextSearchCriteria +=
                FullTextSearchCriteriaBuilderOnBuildCustomFullTextSearchCriteria;
        }

        private void FullTextSearchCriteriaBuilderOnBuildCustomFullTextSearchCriteria(object sender,
                                                                                      BuildCustomSearchCriteriaEventArgs
                                                                                          args)
        {
            if (View != null && View.Info.GetAttributeBoolValue(DisableFullTextForMemoFields))
            {
                ICollection<string> properties = removeUnlimitedSizeMembers(FullTextSearchCriteriaBuilder.GetProperties(
                                                                                args.TypeInfo, args.AdditionalProperties),
                                                                            args.TypeInfo);
                args.Criteria = new SearchCriteriaOnAllPropertiesEngine(args.TypeInfo,
                                                                        properties,
                                                                        args.ValueToSearch, args.GroupOperatorType,
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