using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Xpo;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.Model {
    public interface IModelRuntimeMember : IModelMemberEx {
    }

    public interface IModelRuntimeCalculatedMember : IModelRuntimeNonPersistentMember {
        [Required]
        [Category("eXpand")]
        [Description("Using an expression here it will force the creation of a calculated property insted of a normal one")]
        [CriteriaOptionsAttribute("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        string AliasExpression { get; set; }
    }

    public interface IModelRuntimeNonPersistentMember : IModelRuntimeMember {
    }

    public interface IModelRuntimeOrphanedColection : IModelRuntimeNonPersistentMember {
        [Category("eXpand")]
        [CriteriaOptionsAttribute("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        string Criteria { get; set; }
        [Category("eXpand")]
        [Required]
        [DataSourceProperty("Application.BOModel")]
        [RefreshProperties(RefreshProperties.All)]
        IModelClass CollectionType { get; set; }
    }

    [DomainLogic(typeof(IModelRuntimeCalculatedMember))]
    public class ModelRuntimeCalculatedMemberLogic {
        public static IMemberInfo Get_MemberInfo(IModelRuntimeCalculatedMember runtimeCalculatedMember) {
            var memberInfo = runtimeCalculatedMember.ModelClass.TypeInfo.FindMember(runtimeCalculatedMember.Name) as XafMemberInfo;
            if (ValidState(runtimeCalculatedMember, memberInfo)) {
                var xpClassInfo = XpClassInfo(runtimeCalculatedMember);
                var calcMemberInfo = (XpandCalcMemberInfo)xpClassInfo.FindMember(runtimeCalculatedMember.Name);
                if (calcMemberInfo == null) {
                    memberInfo = MemberInfo(runtimeCalculatedMember, xpClassInfo);
                }
            }
            UpdateAliasExpression(memberInfo, runtimeCalculatedMember.AliasExpression);
            return memberInfo;
        }

        static void UpdateAliasExpression(XafMemberInfo memberInfo, string aliasExpression) {
            if (memberInfo != null) {
                memberInfo.RemoveAttributes<PersistentAliasAttribute>();
                memberInfo.AddAttribute(new PersistentAliasAttribute(aliasExpression));
            }
        }

        static XafMemberInfo MemberInfo(IModelRuntimeCalculatedMember runtimeCalculatedMember, XPClassInfo xpClassInfo) {
            new XpandCalcMemberInfo(xpClassInfo, runtimeCalculatedMember.Name, runtimeCalculatedMember.Type, runtimeCalculatedMember.AliasExpression);
            ((BaseInfo)runtimeCalculatedMember.ModelClass.TypeInfo).Store.RefreshInfo(runtimeCalculatedMember.ModelClass.TypeInfo.Type);
            return runtimeCalculatedMember.ModelClass.TypeInfo.FindMember(runtimeCalculatedMember.Name) as XafMemberInfo;
        }

        static bool ValidState(IModelRuntimeCalculatedMember runtimeCalculatedMember, IMemberInfo memberInfo) {
            if (CheckTag(runtimeCalculatedMember)) return false;
            if (memberInfo == null && !string.IsNullOrEmpty(runtimeCalculatedMember.Name)) {
                runtimeCalculatedMember.Tag = true;
                if (runtimeCalculatedMember.Type != null && !string.IsNullOrEmpty(runtimeCalculatedMember.AliasExpression)) {
                    runtimeCalculatedMember.Tag = null;
                    return true;
                }
            }
            return false;
        }

        static XPClassInfo XpClassInfo(IModelRuntimeCalculatedMember runtimeCalculatedMember) {
            if (runtimeCalculatedMember.ModelClass.TypeInfo.IsInterface)
                return XpandModuleBase.Dictiorary.GetClassInfo(runtimeCalculatedMember.ModelClass.TypeInfo.Type);
        }

        static bool CheckTag(IModelRuntimeCalculatedMember runtimeCalculatedMember) {
            if (Equals(true, runtimeCalculatedMember.Tag)) {
                runtimeCalculatedMember.Tag = null;
                return true;
            }
            return false;
        }
    }

    [DomainLogic(typeof(IModelRuntimeOrphanedColection))]
    public class ModelRuntimeOrphanedColectionDomainLogic {
        public static Type Get_Type(IModelRuntimeOrphanedColection modelRuntimeOrphanedColection) {
            return modelRuntimeOrphanedColection.CollectionType != null ? typeof(XPCollection<>).MakeGenericType(new[] { modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type }) : null;
        }

        public static IMemberInfo Get_MemberInfo(IModelRuntimeOrphanedColection modelRuntimeOrphanedColection) {
            Guard.ArgumentNotNull(modelRuntimeOrphanedColection.ModelClass, "modelMember.ModelClass");
            Guard.ArgumentNotNull(modelRuntimeOrphanedColection.ModelClass.TypeInfo, "modelMember.ModelClass.TypeInfo");
            IMemberInfo info = modelRuntimeOrphanedColection.ModelClass.TypeInfo.FindMember(modelRuntimeOrphanedColection.Name);
            if (info == null) {
                if (!string.IsNullOrEmpty(modelRuntimeOrphanedColection.Name) && modelRuntimeOrphanedColection.Type != null) {
                    var xpClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(modelRuntimeOrphanedColection.ModelClass.TypeInfo.Type);
                    if (xpClassInfo.FindMember(modelRuntimeOrphanedColection.Name) == null) {
                        info = MemberInfo(modelRuntimeOrphanedColection, xpClassInfo);
                    }
                }
            }
            if (info != null)
                modelRuntimeOrphanedColection.SetValue("MemberInfo", info);
            return info;
        }

        static IMemberInfo MemberInfo(IModelRuntimeOrphanedColection modelRuntimeOrphanedColection, XPClassInfo xpClassInfo) {
            xpClassInfo.CreateCollection(modelRuntimeOrphanedColection.Name, modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type, modelRuntimeOrphanedColection.Criteria);
            ((BaseInfo)modelRuntimeOrphanedColection.ModelClass.TypeInfo).Store.RefreshInfo(modelRuntimeOrphanedColection.ModelClass.TypeInfo.Type);
            return modelRuntimeOrphanedColection.ModelClass.TypeInfo.FindMember(modelRuntimeOrphanedColection.Name);
        }
    }

}