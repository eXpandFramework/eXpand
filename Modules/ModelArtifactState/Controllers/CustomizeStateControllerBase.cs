using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelArtifactState.Controllers
{
    public abstract partial class CustomizeStateControllerBase : ViewController
    {
        protected CustomizeStateControllerBase()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        /// <summary>
        /// link the events
        /// </summary>
        protected override void OnActivated()
        {
            base.OnActivated();
            activateAllControllersForAllDepentedContexts();
            ForceActivation();
            View.CurrentObjectChanged += OnCurrentObjectChanged;
            View.ObjectSpace.ObjectChanged += OnObjectChanged;
        }

        private void activateAllControllersForAllDepentedContexts()
        {
            foreach (var controller in Frame.Controllers.Values)
                controller.Active[ActivationContext] = true;
        }

        protected abstract string ActivationContext { get; }

        /// <summary>
        /// unlink the events
        /// </summary>
        protected override void OnDeactivating()
        {
            View.CurrentObjectChanged -= OnCurrentObjectChanged;
            View.ObjectSpace.ObjectChanged -= OnObjectChanged;
            base.OnDeactivating();
        }

        private void OnCurrentObjectChanged(object sender, EventArgs e)
        {
            if (View != null)
                ForceActivation();
        }

        private void OnObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (View!= null)
                ForceActivation();
        }

        protected virtual void ForceActivation()
        {
            
        }
        

    }
}