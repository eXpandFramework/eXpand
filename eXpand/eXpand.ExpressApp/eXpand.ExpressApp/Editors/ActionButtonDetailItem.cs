using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.Editors {
    public interface IModelActionButton : IModelDetailViewItem
    {
        [DataSourceProperty("Application.ActionDesign.Actions")]
        [Required]
        IModelAction ActionId { get; set; }
    }
    
    public abstract class ActionButtonDetailItem : ViewItem {
        private readonly IModelDetailViewItem _model;

        protected ActionButtonDetailItem(Type objectType, string id) : base(objectType, id) {
        }

        protected ActionButtonDetailItem(IModelDetailViewItem model, Type objectType)
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