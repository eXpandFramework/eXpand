using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.WorldCreator.Observers {
    public class CodeTemplateInfoObserver:ObjectObserver<ICodeTemplateInfo> {
        public CodeTemplateInfoObserver(ObjectSpace objectSpace) : base(objectSpace) {
        }

        protected override void OnChanged(ObjectChangedEventArgs<ICodeTemplateInfo> objectChangedEventArgs)
        {
            base.OnChanged(objectChangedEventArgs);
            if (objectChangedEventArgs.NewValue != null && objectChangedEventArgs.Object.GetPropertyName(x => x.CodeTemplate) == objectChangedEventArgs.PropertyName){
                CloneProperties(objectChangedEventArgs.Object, objectChangedEventArgs.NewValue);
            }

        }

        void CloneProperties(ICodeTemplateInfo codeTemplateInfo, object newValue)
        {
            var templateInfo = (ITemplateInfo) (codeTemplateInfo.TemplateInfo ?? ObjectSpace.CreateObject(TypesInfo.Instance.TemplateInfoType));
            codeTemplateInfo.TemplateInfo=templateInfo;
            var type = templateInfo.GetType();
            var infos = typeof(ITemplateInfo).GetProperties().Select(propertyInfo =>
                                                                     new
                                                                     {
                                                                         Value = propertyInfo.GetValue(newValue, null),
                                                                         TypeInfoPropertyInfo = type.GetProperty(propertyInfo.Name)
                                                                     });
            foreach (var info in infos){
                info.TypeInfoPropertyInfo.SetValue(templateInfo, info.Value, null);
            }
        }
    }
}