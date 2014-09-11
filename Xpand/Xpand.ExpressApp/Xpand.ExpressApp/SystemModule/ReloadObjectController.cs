using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule {
    public interface IModelOptionsReloadSequenceObject {
        [Category(AttributeCategoryNameProvider.Xpand)]
        [Description("Reload Sequence objects after objectspace commit.Useful in MiddleTier because the client is not aware of the sequance assigment from the server")]
        bool ReloadSequenceObject { get; set; }
    }
    public class ReloadObjectController : ViewController, IModelExtender {
        public ReloadObjectController() {
            TargetObjectType = typeof(ISupportSequenceObject);
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (((IModelOptionsReloadSequenceObject)Application.Model.Options).ReloadSequenceObject)
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
        }

        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            ObjectSpace.Refresh();
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (((IModelOptionsReloadSequenceObject)Application.Model.Options).ReloadSequenceObject)
                ObjectSpace.Committed -= ObjectSpaceOnCommitted;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsReloadSequenceObject>();
        }
    }
}
