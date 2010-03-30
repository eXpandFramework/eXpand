using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class SearchFromViewController : ViewController {
        public const string SearchModeAttributeName = "SearchMode";

        public override Schema GetSchema() {
            var schemaHelper = new SchemaBuilder();
            DictionaryNode injectAttribute = schemaHelper.InjectAttribute(SearchModeAttributeName,typeof (SearchMemberMode),GetModelElement());
            return new Schema(injectAttribute);
        }

        protected abstract ModelElement GetModelElement();
    }
}