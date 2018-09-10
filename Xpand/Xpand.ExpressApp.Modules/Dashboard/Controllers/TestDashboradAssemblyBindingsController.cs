using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Services;

namespace Xpand.ExpressApp.Dashboard.Controllers{
    public class TestDashboradAssemblyBindingsController:ObjectViewController<DetailView,IDashboardDefinition> {
        protected override void OnActivated() {
            base.OnActivated();
            View.CurrentObjectChanged+=ViewOnCurrentObjectChanged;
            TestDashboradAssemblyBindings();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            View.CurrentObjectChanged-=ViewOnCurrentObjectChanged;
        }

        private void TestDashboradAssemblyBindings(){
            var dashboardDefinition = ((IDashboardDefinition) View.CurrentObject);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
            dashboardDefinition?.ToDashboard();
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args) {
            var value = Regex.Match(args.Name, @"\A.[^,]*").Value;
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == value);
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs e) {
            TestDashboradAssemblyBindings();
        }
    }
}