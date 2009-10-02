using System.ComponentModel;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;


namespace eXpand.ExpressApp.Web
{
    public partial class WebComponent : DevExpress.ExpressApp.Web.WebApplication
    {

        public WebComponent()
        {
            InitializeComponent();
            DatabaseVersionMismatch += (sender, args) => this.DatabaseVersionMismatchEvent(sender, args);

        }
        public string UniqueName
        {
            get { return "A2ABD988-3361-4f75-8790-E2E08E496AB5"; }
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