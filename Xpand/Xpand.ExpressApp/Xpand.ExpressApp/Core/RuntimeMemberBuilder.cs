using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Xpo;
using Xpand.Xpo;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.Core {
    public class RuntimeMemberBuilder {
        static readonly XPDictionary _dictionary = XpandModuleBase.Dictiorary;
        private static IEnumerable<IModelRuntimeMember> GetRuntimeMembers(IModelApplication model) {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelRuntimeMember>().Distinct();
        }

        public static void CreateRuntimeMembers(IModelApplication model) {
            using (var objectSpace = CreateObjectSpace()) {
                Tracing.Tracer.LogVerboseSubSeparator("RuntimeMembers Creation started");
                foreach (var modelRuntimeMember in GetRuntimeMembers(model))
                    CreateRuntimeMember(modelRuntimeMember,objectSpace as XPObjectSpace);
            }
            Tracing.Tracer.LogVerboseSubSeparator("RuntimeMembers Creation started");
        }

        static IObjectSpace CreateObjectSpace() {
            return XpandModuleBase.CompatibilityChecked?ApplicationHelper.Instance.Application.CreateObjectSpace():null;
        }

        static void CreateRuntimeMember(IModelRuntimeMember modelRuntimeMember, XPObjectSpace objectSpace) {
            try {
                Type classType = modelRuntimeMember.ModelClass.TypeInfo.Type;
                XPClassInfo xpClassInfo = _dictionary.GetClassInfo(classType);
                lock (xpClassInfo) {
                    var xpMemberInfo = xpClassInfo.FindMember(modelRuntimeMember.Name);
                    if (xpMemberInfo == null) {
                        var memberInfo = CreateMemberInfo(modelRuntimeMember, xpClassInfo);
                        AddAttributes(modelRuntimeMember, memberInfo);
                        XafTypesInfo.Instance.RefreshInfo(classType);
                    }
                    else {
                        if (objectSpace != null && !modelRuntimeMember.CreatedAtDesignTime) {
                            CreateColumn(objectSpace, xpMemberInfo, xpClassInfo);
                            modelRuntimeMember.CreatedAtDesignTime = true;
                            XafTypesInfo.Instance.RefreshInfo(classType);
                        }
                        UpdateMember(modelRuntimeMember, xpMemberInfo);
                    }
                }
            }
            catch (Exception exception) {
                throw new Exception(
                    ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(
                        ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
                        modelRuntimeMember.MemberInfo.MemberType,
                        ((IModelClass) modelRuntimeMember.Parent).Name,
                        modelRuntimeMember.Name,
                        exception.Message));
            }
        }

        static void CreateColumn(XPObjectSpace objectSpace, XPMemberInfo xpMemberInfo, XPClassInfo xpClassInfo) {
            var dbColumnType = DBColumn.GetColumnType(xpMemberInfo.StorageType);
            var column = new DBColumn(xpMemberInfo.Name, false, null, xpMemberInfo.MappingFieldSize, dbColumnType);
            var connectionProviderSql =((ConnectionProviderSql) ((BaseDataLayer) objectSpace.Session.DataLayer).ConnectionProvider);
            var table = xpClassInfo.Table;
            string textSql = String.Format(CultureInfo.InvariantCulture, "alter table {0} add {1} {2}",
                                           connectionProviderSql.FormatTableSafe(table),
                                           connectionProviderSql.FormatColumnSafe(column.Name),
                                           connectionProviderSql.GetSqlCreateColumnFullAttributes(table, column));
            connectionProviderSql.ExecSql(new Query(textSql));
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

        static XpandCustomMemberInfo CreateMemberInfo(IModelRuntimeMember modelMember, XPClassInfo xpClassInfo) {
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