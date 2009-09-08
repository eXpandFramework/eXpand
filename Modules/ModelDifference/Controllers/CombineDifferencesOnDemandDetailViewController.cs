using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using System.Linq;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;

namespace eXpand.ExpressApp.ModelDifference.Controllers
{
    public partial class CombineDifferencesOnDemandDetailViewController : ViewController<DetailView>
    {
        private DialogController controller;

        public CombineDifferencesOnDemandDetailViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (ModelDifferenceObject);
        }

        public SimpleAction CombineAction{
            get {
                return combineAction;
            }
        }

        public DialogController DialogController
        {
            get { return controller; }
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            controller = new DialogController();
            controller.AcceptAction.Execute += AcceptActionOnExecute;
        }
        public void CreatePopupListView(ShowViewParameters parameters){
            
            parameters.Controllers.Add(controller);
            parameters.CreatedView = Application.CreateListView(Application.FindListViewId(typeof (ModelDifferenceObject)),GetCollectionSource(), true);
            parameters.TargetWindow = TargetWindow.NewModalWindow;
            parameters.Context = TemplateContext.PopupWindow;


        }

        private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs args){
            var modelAspectObject = ((ModelDifferenceObject) View.CurrentObject);
            var dictionary = new Dictionary(modelAspectObject.Model.RootNode, Application.Model.Schema);
            foreach (var selectedObject in args.SelectedObjects.Cast<ModelDifferenceObject>()){
                var combiner = new DictionaryCombiner(dictionary);
                combiner.CombineWith(selectedObject);
            }
            modelAspectObject.Model = dictionary;
        }

        internal CollectionSourceBase GetCollectionSource()
        {

            var source = new CollectionSource(ObjectSpace, TargetObjectType);
            var currentModelAspectObject = ((ModelDifferenceObject)View.CurrentObject);
            var expression =
                new XPQuery<ModelDifferenceObject>(ObjectSpace.Session).TransformExpression(
                    m =>
                    m.Oid != currentModelAspectObject.Oid && 
                    m.PersistentApplication.Name == currentModelAspectObject.PersistentApplication.Name);
            source.Criteria["excludeCurrentObject"] = expression;
            return source;
        }

        private void combineAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            CreatePopupListView(e.ShowViewParameters);
        }
    }
}
