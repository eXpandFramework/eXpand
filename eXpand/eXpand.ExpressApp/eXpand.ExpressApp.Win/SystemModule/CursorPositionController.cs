using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using System.Linq;
using DevExpress.XtraEditors;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public class CursorPositionController : ViewController<DetailView>{
        public const string CursorPosition = "CursorPosition";
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            IEnumerable<PropertyEditor> textEdits = GetTextEdits();

            foreach (var textEdit in textEdits){
                CursorPosition cursorPosition = textEdit.Info.GetAttributeEnumValue(CursorPosition,SystemModule.CursorPosition.Default);
                var edit = ((TextEdit) textEdit.Control);
                edit.GotFocus+=(sender, args) => {
                    var length = cursorPosition == SystemModule.CursorPosition.End ? (edit.EditValue + "").Length : 0;
                    edit.Select(length, length);
                };
            }
        }

        IEnumerable<PropertyEditor> GetTextEdits() {
            return View.GetItems<PropertyEditor>().Where(
                editor =>
                editor.Control is TextEdit &&
                editor.Info.GetAttributeEnumValue(CursorPosition, SystemModule.CursorPosition.Default) !=
                SystemModule.CursorPosition.Default);
        }


        public override Schema GetSchema(){
            var schemaBuilder = new SchemaBuilder();
            string injectString = @"<Attribute Name=""" + CursorPosition + @""" Choice=""{" + typeof(CursorPosition).FullName + @"}""   DefaultValueExpr=""{DevExpress.ExpressApp.Core.DictionaryHelpers.BOPropertyCalculator}ClassName=..\..\@ClassName""/>";
            var schema = new Schema(schemaBuilder.Inject(injectString, ModelElement.DetailViewPropertyEditors));
            schema.CombineWith(new Schema(schemaBuilder.Inject(@"<Attribute Name=""" + CursorPosition + @""" Choice=""{"+typeof(CursorPosition).FullName + @"}"" />", ModelElement.Member)));
            return schema;
        }
    }

    public enum CursorPosition {
        Default,
        Start,
        End
    }
}
