using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;

namespace eXpand.ExpressApp.Win.Editors
{
    public class FilterControl : DevExpress.XtraEditors.FilterControl
    {

        public event Action<BaseEdit> EditorActivated;

        protected void InvokeEditorActivated(BaseEdit baseEdit)
        {
            Action<BaseEdit> activated = EditorActivated;
            if (activated != null && baseEdit != null) activated(baseEdit);
        }


        protected override void OnFocusedElementChanged()
        {
            base.OnFocusedElementChanged();
            InvokeEditorActivated(ActiveEditor);
        }



        protected override void ShowElementMenu(ElementType type)
        {
            base.ShowElementMenu(type);
            InvokeEditorActivated(ActiveEditor);

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            InvokeEditorActivated(ActiveEditor);

        }
    }
}