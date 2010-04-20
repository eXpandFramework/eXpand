using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.Core;


namespace eXpand.ExpressApp.Web
{
    public abstract partial class WebComponent : DevExpress.ExpressApp.Web.WebApplication
    {
        protected WebComponent()
        {
            InitializeComponent();
        }

        protected override void OnCustomProcessShortcut(CustomProcessShortcutEventArgs args)
        {
            base.OnCustomProcessShortcut(args);
            new ViewShortCutProccesor().Proccess(args.Shortcut);
            
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