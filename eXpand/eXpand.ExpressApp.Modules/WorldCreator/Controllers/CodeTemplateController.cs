using System.Diagnostics;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Observers;

namespace eXpand.ExpressApp.WorldCreator.Controllers {
    public class CodeTemplateController : ViewController
    {
        public CodeTemplateController() {
            Debug.Print("");
        }

        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            new CodeTemplateObserver(view.ObjectSpace);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
//        protected override void OnActivated()
//        {
//            base.OnActivated();
//            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
//        }

//        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
//            if (objectChangedEventArgs.Object is ICodeTemplate &&
//                ((ICodeTemplate) objectChangedEventArgs.Object).GetPropertyName(x => x.TemplateType) ==
//                objectChangedEventArgs.PropertyName)
//                ((ICodeTemplate) objectChangedEventArgs.Object).SetDefaults();  
//        }
    }
}