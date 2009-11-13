using System;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;

namespace eXpand.ExpressApp.Core
{
    public static class DictionaryFactory
    {
        public static Dictionary Create(Type type, CultureInfo cultureInfo){
            return Create(type,cultureInfo.ToString());
        }

        private static Dictionary Create(Type type,string aspect){
            var dictionary = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), Schema.GetCommonSchema());
            new ApplicationNodeWrapper(dictionary).Load(type);
            var dictionary1 = new Dictionary(Schema.GetCommonSchema());
            dictionary1.AddAspect(aspect, dictionary.RootNode);
            return dictionary1;
        }

        public static Dictionary Create(Type type)
        {
            return Create(type,DictionaryAttribute.DefaultLanguage);
        }
    }
}
