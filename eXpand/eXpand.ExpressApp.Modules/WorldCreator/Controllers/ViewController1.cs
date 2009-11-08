using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using eXpand.Persistent.Base.General;

namespace eXpand.ExpressApp.WorldCreator.Controllers
{
    public partial class ViewController1 : ViewController
    {
        public ViewController1()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof (IHidden);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            
        }
    }
}
