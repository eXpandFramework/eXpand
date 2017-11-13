using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Maps.Mobile;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using MainDemo.Module.BusinessObjects;
using MainDemo.Module.Controllers;

namespace MainDemo.Module.Mobile.Controllers {
    public partial class DisableActionsController : ViewController {
        public DisableActionsController() {
            InitializeComponent();
        }
        protected override void OnActivated() {
            base.OnActivated();
            ClearContactTasksController clearTasksController = Frame.GetController<ClearContactTasksController>();
            if(clearTasksController != null) {
                clearTasksController.Actions["ClearTasksAction"].Active["ByDesign"] = false;
            }
            if(View is DetailView && View.ObjectTypeInfo.Type == typeof(Contact)) {
                GetCurrentLocationController getCurrentLocation = Frame.GetController<GetCurrentLocationController>();
                if(getCurrentLocation != null) {
                    getCurrentLocation.GetCurrentLocationAction.Enabled["EnabledForContact"] = false;
                }
            }
        }
        protected override void OnDeactivated() {
            if(View is DetailView && View.ObjectTypeInfo.Type == typeof(Contact)) {
                GetCurrentLocationController getCurrentLocation = Frame.GetController<GetCurrentLocationController>();
                if(getCurrentLocation != null) {
                    getCurrentLocation.GetCurrentLocationAction.Enabled["EnabledForContact"] = true;
                }
            }
        }
    }
}
