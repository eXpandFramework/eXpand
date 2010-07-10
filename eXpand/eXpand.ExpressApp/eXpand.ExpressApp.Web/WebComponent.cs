using System.ComponentModel;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;


namespace eXpand.ExpressApp.Web
{
    public abstract partial class WebComponent : DevExpress.ExpressApp.Web.WebApplication,ISupportModelsManager
    {
        protected WebComponent()
        {
            InitializeComponent();
        }

        protected override void OnCustomProcessShortcut(CustomProcessShortcutEventArgs args)
        {
            base.OnCustomProcessShortcut(args);
            new ViewShortCutProccesor(this).Proccess(args);
            
        }
        public ApplicationModelsManager ModelsManager
        {
            get { return modelsManager; }
        }

        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            this.CreateCustomObjectSpaceprovider(args);
            base.OnCreateCustomObjectSpaceProvider(args);
        }

        protected WebComponent(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }


    }
}