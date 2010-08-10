using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using System.Linq;
using eXpand.Persistent.Base.General;

namespace FeatureCenter.Module
{
    public class DisplayFeatureModelController:ViewController
    {
        public event EventHandler<RequestingModelNameArgs> RequestingModelName;

        public void OnRequestingModelName(RequestingModelNameArgs e) {
            EventHandler<RequestingModelNameArgs> handler = RequestingModelName;
            if (handler != null) handler(this, e);
        }

        public const string NoModelAssociated = "NoModelAssociated";
        readonly SimpleAction _simpleAction;
        DisplayFeatureModelAttribute _displayFeatureModelAttribute;

        public DisplayFeatureModelController() {
            _simpleAction = new SimpleAction(this,"Differences",PredefinedCategory.View);
            _simpleAction.Execute+=SimpleActionOnExecute;
            _simpleAction.ImageName = "MenuBar_EditModel";
        }

        public SimpleAction SimpleAction {
            get { return _simpleAction; }
        }

        void SimpleActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var objectSpace = Application.CreateObjectSpace();
            var modelDifferenceObject = new QueryModelDifferenceObject(objectSpace.Session).GetActiveModelDifference(GetModelName());
            simpleActionExecuteEventArgs.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace,modelDifferenceObject);
        }

        string GetModelName() {
            var requestingModelNameArgs = new RequestingModelNameArgs();
            OnRequestingModelName(requestingModelNameArgs);
            if (requestingModelNameArgs.ModelName!= null)
                return requestingModelNameArgs.ModelName;
            var s =_displayFeatureModelAttribute.ModelDifferenceObjectName?? _displayFeatureModelAttribute.ViewId;
            return s.Replace("_DetailView","").Replace("_ListView","");
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var displayFeatureModelAttributes = View.ObjectTypeInfo.FindAttributes<DisplayFeatureModelAttribute>();
            _displayFeatureModelAttribute =
                displayFeatureModelAttributes.Where(AttributePredicate()).SingleOrDefault();
            _simpleAction.Active[NoModelAssociated] = (_displayFeatureModelAttribute != null&&_displayFeatureModelAttribute.ViewId==View.Id);    
        }

        Func<DisplayFeatureModelAttribute, bool> AttributePredicate() {
            return attribute => {
                var view = attribute.ViewId == View.Id;
                var isObjectFitForCriteria = ObjectSpace.IsObjectFitForCriteria(View.CurrentObject, attribute.Criteria);
                if (!(isObjectFitForCriteria.HasValue)||View is ListView)
                    return view;
                return view&&isObjectFitForCriteria.Value;
            };
        }
    }

    public class RequestingModelNameArgs:EventArgs {
        public string ModelName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class DisplayFeatureModelAttribute:Attribute,ISupportViewId {
        readonly CriteriaOperator _criteria;
        readonly string _viewId;
        readonly string _modelDifferenceObjectName;


        public DisplayFeatureModelAttribute(string viewId) {
            _viewId = viewId;
        }

        public DisplayFeatureModelAttribute(string viewId, CriteriaOperator criteria) {
            _viewId = viewId;
            _criteria = criteria;
        }

        public DisplayFeatureModelAttribute(string viewId, string modelDifferenceObjectName) {
            _viewId = viewId;
            _modelDifferenceObjectName = modelDifferenceObjectName;
        }

        public DisplayFeatureModelAttribute(string viewId, string modelDifferenceObjectName, CriteriaOperator criteria)
            : this(viewId, modelDifferenceObjectName)
        {
            _criteria = criteria;
        }

        public CriteriaOperator Criteria {
            get { return _criteria; }
        }

        public string ModelDifferenceObjectName {
            get { return _modelDifferenceObjectName; }
        }

        public string ViewId {
            get {
                return _viewId;
            }
        }

    }
}
