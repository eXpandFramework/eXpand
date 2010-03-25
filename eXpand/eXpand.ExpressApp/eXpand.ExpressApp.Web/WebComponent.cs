using System.ComponentModel;
using DevExpress.ExpressApp;
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
            if (args.Shortcut.ObjectKey.StartsWith("@"))
                args.Shortcut.ObjectKey = ParametersFactory.CreateParameter(args.Shortcut.ObjectKey.Substring(1)).CurrentValue.ToString();
        }
        protected override void OnCreateCustomObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            this.CreateCustomObjectSpaceprovider(args);
            base.OnCreateCustomObjectSpaceProvider(args);
        }

        public WebComponent(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }


    }
}