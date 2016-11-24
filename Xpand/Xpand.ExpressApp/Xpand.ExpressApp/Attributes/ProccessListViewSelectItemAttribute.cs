using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ProccessListViewSelectItemAttribute : Attribute
    {
        public ProccessListViewSelectItemAttribute(string action, string detailView)
        {
            this.Action = action;
            this.DetailView = detailView;
        }

        public ProccessListViewSelectItemAttribute(bool handled)
        {
            this.Handled = handled;
        }

        public TemplateContext? TemplateContext { get; set; }
        public bool? AllowAllControllers { get; set; }
        public NewWindowTarget? NewWindowTarget { get; set; }
        public TargetWindow? TargetWindow { get; set; }
        public bool? Handled { get; set; }
        private bool _HandledOnActionNonExecution = true;
        public bool HandledOnActionNonExecution { get { return _HandledOnActionNonExecution; } set { _HandledOnActionNonExecution = value; } }
        public string Action { get; set; }
        public string DetailView { get; set; }
    }
}
