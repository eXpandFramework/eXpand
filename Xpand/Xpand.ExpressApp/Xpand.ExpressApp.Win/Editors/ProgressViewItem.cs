using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors;
using Xpand.ExpressApp.Editors;

namespace Xpand.ExpressApp.Win.Editors {
    [ViewItem(typeof(IModelProgressViewItem))]
    public class ProgressViewItem:ExpressApp.Editors.ProgressViewItem,IComplexViewItem {
        private XafApplication _application;

        public ProgressViewItem(IModelProgressViewItem info, Type classType) : base(info, classType){
        }

        public ProgressBarControl ProgressBarControl { get; private set; }

        protected override object CreateControlCore() {
            ProgressBarControl = new ProgressBarControl();
            return ProgressBarControl;
        }


        public override void SetPosition(decimal value) {
            base.SetPosition(value);
            ProgressBarControl.Position = (int) value;

        }

        public override void SetFinishOptions(MessageOptions messageOptions) {
            base.SetFinishOptions(messageOptions);
            _application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _application = application;
        }
    }
}
