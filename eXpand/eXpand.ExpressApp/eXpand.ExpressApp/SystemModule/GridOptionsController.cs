using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using eXpand.Xpo;

namespace eXpand.ExpressApp.SystemModule {
    public abstract class GridOptionsController<ControlType,SettingsType>:ViewController<ListView>
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
        public override Schema GetSchema()
        {
            string schema = @"<Element Name=""Application"">
                    <Element Name=""Views"" >
                        <Element Name=""ListView"" >
                            <Element Name=""GridOptions"" >
                                " +GetGridSchema()+ @"
                            </Element>
                        </Element>
                    </Element>
                </Element>";

            return new Schema(new DictionaryXmlReader().ReadFromString(schema));
        }

        string GetGridSchema() {
            return typeof (ControlType).GetProperties().Where(propertyInfo =>typeof (SettingsType).IsAssignableFrom(propertyInfo.PropertyType))
                    .Aggregate<PropertyInfo, string>(null, (current, propertyInfo1) 
            => current + (@"<Element Name=""" + propertyInfo1.Name + @""" >
                        " + GetSettingsSchema(propertyInfo1.PropertyType) + @"
                       </Element>"));
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
            DictionaryNode dictionaryNode = View.Info.FindChildNode("GridOptions");
            if (dictionaryNode != null) {
                var propertyInfos = typeof(ControlType).GetProperties().Where(info1 => typeof(SettingsType).IsAssignableFrom(info1.PropertyType));
                foreach (DictionaryNode childNode in dictionaryNode.ChildNodes) {
                    DictionaryNode node = childNode;
                    var propertyInfo = propertyInfos.Where(info1 => info1.Name==node.Name).FirstOrDefault();
                    if (propertyInfo != null) {
                        object settings = propertyInfo.GetValue(GetControl(), null);
                        Delegate(childNode.Name, settings, dictionaryNode);
                    }
                }
            }
        }

        protected virtual object GetControl() {
            return View.Editor.Control;
        }


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