using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using eXpand.ExpressApp.Security.Permissions;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.Security.Controllers
{
    public partial class StatePermissionSynchronizerController : ViewController
    {
        public StatePermissionSynchronizerController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(LogicRulePermission);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args){
            var statePermission = View.CurrentObject as LogicRulePermission;
            if (statePermission != null)
                if (string.IsNullOrEmpty(statePermission.ViewId) &&args.PropertyName == statePermission.GetPropertyInfo(x => x.ObjectType).Name &&args.NewValue != null){
                    if (new ApplicationNodeWrapper(Application.Model).Views.FindViewById(statePermission.ViewId).BusinessObjectType != args.NewValue)
                        statePermission.ViewId = null;
                }
        }
    }
}
