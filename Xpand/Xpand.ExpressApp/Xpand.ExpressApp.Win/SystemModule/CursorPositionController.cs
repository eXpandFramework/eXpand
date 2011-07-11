using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelMemberCursorPosition {
        [Category("eXpand")]
        [Description("Controls the position of the cursor after the control gets focused")]
        CursorPosition CursorPosition { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberCursorPosition), "ModelMember")]
    public interface IModelPropertyEditorCursorPosition : IModelMemberCursorPosition {
    }
    public class CursorPositionController : ViewController<DetailView>, IModelExtender {
        public const string CursorPosition = "CursorPosition";

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember, IModelMemberCursorPosition>();
            extenders.Add<IModelPropertyEditor, IModelPropertyEditorCursorPosition>();
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            IEnumerable<PropertyEditor> textEdits = GetTextEdits();

            foreach (var textEdit in textEdits) {
                CursorPosition cursorPosition = ((IModelPropertyEditorCursorPosition)textEdit.Model).CursorPosition;
                var edit = ((TextEdit)textEdit.Control);
                edit.GotFocus += (sender, args) => {
                    var length = cursorPosition == SystemModule.CursorPosition.End ? (edit.EditValue + "").Length : 0;
                    edit.Select(length, length);
                };
            }
        }

        IEnumerable<PropertyEditor> GetTextEdits() {
            return View.GetItems<PropertyEditor>().Where(
                editor =>
                editor.Control is TextEdit &&
                ((IModelPropertyEditorCursorPosition)editor.Model).CursorPosition != SystemModule.CursorPosition.Default);
        }
    }

    public enum CursorPosition {
        Default,
        Start,
        End
    }
}
