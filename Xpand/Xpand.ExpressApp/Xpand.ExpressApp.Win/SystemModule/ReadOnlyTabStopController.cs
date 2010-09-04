using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors;

namespace Xpand.ExpressApp.Win.SystemModule
{
    public interface IModelClassTabStopForReadOnly : IModelNode
    {
        [Category("eXpand")]
        [Description("If a detailview editor is readonly then you can not navigate to it using the TAB key")]
        bool TabOverReadOnlyEditors { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassTabStopForReadOnly), "ModelClass")]
    public interface IModelDetailViewTabStopForReadOnly : IModelClassTabStopForReadOnly
    {
    }

    public class ReadOnlyTabStopController : ViewController<XpandDetailView>,IModelExtender
    {


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelClass, IModelClassTabStopForReadOnly>();
            extenders.Add<IModelDetailView, IModelDetailViewTabStopForReadOnly>();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            
            

            if (((IModelDetailViewTabStopForReadOnly)View.Model).TabOverReadOnlyEditors){
                CheckControlsVisibility(View);
            }
            
        }
        private void CheckControlsVisibility(XpandDetailView xpandDetailView)
        {
            Guard.ArgumentNotNull(xpandDetailView, "detailView");
            foreach (PropertyEditor propertyEditor in xpandDetailView.GetItems<PropertyEditor>())
            {
                var editor = propertyEditor.Control as BaseEdit;
                if (editor != null) {
                    Boolean controlHasTabStop = editor.TabStop;
                    Boolean revisedTabStop = controlHasTabStop && (!editor.Properties.ReadOnly);
                    if (revisedTabStop != controlHasTabStop) {
                        editor.TabStop = false;
                    }
                }
                
            }
        }

    }
}