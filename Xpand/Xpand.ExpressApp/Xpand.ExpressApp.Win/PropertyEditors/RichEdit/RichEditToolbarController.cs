using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraRichEdit;

namespace Xpand.ExpressApp.Win.PropertyEditors.RichEdit {
    public class RichEditToolbarController:ViewController<DetailView>{
        private Control _lastFocusedControl;
        private IList<RichEditWinPropertyEditor> _richEditWinPropertyEditors;

        protected override void OnActivated(){
            base.OnActivated();
            _richEditWinPropertyEditors = View.GetItems<RichEditWinPropertyEditor>();
            if (_richEditWinPropertyEditors.Any()){
                foreach (var item in View.GetItems<PropertyEditor>()){
                    item.ControlCreated += Item_ControlCreated;
                }
            }
        }

        private void Item_ControlCreated(object sender, EventArgs e){
            var richEditWinPropertyEditor = sender as RichEditWinPropertyEditor;
            if (richEditWinPropertyEditor != null)
                richEditWinPropertyEditor.Control.RichEditControl.GotFocus += RichEditControl_GotFocus;
            var propertyEditor = ((PropertyEditor) sender) ;
            ((Control)propertyEditor.Control).GotFocus += OnGotFocus;
        }

        private void RichEditControl_GotFocus(object sender, EventArgs e) {
            var richEditControl = ((RichEditControl)sender);
            if (_lastFocusedControl!=richEditControl){
                DestroyAllToolBars();
                ((RichEditContainerBase) richEditControl.Parent).CreateToolBars();
                _lastFocusedControl = richEditControl;
            }
        }

        private void OnGotFocus(object sender, EventArgs e){
            _lastFocusedControl = (Control) sender;
            DestroyAllToolBars();
        }

        private void DestroyAllToolBars(){
            foreach (var item in _richEditWinPropertyEditors){
                if (item.Control != null) item.Control.DestroyToolBar();
            }
        }
    }
}
