using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.Taxonomies{
    public partial class GenericWeakReferenceNestedListViewController : ViewController<ListView>{
        public GenericWeakReferenceNestedListViewController(){
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated(){
            base.OnActivated();
            
            var standardController = Frame.GetController<NewObjectViewController>();
            standardController.ObjectCreated += StandardControllerObjectCreated;
        }

        private void StandardControllerObjectCreated(object sender, ObjectCreatedEventArgs e){
            ((XPWeakReference) e.CreatedObject).Target = e.ObjectSpace.GetObject(((PropertyCollectionSource) View.CollectionSource).MasterObject);
        }

        protected override void OnDeactivating(){
            base.OnDeactivating();
            //FrameAssigned -= GenericWeakReferenceNestedListViewController_FrameAssigned;
        }
    }
}