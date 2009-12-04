using DevExpress.ExpressApp;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator {
    public partial class GenreratePersistentTypeInfoAttributeCodeController : ViewController {
        public GenreratePersistentTypeInfoAttributeCodeController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IPersistentAttributeInfo);
        }

        protected override void OnActivated() {
            base.OnActivated();
//            Frame.GetController<TrackNewObjectSavedController>().NewObjectSaved += OnNewObjectSaved;
        }

        void OnNewObjectSaved(object sender, NewObjectSavedEventArgs newObjectSavedEventArgs) {
            if (newObjectSavedEventArgs.Object == View.CurrentObject) {
                var persistentAttributeInfo = ((IPersistentAttributeInfo) View.CurrentObject);
                persistentAttributeInfo.Owner.GeneratedCode = CodeEngine.GenerateCode(persistentAttributeInfo) + persistentAttributeInfo.Owner.GeneratedCode; 
            }
        }
    }
}