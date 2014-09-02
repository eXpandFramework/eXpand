using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using Xpand.Utils.Linq;

namespace SecurityTester.Module.FunctionalTests.NavigationItemPermission{
    public class NavigationItemPermission:ViewController {
        public NavigationItemPermission(){
            var singleChoiceAction = new SingleChoiceAction(this,GetType().Name,PredefinedCategory.ObjectsCreation);
            TargetObjectType = typeof (NavigationItemPermissionObject);
            singleChoiceAction.Items.Add(new ChoiceActionItem("None",null));
            singleChoiceAction.Execute+=SingleChoiceActionOnExecute;
        }
        public static ISecurityStrategyBase SessionSecurity { get; set; }

        private void SingleChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e){
            if (e.SelectedChoiceActionItem.Id == "None"){
                Application.Security=new SecurityDummy();
                SecuritySystem.SetInstance(Application.Security);
//                var rootNavigationItems = ((IModelApplicationNavigationItems) Application.Model).NavigationItems;
//                var modelNavigationItems = rootNavigationItems.Items.GetItems<IModelNavigationItem>(item => item.Items);
//                var navigationItem = modelNavigationItems.First(item => item.Id=="Default");
//                navigationItem.Remove();
//                rootNavigationItems.StartupNavigationItem = modelNavigationItems.First(item => item.View == View.Model);
//                SessionSecurity = null;
//                Frame.GetController<ShowNavigationItemController>().RecreateNavigationItems();
//                Frame.GetController<LogoffController>().LogoffAction.DoExecute();
            }
        }
    }
    sealed class SecurityDummy : ISecurity {
        public Boolean IsGranted(IPermission permission) {
            return true;
        }
        public Type GetModuleType() {
            return null;
        }
        public IList<Type> GetBusinessClasses() {
            return Type.EmptyTypes;
        }
        public void Logon(IObjectSpace objectSpace) { }
        public void Logoff() { }
        public void ClearSecuredLogonParameters() { }
        public void ReloadPermissions() { }
        public Type UserType { get { return null; } }
        public Object User { get { return null; } }
        public String UserName { get { return ""; } }
        public Object UserId { get { return null; } }
        public Object LogonParameters { get { return null; } }
        public Boolean NeedLogonParameters { get { return false; } }
        public Boolean IsAuthenticated { get { return true; } }
        public Boolean IsLogoffEnabled { get { return false; } }
        public IObjectSpace LogonObjectSpace { get { return null; } }
    }

}