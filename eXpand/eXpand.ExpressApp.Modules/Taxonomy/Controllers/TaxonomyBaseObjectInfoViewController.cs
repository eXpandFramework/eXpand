using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.ExpressApp.Taxonomy.Controllers
{
    public partial class TaxonomyBaseObjectInfoViewController : ViewController
    {
        public TaxonomyBaseObjectInfoViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            var action = new SimpleAction(Container);
            action.Id = Guid.NewGuid().ToString();
            action.Caption = "addddddddd";
            Actions.Add(action);
            TargetViewType=ViewType.DetailView;
            TargetObjectType = typeof (TaxonomyBaseObjectInfo);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            if (View.ObjectSpace.Session.IsNewObject(View.CurrentObject))
                View.ObjectSpace.ObjectSaving+=ObjectSpaceOnObjectSaving;
        }

        private void ObjectSpaceOnObjectSaving(object sender, ObjectManipulatingEventArgs args){
            if (args.Object is TaxonomyBaseObjectInfo){
                var taxonomyBaseObjectInfo = ((TaxonomyBaseObjectInfo) args.Object);
                Term oldCategory = taxonomyBaseObjectInfo.Category;
                Term newCategory = new Term(taxonomyBaseObjectInfo.Session){
                                                                               ParentTerm = oldCategory,
                                                                               Key = taxonomyBaseObjectInfo.Owner.ToString()
                                                                           };
                taxonomyBaseObjectInfo.Category = newCategory;
            }
        }
    }
}
