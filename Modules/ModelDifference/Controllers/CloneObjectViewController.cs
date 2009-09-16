using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public partial class CloneObjectViewController : DevExpress.ExpressApp.CloneObject.CloneObjectViewController
    {
        private DialogController _dialogController;

        public CloneObjectViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void CloneObject(SingleChoiceActionExecuteEventArgs args)
        {
            base.CloneObject(args);
            var modelDifferenceObject = (ModelDifferenceObject) args.ShowViewParameters.CreatedView.CurrentObject;
            modelDifferenceObject.DateCreated = DateTime.Now;
            modelDifferenceObject.Disabled = true;
            modelDifferenceObject.Name = null;
            modelDifferenceObject.PersistentApplication = (PersistentApplication) modelDifferenceObject.Session.GetObject(((ModelDifferenceObject)View.CurrentObject).PersistentApplication);



        }

        public void CreateDifferenceTypeDetailView(SingleChoiceActionExecuteEventArgs args){
            var differenceTypeObject = new DifferenceTypeObject();
            ShowViewParameters parameters = args.ShowViewParameters;
            parameters.Context=TemplateContext.PopupWindow;
            parameters.TargetWindow=TargetWindow.NewModalWindow;
            parameters.CreatedView = Application.CreateDetailView(Application.CreateObjectSpace(),differenceTypeObject);
            _dialogController = new DialogController();
            parameters.Controllers.Add(_dialogController);
            _dialogController.AcceptAction.Execute+=AcceptActionOnExecute;
        }

        public DialogController DialogController{
            get { return _dialogController; }
        }

        private void AcceptActionOnExecute(object sender, SimpleActionExecuteEventArgs args){
            var cloner = new Cloner();
            var xpSimpleObject = (ModelDifferenceObject) cloner.CloneTo((IXPSimpleObject) View.CurrentObject, getType((DifferenceTypeObject) args.CurrentObject));
            xpSimpleObject.DateCreated = DateTime.Now;
            xpSimpleObject.Disabled = true;
            xpSimpleObject.Name = null;
            xpSimpleObject.Session.Save(xpSimpleObject);

        }

        private Type getType(DifferenceTypeObject currentObject){
            if (currentObject.DifferenceType==DifferenceType.Model)
                return typeof(ModelDifferenceObject);
            if (currentObject.DifferenceType==DifferenceType.User)
                return typeof(UserModelDifferenceObject);
            if (currentObject.DifferenceType==DifferenceType.Role)
                return typeof(RoleModelDifferenceObject);
            throw new NotImplementedException();
        }
    }
}