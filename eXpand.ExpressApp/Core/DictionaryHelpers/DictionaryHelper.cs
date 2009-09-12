using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using System.Linq;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class DictionaryHelper
    {
        public const string IsRuntimeMember = "IsRuntimeMember";
        

        public string GetAspectFromXml(List<string> aspects, string xml)
        {
            aspects = aspects.OrderBy(s => s).ToList();
            xml = xml.Replace("&#165;", "¥");
            string defaultAspectValuesWhenNoOtherAspectExist = Regex.Replace(xml, "\":([^\":]*)\"", "\"$1\"");
            string removedAspectsWithNoDefaultAspects = defaultAspectValuesWhenNoOtherAspectExist;
            if (!string.IsNullOrEmpty(aspects[0])){
                string defaultAspectWhenOtherAspectExists = Regex.Replace(defaultAspectValuesWhenNoOtherAspectExist, @""":([^""\xA5]*)\xA5" +aspects[0]+ @":([^""]*)""", "\"$1\"");
                removedAspectsWithNoDefaultAspects = defaultAspectWhenOtherAspectExists;
                foreach (var aspect in aspects){
                    removedAspectsWithNoDefaultAspects = removeAttributesWithNoDefaultValue(aspect, removedAspectsWithNoDefaultAspects);
                }
            }
            return removedAspectsWithNoDefaultAspects;
        }

        private string removeAttributesWithNoDefaultValue(string aspect,string value){
            return Regex.Replace(value, "( [^=\"]*=\"" +aspect+ ":([^\"]*)\")", "");
        }


        public string GetAspectFromXml(List<string> aspects, DictionaryNode dictionaryNode)
        {
            string xml1 = dictionaryNode.ToXml();
            return GetAspectFromXml(aspects, xml1);

        }


        private static ICollection<PropertyInfoNodeWrapper> GetCustomFields(DictionaryNode applicationNode)
        {
            var result = new List<PropertyInfoNodeWrapper>();
            foreach (DictionaryNode node in applicationNode.GetChildNode(BOModelNodeWrapper.NodeName).GetChildNodes(PropertyInfoNodeWrapper.NodeName, IsRuntimeMember, bool.TrueString, true))
            {
                result.Add(new PropertyInfoNodeWrapper(node));
            }

            return result;
        }

        public static void AddFields(DictionaryNode rootNode, XPDictionary dictionary)
        {
            foreach (PropertyInfoNodeWrapper customFieldInfo in GetCustomFields(rootNode))
                try
                {
                    Type classType = ReflectionHelper.GetType(customFieldInfo.Class.Name);
                    var typeInfo = ((TypeInfo)XafTypesInfo.Instance.FindTypeInfo(classType));
                    lock (typeInfo)
                    {
                        if (typeInfo.FindMember(customFieldInfo.Name) == null)
                        {
                            Type memberType = ReflectionHelper.GetType(customFieldInfo.Type);
                            IMemberInfo memberInfo = typeInfo.CreateMember(customFieldInfo.Name, memberType);
                            if (customFieldInfo.Size != 0)
                                memberInfo.AddAttribute(new DevExpress.Xpo.SizeAttribute(customFieldInfo.Size));
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(
                            ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
                            customFieldInfo.Type,
                            customFieldInfo.Class.Name,
                            customFieldInfo.Name,
                            exception.Message));
                }
        }

    }
}