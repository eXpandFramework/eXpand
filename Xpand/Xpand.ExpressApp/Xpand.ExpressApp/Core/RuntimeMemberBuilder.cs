using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Model.RuntimeMembers;
using Xpand.ExpressApp.Model.RuntimeMembers.Collections;
using Xpand.ExpressApp.Xpo;
using Xpand.Xpo;
using Xpand.Xpo.MetaData;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Core {
    public class RuntimeMemberBuilder {
        static readonly XPDictionary _dictionary = XpandModuleBase.Dictiorary;
        private static IEnumerable<IModelMemberEx> GetMembersEx(IModelApplication model) {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelMemberEx>().Distinct();
        }

        public static void CreateRuntimeMembers(IModelApplication model) {
            using (var objectSpace = CreateObjectSpace()) {
                Tracing.Tracer.LogVerboseSubSeparator("RuntimeMembers Creation started");
                var throwOnDbColumnCreationError = ((IModelOptionRuntimeMembers)model.Options).ThrowOnDbColumnCreationError;
                foreach (var modelRuntimeMember in GetMembersEx(model))
                    CreateRuntimeMember(modelRuntimeMember, objectSpace as XPObjectSpace, throwOnDbColumnCreationError);
            }
            Tracing.Tracer.LogVerboseSubSeparator("RuntimeMembers Creation started");
        }

        static IObjectSpace CreateObjectSpace() {
            return XpandModuleBase.CompatibilityChecked?ApplicationHelper.Instance.Application.CreateObjectSpace():null;
        }

        static void CreateRuntimeMember(IModelMemberEx modelMemberEx, XPObjectSpace objectSpace, bool throwOnDbColumnCreationError) {
            try {
                Type classType = modelMemberEx.ModelClass.TypeInfo.Type;
                XPClassInfo xpClassInfo = _dictionary.GetClassInfo(classType);
                lock (xpClassInfo) {
                    var customMemberInfo = xpClassInfo.FindMember(modelMemberEx.Name) as XpandCustomMemberInfo;
                    if (customMemberInfo == null) {
                        customMemberInfo= CreateMemberInfo(modelMemberEx, xpClassInfo);
                        AddAttributes(modelMemberEx, customMemberInfo);
                        XafTypesInfo.Instance.RefreshInfo(classType);
                    }
                    else {
                        CreateColumn(modelMemberEx as IModelMemberPersistent, objectSpace, throwOnDbColumnCreationError, customMemberInfo, xpClassInfo, classType);
                        UpdateMember(modelMemberEx, customMemberInfo);
                    }
                }
            }
            catch (Exception exception) {
                throw new Exception(
                    ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(
                        ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
                        modelMemberEx.MemberInfo.MemberType,
                        ((IModelClass) modelMemberEx.Parent).Name,
                        modelMemberEx.Name,
                        exception.Message));
            }
        }

        static void CreateColumn(IModelMemberPersistent modelMemberPersistent, XPObjectSpace objectSpace, bool throwOnDbColumnCreationError,
                                 XpandCustomMemberInfo customMemberInfo, XPClassInfo xpClassInfo, Type classType) {
                                     if (CanCreateColumn(modelMemberPersistent, objectSpace)) {
                objectSpace.CreateColumn(customMemberInfo, xpClassInfo, throwOnDbColumnCreationError);
                modelMemberPersistent.DataStoreColumnCreated = true;
                XafTypesInfo.Instance.RefreshInfo(classType);
            }
        }

        static bool CanCreateColumn(IModelMemberPersistent modelMemberPersistent, XPObjectSpace objectSpace) {
            return  modelMemberPersistent != null &&
                                   (objectSpace != null && modelMemberPersistent.MemberInfo.MemberTypeInfo.IsPersistent &&
                                    !modelMemberPersistent.CreatedAtDesignTime &&
                                    !modelMemberPersistent.DataStoreColumnCreated);
        }


        static void UpdateMember(IModelMemberEx modelMemberEx, XPMemberInfo xpMemberInfo) {
            var modelRuntimeCalculatedMember = modelMemberEx as IModelMemberCalculated;
            if (modelRuntimeCalculatedMember != null) {
                ((XpandCalcMemberInfo)xpMemberInfo).SetAliasExpression(modelRuntimeCalculatedMember.AliasExpression);
                XpandModuleBase.TypesInfo.RefreshInfo(xpMemberInfo.Owner.ClassType);
            }
        }

        static void AddAttributes(IModelMemberEx modelMemberEx, XPCustomMemberInfo memberInfo) {
            if (modelMemberEx.Size != 0)
                memberInfo.AddAttribute(new SizeAttribute(modelMemberEx.Size));
            if (modelMemberEx is IModelMemberNonPersistent && !(modelMemberEx is IModelMemberCalculated))
                memberInfo.AddAttribute(new NonPersistentAttribute());
        }

        static XpandCustomMemberInfo CreateMemberInfo(IModelMemberEx modelMemberEx, XPClassInfo xpClassInfo) {
            var calculatedMember = modelMemberEx as IModelMemberCalculated;
            if (calculatedMember != null)
                return xpClassInfo.CreateCalculabeMember(calculatedMember.Name, calculatedMember.Type, calculatedMember.AliasExpression);
            var member = modelMemberEx as IModelMemberOrphanedColection;
            if (member != null) {
                var modelRuntimeOrphanedColection = member;
                return xpClassInfo.CreateCollection(member.Name, modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type,
                                                    modelRuntimeOrphanedColection.Criteria);
            }
            return xpClassInfo.CreateCustomMember(modelMemberEx.Name, modelMemberEx.Type, modelMemberEx is IModelMemberNonPersistent);
        }
    }
}