using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.Security;
using eXpand.ExpressApp.ModelArtifactState.Security.Permissions;

namespace eXpand.ExpressApp.ModelArtifactState.Security.Controllers{
    public partial class FillRulesFromPermissionsController : ViewController
    {
        public FillRulesFromPermissionsController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(IPersistentPermission);
        }

        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.ObjectDeleted+=ObjectSpaceOnObjectDeleted;
            ObjectSpace.ObjectSaved+=ObjectSpaceOnObjectSaved;
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            ObjectSpace.ObjectDeleted -= ObjectSpaceOnObjectDeleted;
            ObjectSpace.ObjectSaved -= ObjectSpaceOnObjectSaved;
        }
        private void ObjectSpaceOnObjectSaved(object sender, ObjectManipulatingEventArgs args) {
            var permission = (args.Object) as IPersistentPermission;
            if (permission != null && permission.Permission is ArtifactStateRulePermission) {
                ModelArtifactStateModule.CollectRules(Application);
            }
        }

        private void ObjectSpaceOnObjectDeleted(object sender, ObjectsManipulatingEventArgs args){
            ModelArtifactStateModule.CollectRules(Application);
        }


    }
}