using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Xpo;
using Xpand.Xpo;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.Core {
    public class RuntimeMemberBuilder {
        static readonly XPDictionary _dictionary = XpandModuleBase.Dictiorary;
        private static IEnumerable<IModelRuntimeMember> GetCustomFields(IModelApplication model) {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelRuntimeMember>();
        }

        public static void AddFields(IModelApplication model) {
            AddRuntimeMembers(model);
        }

        static void AddRuntimeMembers(IModelApplication model) {
            foreach (IModelRuntimeMember modelRuntimeMember in GetCustomFields(model))
                try {
                    Type classType = modelRuntimeMember.ModelClass.TypeInfo.Type;
                    XPClassInfo typeInfo = _dictionary.GetClassInfo(classType);
                    lock (typeInfo) {
                        var xpMemberInfo = typeInfo.FindMember(modelRuntimeMember.Name);
                        if (xpMemberInfo == null) {
                            XpandCustomMemberInfo memberInfo = GetMemberInfo(modelRuntimeMember, typeInfo);
                            AddAttributes(modelRuntimeMember, memberInfo);
                            XafTypesInfo.Instance.RefreshInfo(classType);
                        } else {
                            UpdateMember(modelRuntimeMember, xpMemberInfo);
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

        static void UpdateMember(IModelRuntimeMember modelRuntimeMember, XPMemberInfo xpMemberInfo) {
            var modelRuntimeCalculatedMember = modelRuntimeMember as IModelRuntimeCalculatedMember;
            if (modelRuntimeCalculatedMember != null) {
                ((XpandCalcMemberInfo)xpMemberInfo).SetAliasExpression(modelRuntimeCalculatedMember.AliasExpression);
                XpandModuleBase.TypesInfo.RefreshInfo(xpMemberInfo.Owner.ClassType);
            }
        }

        static void AddAttributes(IModelRuntimeMember runtimeMember, XPCustomMemberInfo memberInfo) {
            if (runtimeMember.Size != 0)
                memberInfo.AddAttribute(new SizeAttribute(runtimeMember.Size));
            if (runtimeMember is IModelRuntimeNonPersistentMember && !(runtimeMember is IModelRuntimeCalculatedMember))
                memberInfo.AddAttribute(new NonPersistentAttribute());
        }

        static XpandCustomMemberInfo GetMemberInfo(IModelRuntimeMember modelMember, XPClassInfo xpClassInfo) {
            var calculatedMember = modelMember as IModelRuntimeCalculatedMember;
            if (calculatedMember != null)
                return xpClassInfo.CreateCalculabeMember(calculatedMember.Name, calculatedMember.Type, calculatedMember.AliasExpression);
            var member = modelMember as IModelRuntimeOrphanedColection;
            if (member != null) {
                var modelRuntimeOrphanedColection = member;
                return xpClassInfo.CreateCollection(member.Name, modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type,
                                                    modelRuntimeOrphanedColection.Criteria);
            }
            return xpClassInfo.CreateCustomMember(modelMember.Name, modelMember.Type, modelMember is IModelRuntimeNonPersistentMember);
        }
    }
}