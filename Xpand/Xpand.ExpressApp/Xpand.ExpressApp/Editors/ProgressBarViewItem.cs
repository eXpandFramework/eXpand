using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Editors{
    
    public interface IProgressViewItem {
        MessageOptions FinishOptions { get;  }
        void SetFinishOptions(MessageOptions messageOptions);
        void Start();
        void SetPosition(decimal value);
        int PollingInterval{ get; set; }
        decimal Position{ get; }
    }
    public interface IModelProgressViewItem : IModelViewItem {
    }

    public abstract class ProgressViewItem:ViewItem,IProgressViewItem {
        

        protected ProgressViewItem(IModelProgressViewItem info, Type classType)
            : base(classType, info.Id){
            PollingInterval = 1000;
        }

        public MessageOptions FinishOptions { get; private set; }

        public virtual void SetFinishOptions(MessageOptions messageOptions) {
            var finishOptions = messageOptions;
            FinishOptions = finishOptions;
        }

        public virtual void Start() {
            FinishOptions = null;
        }

        public decimal Position { get; private set; }

        public virtual void SetPosition(decimal value){
            Position = value;
        }

        public int PollingInterval{ get; set; }
    }
}
