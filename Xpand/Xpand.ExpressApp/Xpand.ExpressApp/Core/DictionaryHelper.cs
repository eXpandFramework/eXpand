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
        private static IEnumerable<IModelMember> GetCustomFields(IModelApplication model) {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelBOModelRuntimeMember>().Where(member => member.IsRuntimeMember).Cast<IModelMember>();
        }

        public static void AddFields(IModelApplication model, XPDictionary dictionary) {
            foreach (IModelMember modelMember in GetCustomFields(model))
                try {
                    Type classType = modelMember.ModelClass.TypeInfo.Type;
                    XPClassInfo typeInfo = dictionary.GetClassInfo(classType);
                    lock (typeInfo) {
                        if (typeInfo.FindMember(modelMember.Name) == null) {
                            var aliasExpression = ((IModelBOModelRuntimeMember)modelMember).AliasExpression;
                            var memberInfo = string.IsNullOrEmpty(aliasExpression) ? 
                                typeInfo.CreateMember(modelMember.Name, modelMember.Type) :
                                typeInfo.CreateCalculabeMember(modelMember.Name, modelMember.Type, new Attribute[] { new PersistentAliasAttribute(aliasExpression) });
                            if (modelMember.Size != 0)
                                memberInfo.AddAttribute(new SizeAttribute(modelMember.Size));

                            XafTypesInfo.Instance.RefreshInfo(classType);
                        }
                    }
                } catch (Exception exception) {
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