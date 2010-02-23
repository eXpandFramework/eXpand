using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using System.Linq;

namespace eXpand.ExpressApp.Editors
{
    public abstract class ActionButtonDetailItem : DetailViewItem, IComplexPropertyEditor
    {
        XafApplication _application;
        SimpleAction simpleAction;

        protected ActionButtonDetailItem(string id) : base(id) {
        }

        protected ActionButtonDetailItem(DictionaryNode info) : base(info) {
        }

        protected ActionButtonDetailItem(Type objectType, DictionaryNode info) : base(objectType, info) {
        }

        public SimpleAction SimpleAction {
            get { return simpleAction; }
        }

        public void Setup(ObjectSpace objectSpace, XafApplication application) {
            _application = application;
            simpleAction = _application.Modules[0].ModuleManager.ControllersManager.CollectControllers(typeInfo
                => true).SelectMany(controller => controller.Actions).OfType<SimpleAction>().Where(@base => @base.Id == Info.GetAttributeValue("ActionId")).Single();
        }

        public void ExecuteAction()
        {
            
            simpleAction.DoExecute();
        }
    }
}


