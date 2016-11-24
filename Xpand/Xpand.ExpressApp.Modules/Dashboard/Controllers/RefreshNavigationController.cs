using System;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Dashboard.Controllers {
    public class RefreshNavigationController : ViewController {
        bool _objectChanged;
        private static readonly string _xmlPropertyName;

        static RefreshNavigationController(){
            _xmlPropertyName = ReflectionExtensions.GetPropertyName<IDashboardDefinition>(definition => definition.Xml);
        }

        public RefreshNavigationController() {
            TargetObjectType = typeof(IDashboardDefinition);
        }

        protected override void OnActivated() {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            ObjectSpace.Committed += ObjectSpace_Committed;
        }

        protected override void OnDeactivated() {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            ObjectSpace.Committed -= ObjectSpace_Committed;
            base.OnDeactivated();
        }

        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
            if (e.NewValue != e.OldValue&&e.PropertyName==_xmlPropertyName)
                _objectChanged = true;
        }

        void ObjectSpace_Committed(object sender, EventArgs e) {
            if (_objectChanged){
                Frame.Application.MainWindow.GetController<DashboardNavigationController>().RecreateNavigationItems();
                _objectChanged = false;
            }
        }
    }
}