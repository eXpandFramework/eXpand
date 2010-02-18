using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.SystemModule {
    public class ViewEditModeController : ViewController<DetailView> {
        public const string ViewEditModeAttributeName = "ViewEditMode";

        protected override void OnActivated() {
            base.OnActivated();
            View.ViewEditMode= View.Info.GetAttributeEnumValue(ViewEditModeAttributeName, ViewEditMode.View);
        }

        public override Schema GetSchema() {
            DictionaryNode injectAttribute = new SchemaHelper().InjectAttribute(ViewEditModeAttributeName,
                                                                                typeof (ViewEditMode),
                                                                                ModelElement.DetailView);
            return new Schema(injectAttribute);
        }
    }
}