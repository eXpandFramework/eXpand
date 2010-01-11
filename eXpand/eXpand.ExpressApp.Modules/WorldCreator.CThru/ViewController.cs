using CThru;

namespace eXpand.ExpressApp.WorldCreator.CThru
{
    public partial class ViewController : DevExpress.ExpressApp.ViewController {
        public ViewController() {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.Committing += (o, eventArgs) => {
                CThruEngine.AddAspect(new ExistentMembersEnableValidationAspect());
                CThruEngine.StartListening();
            };
            ObjectSpace.Committed += (sender, args) => CThruEngine.StopListeningAndReset();
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            CThruEngine.StopListeningAndReset();
        }
    }
}
