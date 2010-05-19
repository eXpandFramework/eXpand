using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using eXpand.Xpo;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.SystemModule
{
    public abstract class GridOptionsController<ControlType, SettingsType> : ViewController<ListView>
    {
        object Getvalue(PropertyInfo propertyInfo, string attributeValue, object settings)
        {
            return string.IsNullOrEmpty(attributeValue) ? propertyInfo.GetValue(settings, null) : ReflectorHelper.ChangeType(attributeValue, propertyInfo.PropertyType);
        }

        //        public override void ExtendModelInterfaces(DevExpress.ExpressApp.Model.ModelInterfaceExtenders extenders)
        //        {
        //            base.ExtendModelInterfaces(extenders);
        //extenders.Add<IModelListView, IModelListViewGridOptions>();
        //        }

        protected override void OnViewControlsCreated()
        {
            //            throw new NotImplementedException();
            base.OnViewControlsCreated();
            var dictionaryNode = View.Model;
            if (dictionaryNode != null)
            {
                var propertyInfos = typeof(ControlType).GetProperties().Where(info1 => typeof(SettingsType).IsAssignableFrom(info1.PropertyType));
                //foreach (DictionaryNode childNode in dictionaryNode.ChildNodes) {
                //    DictionaryNode node = childNode;
                //    var propertyInfo = propertyInfos.Where(info1 => info1.Name==node.Name).FirstOrDefault();
                //    if (propertyInfo != null) {
                //        object settings = propertyInfo.GetValue(GetControl(), null);
                //        Delegate(childNode.Name, settings, dictionaryNode);
                //    }
                //}
            }
        }

        protected virtual object GetControl()
        {
            return View.Editor.Control;
        }

        void Delegate(string sourceChildNode, object settings, object optons)
        {
            //if (optons != null)
            //foreach (var dictionaryAttribute in dictionaryNode.Attributes.OfType<DictionaryAttribute>()){
            //    var attributeValue = dictionaryNode.GetAttributeValue(dictionaryAttribute.Name);
            //    DictionaryAttribute attribute = dictionaryAttribute;
            //    var single = settings.GetType().GetProperties().Where(propertyInfo => propertyInfo.Name == attribute.Name).Single();
            //    if (!string.IsNullOrEmpty(attributeValue)){
            //        var value = Getvalue(single, attributeValue, settings);
            //        single.SetValue(settings, value, null);
            //    }
            //}
        }

    }
}