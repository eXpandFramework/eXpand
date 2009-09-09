using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.Controllers{
    public partial class CreateNewDifferenceObjectViewController : ViewController
    {
        public CreateNewDifferenceObjectViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (ModelDifferenceObject);

        }

        
        protected override void OnActivated()
        {
            base.OnActivated();
            var controller = Frame.GetController<NewObjectViewController>();
            controller.ObjectCreating+=ControllerOnObjectCreating;
            controller.ObjectCreated+=OnObjectCreated;
            
                
        }

        private void ControllerOnObjectCreating(object sender, ObjectCreatingEventArgs args){
            if (typeof(ModelDifferenceObject).IsAssignableFrom(args.ObjectType))
            {
                throw new UserFriendlyException(new Exception("Only cloned is allowed"));
            }
        }

        [CoverageExclude]
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            Frame.GetController<NewObjectViewController>().ObjectCreated -= OnObjectCreated;
        }

        protected virtual internal void OnObjectCreated(object sender, ObjectCreatedEventArgs args){
            ((ModelDifferenceObject) args.CreatedObject).InitializeMembers(Application.Title, ((IApplicationUniqueName) Application).UniqueName);
        }
    }
}