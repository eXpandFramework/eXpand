using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Model;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Core {
    public class DictionaryHelper {
        private static IEnumerable<IModelRuntimeMember> GetCustomFields(IModelApplication model) {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelRuntimeMember>();
        }

        public static void AddFields(IModelApplication model, XPDictionary dictionary) {
            AddRuntimeMembers(model, dictionary);
        }

        static void AddRuntimeMembers(IModelApplication model, XPDictionary dictionary) {
            foreach (IModelRuntimeMember modelRuntimeMember in GetCustomFields(model))
                try {
                    Type classType = modelRuntimeMember.ModelClass.TypeInfo.Type;
                    XPClassInfo typeInfo = dictionary.GetClassInfo(classType);
                    lock (typeInfo) {
                        if (typeInfo.FindMember(modelRuntimeMember.Name) == null) {
                            XPCustomMemberInfo memberInfo = GetMemberInfo(modelRuntimeMember, typeInfo);
                            AddAttributes(modelRuntimeMember, memberInfo);
                            XafTypesInfo.Instance.RefreshInfo(classType);
                        }
                    }
                } catch (Exception exception) {
                    throw new Exception(
                        ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(
                            ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
                            modelRuntimeMember.MemberInfo.MemberType,
                            ((IModelClass)modelRuntimeMember.Parent).Name,
                            modelRuntimeMember.Name,
                            exception.Message));
                }
        }

        static void AddAttributes(IModelRuntimeMember runtimeMember, XPCustomMemberInfo memberInfo) {
            if (runtimeMember.Size != 0)
                memberInfo.AddAttribute(new SizeAttribute(runtimeMember.Size));
            if (runtimeMember.NonPersistent)
                memberInfo.AddAttribute(new NonPersistentAttribute());
        }

        static XPCustomMemberInfo GetMemberInfo(IModelRuntimeMember modelMember, XPClassInfo typeInfo) {
            return (modelMember is IModelCalculatedRuntimeMember)
                       ? typeInfo.CreateCalculabeMember(modelMember.Name,modelMember.Type,
                                                        new Attribute[] {new PersistentAliasAttribute(((IModelCalculatedRuntimeMember)modelMember).AliasExpression)})
                       : typeInfo.CreateMember(modelMember.Name,modelMember.Type);
        }
    }
}