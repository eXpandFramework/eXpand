using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelClassTabStopForReadOnly : IModelNode {
        [Category("eXpand")]
        [Description("If a detailview editor is readonly then you can not navigate to it using the TAB key")]
        bool TabOverReadOnlyEditors { get; set; }
        [Category("eXpand")]
        [Description("Set LayoutControl AllowFocusReadonlyEditors property value")]
        [DefaultValue(true)]
        bool AllowFocusReadonlyEditors { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassTabStopForReadOnly), "ModelClass")]
    public interface IModelDetailViewTabStopForReadOnly : IModelClassTabStopForReadOnly {
        
    }

    public class ReadOnlyTabStopController : ViewController<DetailView>, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelClass, IModelClassTabStopForReadOnly>();
            extenders.Add<IModelDetailView, IModelDetailViewTabStopForReadOnly>();
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (((IModelDetailViewTabStopForReadOnly)View.Model).TabOverReadOnlyEditors) {
                CheckControlsVisibility(View);
            }
            var layoutControl = (LayoutControl) ((WinLayoutManager) (View.LayoutManager)).Container;
            layoutControl.OptionsFocus.AllowFocusReadonlyEditors = ((IModelDetailViewTabStopForReadOnly)View.Model).AllowFocusReadonlyEditors;
        }

        private void CheckControlsVisibility(DetailView xpandDetailView) {
            Guard.ArgumentNotNull(xpandDetailView, "detailView");
            foreach (PropertyEditor propertyEditor in xpandDetailView.GetItems<PropertyEditor>()) {
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