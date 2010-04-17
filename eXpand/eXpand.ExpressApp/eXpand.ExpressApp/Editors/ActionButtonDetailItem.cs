using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Editors {
    public abstract class ActionButtonDetailItem : ViewItem {
        private IModelDetailViewItem _model;

        protected ActionButtonDetailItem(Type objectType, IModelDetailViewItem model)
            : base(objectType, model != null ? model.Id : string.Empty)
        {
            _model = model;
        }

        public event EventHandler Executed;

        public IModelDetailViewItem Model
        {
            get
            {
                return _model;
            }
        }

        public override string Caption {
            get { return _model.Caption; }
            set { throw new NotImplementedException(); }
        }

        protected void InvokeExecuted(EventArgs e) {
            EventHandler handler = Executed;
            if (handler != null) handler(this, e);
        }
    }
}