using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.Metadata;

namespace eXpand.ExpressApp.Core.DictionaryHelpers
{
    public class DictionaryHelper
    {
        public const string IsRuntimeMember = "IsRuntimeMember";
        private static ICollection<PropertyInfoNodeWrapper> GetCustomFields(DictionaryNode applicationNode)
        {
            var result = new List<PropertyInfoNodeWrapper>();
            foreach (DictionaryNode node in applicationNode.GetChildNode(BOModelNodeWrapper.NodeName).GetChildNodes(PropertyInfoNodeWrapper.NodeName, IsRuntimeMember, bool.TrueString, true))
                result.Add(new PropertyInfoNodeWrapper(node));

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