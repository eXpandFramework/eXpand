using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using eXpand.Xpo;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class GridOptionsController:ViewController<ListView>
    {
        object Getvalue(PropertyInfo propertyInfo, string attributeValue, object settings)
        {
            return string.IsNullOrEmpty(attributeValue) ? propertyInfo.GetValue(settings, null) : ReflectorHelper.ChangeType(attributeValue, propertyInfo.PropertyType);
        }

        protected string GetSettingsSchema(Type type1)
        {
            string settingsBehaviour = null;
            return type1.GetProperties().Where(propertyInfo => propertyInfo.GetSetMethod() != null).Aggregate(settingsBehaviour, (current, property) => current + (@"<Attribute Name=""" + property.Name + @""" " + GetChoice(property.PropertyType) + "/>" + Environment.NewLine));
        }

        string GetChoice(Type propertyType)
        {
            if (propertyType == typeof(bool))
                return @"Choice=""True,False""";
            if (propertyType.IsEnum)
                return @"Choice=""{" + propertyType.FullName + @"}""";
            return null;
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            DictionaryNode dictionaryNode = View.Info.FindChildNode(RootSettingsNode);
            if (dictionaryNode != null) {
                foreach (DictionaryNode childNode in dictionaryNode.ChildNodes) {
                    object settings =View.Editor.Control.GetType().GetProperty(childNode.Name).GetValue(View.Editor.Control, null);
                    Delegate(childNode.Name, settings, dictionaryNode);
                }
            }
        }

        protected abstract string RootSettingsNode { get;  }

        void Delegate(string sourceChildNode, object settings, DictionaryNode aspxGridViewNode)
        {
            DictionaryNode dictionaryNode = aspxGridViewNode.FindChildNode(sourceChildNode);
            if (dictionaryNode != null)
                foreach (var dictionaryAttribute in dictionaryNode.Attributes.OfType<DictionaryAttribute>()){
                    var attributeValue = dictionaryNode.GetAttributeValue(dictionaryAttribute.Name);
                    DictionaryAttribute attribute = dictionaryAttribute;
                    var single = settings.GetType().GetProperties().Where(propertyInfo => propertyInfo.Name == attribute.Name).Single();
                    if (!string.IsNullOrEmpty(attributeValue)){
                        var value = Getvalue(single, attributeValue, settings);
                        single.SetValue(settings, value, null);
                    }
                }
        }

    }
}