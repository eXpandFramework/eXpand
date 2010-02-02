using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class SchemaHelper
    {
        public event EventHandler<AttibuteCreatedEventArgs> AttibuteCreating;
        #region OnAttibuteCreating
        /// <summary>
        /// Triggers the AttibuteCreating event.
        /// </summary>
        protected virtual void OnAttibuteCreating(AttibuteCreatedEventArgs ea)
        {
            if (AttibuteCreating != null)
                AttibuteCreating(null/*this*/, ea);
        }
        #endregion
        public string Serialize<T>(bool includeBaseTypes)
        {
            string schema = null;
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!includeBaseTypes&&typeof(T)!=property.DeclaringType)
                    continue;
                if (property.PropertyType == typeof (bool))
                    schema += GetAttribute("<Attribute Name=\"" + property.Name + "\" Choice=\"True,False\"/>");
                else if (typeof (Enum).IsAssignableFrom(property.PropertyType))
                    schema += GetAttribute("<Attribute Name=\"" + property.Name + "\" Choice=\"{" + property.PropertyType.FullName +
                                           "}\"/>");
                else
                    schema += GetAttribute("<Attribute Name=\"" + property.Name + "\"/>");
            }
            return schema;
        }

        private string GetAttribute(string s)
        {
            var args = new AttibuteCreatedEventArgs(s);
            OnAttibuteCreating(args);
            if (args.Handled)
                s=args.Attribute;
            return s;
        }

        public DictionaryNode CreateElement(ModelElement modelElement)
        {
            
//            var dictionaryNode=new DictionaryNode(ModelElement.Application.ToString());
            if (modelElement == ModelElement.Application)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"</Element>");
            if (modelElement == ModelElement.BOModel)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""BOModel"">" +
                                  @"	</Element>" +
                                  @"</Element>");
            if (modelElement == ModelElement.Views)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""Views"">" +
                                  @"	</Element>" +
                                  @"</Element>");
            if (modelElement == ModelElement.Class)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""BOModel"">" +
                                  @"		<Element Name=""Class"">" +
                                  @"		</Element>" +
                                  @"	</Element>" +
                                  @"</Element>");
            if (modelElement == ModelElement.ListView)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""Views"">" +
                                  @"		<Element Name=""ListView"">" +
                                  @"		</Element>" +
                                  @"	</Element>" +
                                  @"</Element>");
            if (modelElement == ModelElement.DetailView)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""Views"">" +
                                  @"		<Element Name=""DetailView"">" +
                                  @"		</Element>" +
                                  @"	</Element>" +
                                  @"</Element>");
            if (modelElement == ModelElement.DetailViewItems)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""Views"">" +
                                  @"		<Element Name=""DetailView"">" +
                                  @"		    <Element Name=""Items"">" +
                                  @"		    </Element>" +
                                  @"		</Element>" +
                                  @"	</Element>" +
                                  @"</Element>");
            if (modelElement == ModelElement.DetailViewPropertyEditors)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""Views"">" +
                                  @"		<Element Name=""DetailView"">" +
                                  @"		    <Element Name=""Items"">" +
                                  @"		        <Element Name=""PropertyEditor"">" +
                                  @"		        </Element>" +
                                  @"		    </Element>" +
                                  @"		</Element>" +
                                  @"	</Element>" +
                                  @"</Element>");
            if (modelElement == ModelElement.Member)
                return new DictionaryXmlReader().ReadFromString(
                                  @"<?xml version=""1.0""?>" +
                                  @"<Element Name=""Application"">" +
                                  @"	<Element Name=""BOModel"">" +
                                  @"		<Element Name=""Class"">" +
                                  @"			<Element Name=""Member"">" +
                                  @"			</Element>" +
                                  @"		</Element>" +
                                  @"	</Element>" +
                                  @"</Element>");


            throw new NotImplementedException(modelElement.ToString());
        }

        public DictionaryNode InjectAttribute(string name, Type choiceEnumType,ModelElement element) {
            return Inject(@"<Attribute Name=""" + name + @""" Choice=""{" + choiceEnumType.FullName + @"}""/>",element);
        }
        public DictionaryNode Inject(string injectString, ModelElement element)
        {
            DictionaryNode node = CreateElement(element);

            string path = null;
            string name = element.ToString();
            switch (element){
                case ModelElement.Application: {
                    node.AddChildNode(new DictionaryXmlReader().ReadFromString(injectString));
                    return node;
                }
                case ModelElement.ListView:
                case ModelElement.DetailView:
                case ModelElement.Class:
                    path = @"Element\Element";
                    break;
                case ModelElement.DetailViewItems:
                    name = "Items";
                    path = @"Element\Element\Element";
                    break;
                case ModelElement.DetailViewPropertyEditors:
                    name = "PropertyEditor";
                    path = @"Element\Element\Element\Element";
                    break;
            }
            var dictionaryElement = (DictionaryNode)node.FindChildElementByPath(path + @"[@Name='" + name + @"']");

            dictionaryElement.AddChildNode(new DictionaryXmlReader().ReadFromString(injectString));
            return node;
        }

    }
    [Flags]
    public enum ModelElement
    {
        Application,
        BOModel   ,
        Views,
        Class,
        DetailView,
        Member,
        ListView,
        DetailViewItems,
        DetailViewPropertyEditors
    }
    

    public class AttibuteCreatedEventArgs : HandledEventArgs
    {
        public string Attribute { get; set; }

        public AttibuteCreatedEventArgs(string attribute)
        {
            Attribute = attribute;
        }

        public void AddTag(string tag)
        {
            Handled = true;
            Attribute = Attribute.Replace("/>"," "+ tag + "/>");
        }
    }
}