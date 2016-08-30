using System;
using DevExpress.ExpressApp.Controls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.TreeListEditors.Web;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.TreeListEditors.Web.ListEditors {
    public class XpandASPxTreeListEditor : ASPxTreeListEditor, IDataBound{
        public event EventHandler<EventArgs> DataBound;
        public XpandASPxTreeListEditor(IModelListView model)
            : base(model) {
            
        }

        protected override ASPxTreeListDataBinderBase CreateDataBinder(NodeObjectAdapter adapter){
            var binder = base.CreateDataBinder(adapter);
            binder.DataBound+=BinderOnDataBound;
            return binder;
        }

        private void BinderOnDataBound(object sender, EventArgs eventArgs){
            OnDataBound();
        }

        protected virtual void OnDataBound(){
            DataBound?.Invoke(this, EventArgs.Empty);
        }
    }
}
