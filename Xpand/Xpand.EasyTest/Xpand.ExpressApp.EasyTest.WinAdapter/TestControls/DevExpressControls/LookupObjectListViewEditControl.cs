using System;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;

namespace Xpand.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls {
    public class LookupObjectListViewEditControl : DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Xaf.LookupObjectListViewEditControl,IControlAct {
        public LookupObjectListViewEditControl(LookupEdit control) : base(control){
            
        }

        protected override void InternalSetText(string text){
            try{
                control.ShowPopup();
                CollectionSourceBase collection = control.PopupForm.ListView.CollectionSource;
                object editValue = FindSetEditValueByDisplayText(control, collection, text);
                control.ClosePopup();
                control.EditValue = editValue;
                control.IsModified = true;
            }
            catch (Exception e){
                throw new AdapterOperationException(e.Message);
            }
            finally{
                control.ClosePopup();
            }
        }

        void IControlAct.Act(string value){
            if (value == "focus"){
                control.Focus();
            }
            else{
                base.Act(value);
            }
        }
    }
}
