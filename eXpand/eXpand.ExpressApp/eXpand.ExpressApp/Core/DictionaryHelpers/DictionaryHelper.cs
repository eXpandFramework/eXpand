using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
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
                removedAspectsWithNoDefaultAspects = aspects.Aggregate(defaultAspectWhenOtherAspectExists, (current, aspect) => removeAttributesWithNoDefaultValue(aspect, current));
            }
            return removedAspectsWithNoDefaultAspects;
        }

        private string removeSpaces(string aspects){
            return aspects.Replace(" >",">");
        }

        private string removeAttributesWithNoDefaultValue(string aspect,string value){
            return Regex.Replace(value, "( [^=\"]*=\"" +aspect+ ":([^\"]*)\")", "");
        }

        private static IEnumerable<IModelMember> GetCustomFields(IModelApplication model)
        {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).Where(member => member.MemberInfo!=null);
        }

        public static void AddFields(IModelApplication model, XPDictionary dictionary)
        {
            model.BOModel.SelectMany(modelClass => modelClass.AllMembers).Where(member => member.MemberInfo != null).
                ToList();
            return;
            foreach (IModelMember modelMember in GetCustomFields(model))
                try
                {
                    continue;
                    Type classType = ((IModelClass)modelMember.Parent.Parent).TypeInfo.Type;
                    XPClassInfo typeInfo = dictionary.GetClassInfo(classType);
                    lock (typeInfo)
                    {
                        if (typeInfo.FindMember(modelMember.Name) == null)
                        {
                            XPCustomMemberInfo memberInfo = typeInfo.CreateMember(modelMember.Name, modelMember.MemberInfo.MemberType);
                            if (modelMember.Size != 0)
                                memberInfo.AddAttribute(new DevExpress.Xpo.SizeAttribute(modelMember.Size));

                            XafTypesInfo.Instance.RefreshInfo(classType);
                        }
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(
                        ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(
                            ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
                            modelMember.MemberInfo.MemberType,
                            ((IModelClass)modelMember.Parent).Name,
                            modelMember.Name,
                            exception.Message));
                }
        }

    }
}