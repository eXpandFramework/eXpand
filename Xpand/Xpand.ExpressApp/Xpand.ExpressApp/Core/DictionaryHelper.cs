using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Core
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

        private string removeSpaces(string aspects)
        {
            return aspects.Replace(" >", ">");
        }

        private string removeAttributesWithNoDefaultValue(string aspect, string value)
        {
            return Regex.Replace(value, "( [^=\"]*=\"" + aspect + ":([^\"]*)\")", "");
        }

        private static IEnumerable<IModelMember> GetCustomFields(IModelApplication model)
        {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelBOModelRuntimeMember>().Where(member => member.IsRuntimeMember).Cast<IModelMember>().ToList();
        }

        public static void AddFields(IModelApplication model, XPDictionary dictionary)
        {
            foreach (IModelMember modelMember in GetCustomFields(model))
                try
                {
                    Type classType = modelMember.ModelClass.TypeInfo.Type;
                    XPClassInfo typeInfo = dictionary.GetClassInfo(classType);
                    lock (typeInfo)
                    {
                        if (typeInfo.FindMember(modelMember.Name) == null)
                        {
                            XPCustomMemberInfo memberInfo = typeInfo.CreateMember(modelMember.Name, modelMember.Type);
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