using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace eXpand.ExpressApp.Editors {
    public abstract class ActionButtonDetailItem : DetailViewItem {
        protected ActionButtonDetailItem(string id) : base(id) {
        }

        protected ActionButtonDetailItem(DictionaryNode info) : base(info) {
        }

        protected ActionButtonDetailItem(Type objectType, DictionaryNode info) : base(objectType, info) {
        }

        public event EventHandler Executed;

        public override string Caption {
            get { return Info.GetAttributeValue("Caption"); }
            set { throw new NotImplementedException(); }
        }

        protected void InvokeExecuted(EventArgs e) {
            EventHandler handler = Executed;
            if (handler != null) handler(this, e);
        }
    }
}