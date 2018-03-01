using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.SystemModule;

namespace SystemTester.Module.Win.FunctionalTests.ActiveDocumentView{

    public class ActiveDocumentController:ObjectViewController<DetailView,ActiveDocumentView>{
        protected override void OnActivated(){
            base.OnActivated();
            Application.MainWindow.GetController<ActiveDocumentViewController>().ActiveViewChanged+=OnActiveViewChanged;
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Application.MainWindow.GetController<ActiveDocumentViewController>().ActiveViewChanged-=OnActiveViewChanged;
        }

        private void OnActiveViewChanged(object sender, ActiveViewChangedEventArgs e){
            ((ActiveDocumentView) View.CurrentObject).Name = "inactive";
            ObjectSpace.CommitChanges();
            if (e.View != null){
                ((ActiveDocumentView) e.View.CurrentObject).Name = "active";
            }
        }
    }
    [DefaultClassOptions]
    public class ActiveDocumentView:BaseObject{
        public ActiveDocumentView(Session session) : base(session){
        }

        string _name;

        public string Name{
            get{ return _name; }
            set{ SetPropertyValue(nameof(Name), ref _name, value); }
        }
    }
}