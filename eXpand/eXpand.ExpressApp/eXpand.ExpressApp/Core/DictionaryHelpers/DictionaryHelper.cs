using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;
using System.Linq;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class DictionaryHelper
    {
        public string GetAspectFromXml(List<string> aspects, string xml)
        {
            aspects = aspects.OrderBy(s => s).ToList();

            xml = xml.Replace("&#165;", "¥");
            xml = removeSpaces(xml);
            string defaultAspectValuesWhenNoOtherAspectExist = Regex.Replace(xml, "\":([^\"\xA5]*)\"", "\"$1\"");
            string removedAspectsWithNoDefaultAspects = defaultAspectValuesWhenNoOtherAspectExist;
            if (!string.IsNullOrEmpty(aspects[0]))
            {
                string defaultAspectWhenOtherAspectExists = Regex.Replace(defaultAspectValuesWhenNoOtherAspectExist, @""":([^""\xA5]*)\xA5" + aspects[0] + @":([^""]*)""", "\"$1\"");
                removedAspectsWithNoDefaultAspects = defaultAspectWhenOtherAspectExists;
                foreach (var aspect in aspects)
                {
                    removedAspectsWithNoDefaultAspects = removeAttributesWithNoDefaultValue(aspect, removedAspectsWithNoDefaultAspects);
                }
            }
            return removedAspectsWithNoDefaultAspects;
        }

        private string removeSpaces(string aspects){
            return aspects.Replace(" >",">");
        }

        private string removeAttributesWithNoDefaultValue(string aspect,string value){
            return Regex.Replace(value, "( [^=\"]*=\"" +aspect+ ":([^\"]*)\")", "");
        }

        private static ICollection<IModelMember> GetCustomFields(IModelApplication model)
        {
            var result = new List<IModelMember>();
            foreach (IModelMember node in model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelBOModelRuntimeMember>().Where(member => member.IsRuntimeMember))
            {
                result.Add(node);
            }

            return result;
        }

        public static void AddFields(IModelApplication model, XPDictionary dictionary)
        {
            foreach (IModelMember customFieldInfo in GetCustomFields(model))
                try
                {
                    Type classType = customFieldInfo.ModelClass.TypeInfo.Type;
                    var typeInfo = dictionary.GetClassInfo(classType);
                    lock (typeInfo)
                    {
                        if (typeInfo.FindMember(customFieldInfo.MemberInfo.Name) == null)
                        {
                            XPCustomMemberInfo memberInfo = typeInfo.CreateMember(customFieldInfo.Name, customFieldInfo.MemberInfo.MemberType);
                            if (customFieldInfo.Size != 0)
                                memberInfo.AddAttribute(new DevExpress.Xpo.SizeAttribute(customFieldInfo.Size));

                            XafTypesInfo.Instance.RefreshInfo(classType);
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(
                            ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
                            customFieldInfo.MemberInfo.MemberType,
                            ((IModelClass)customFieldInfo.Parent).Name,
                            customFieldInfo.Name,
                            exception.Message));
                }
        }

    }
}