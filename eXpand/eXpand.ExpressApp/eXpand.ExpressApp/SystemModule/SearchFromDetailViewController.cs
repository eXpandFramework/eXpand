using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule {
    public class SearchFromDetailViewController:ViewController<DetailView> {
        public override Schema GetSchema() {
            var schemaHelper = new SchemaBuilder();
            var injectAttribute = schemaHelper.InjectAttribute("SearchMode",typeof(SearchMemberMode),ModelElement.DetailView);
            return new Schema(injectAttribute);
        }
    }
}