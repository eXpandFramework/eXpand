using DevExpress.ExpressApp.Win.SystemModule;
using eXpand.ExpressApp.Taxonomy.Controllers;

namespace eXpand.ExpressApp.Taxonomy.Win.Controllers{
    public partial class TermWinViewController : TermViewController{
        public TermWinViewController(){
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated(){
            base.OnActivated();
            newObjectAction = Frame.GetController<WinNewObjectViewController>().NewObjectAction;
            newObjectAction.Execute += NewObjectActionOnExecute;
        }

        protected override void OnDeactivating(){
            base.OnDeactivating();
            newObjectAction.Execute -= NewObjectActionOnExecute;
        }
    }
}