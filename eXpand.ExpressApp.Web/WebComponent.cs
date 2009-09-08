﻿using System.ComponentModel;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;
using eXpand.Persistent.Base;


namespace eXpand.ExpressApp.Web
{
    public partial class WebComponent : DevExpress.ExpressApp.Web.WebApplication
    {

        public WebComponent()
        {
            InitializeComponent();
            DatabaseVersionMismatch += (sender, args) => this.DatabaseVersionMismatchEvent(sender, args);

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