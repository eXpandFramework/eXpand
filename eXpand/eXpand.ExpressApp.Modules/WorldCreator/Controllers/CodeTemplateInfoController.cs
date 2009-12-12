using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Observers;

namespace eXpand.ExpressApp.WorldCreator.Controllers {
    public class CodeTemplateInfoController : ViewController
    {
        protected override void OnViewChanging(View view)
        {
            base.OnViewChanging(view);
            new CodeTemplateInfoObserver(view.ObjectSpace);
        }
//        protected override void OnActivated()
//        {
//            base.OnActivated();
//            ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
//        }

/*
        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs)
        {
            if (objectChangedEventArgs.Object is ICodeTemplateInfo && objectChangedEventArgs.NewValue != null &&
                ((ICodeTemplateInfo)objectChangedEventArgs.Object).GetPropertyName(x => x.CodeTemplate) ==
                objectChangedEventArgs.PropertyName) {
                var persistentTypeInfo = ((ICodeTemplateInfo)objectChangedEventArgs.Object);
                var type = persistentTypeInfo.TemplateInfo.GetType();
                foreach (var info in typeof(ITemplateInfo).GetProperties().Select(propertyInfo =>new {
                                                                                                         Value=propertyInfo.GetValue(objectChangedEventArgs.NewValue,null),
                                                                                                         TypeInfoPropertyInfo=type.GetProperty(propertyInfo.Name)
                                                                                                     })) {
                    info.TypeInfoPropertyInfo.SetValue(persistentTypeInfo.TemplateInfo,info.Value,null);
                }
            }
        }
*/
    }
}