using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using eXpand.ExpressApp.Taxonomy.BaseObjects;

namespace eXpand.ExpressApp.Taxonomy.Controllers{
    public partial class TaxonomyQueryController : ViewController {
        public TaxonomyQueryController() {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (TaxonomyQuery);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        private void ActionOnExecute(object sender, SimpleActionExecuteEventArgs args) {
            ObjectSpace space = Application.CreateObjectSpace();
            
            var source = new CollectionSource(space, typeof(Term));
            source.Criteria["Query"] = ((TaxonomyQuery)args.CurrentObject).ParseCriteria();

            args.ShowViewParameters.CreatedView = Application.CreateListView(Application.GetListViewId(typeof(Term)), source, false);
            args.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        }
    }
}