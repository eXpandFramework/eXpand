using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.Xpo;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Xpo.MetaData;

namespace Xpand.ExpressApp.Model {
    public interface IModelRuntimeMember : IModelMemberEx {
    }
    public interface IModelRuntimeCalculatedColumn : IModelColumn {
        [ModelBrowsable(typeof(NotVisibileCalculator))]
        [RefreshProperties(RefreshProperties.All)]
        [Category("Data")]
        new string PropertyName { get; set; }
        [Category("Appearance")]
        [DataSourceProperty("PropertyEditorsType")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [ModelBrowsable(typeof(NotVisibileCalculator))]
        new Type PropertyEditorType { get; set; }
        [Required]
        string CalcPropertyName { get; set; }
    }
    [DomainLogic(typeof(IModelRuntimeCalculatedColumn))]
    public class ModelRuntimeCalculatedColumnLogic {
        public static string Get_PropertyName(IModelRuntimeCalculatedColumn runtimeCalculatedColumn) {
            if (DesignerOnlyCalculator.IsRunFromDesigner)
                return ((IModelListView)runtimeCalculatedColumn.Parent.Parent).ModelClass.AllMembers.First().Name;
            return runtimeCalculatedColumn.CalcPropertyName;
        }
    }

    public class NotVisibileCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return false;
        }
    }
    public class RuntimeOnlyCalculator : IModelIsVisible {
        public bool IsVisible(IModelNode node, string propertyName) {
            return !DesignerOnlyCalculator.IsRunFromDesigner;
        }
    }
    public class AlwaysEditableCalculator : IModelIsReadOnly {

        public bool IsReadOnly(IModelNode node, string propertyName) {
            return false;
        }

        public bool IsReadOnly(IModelNode node) {
            return false;
        }
    }

    public interface IModelRuntimeCalculatedMember : IModelRuntimeNonPersistentMember {
        [Required]
        [Category("eXpand")]
        [Description("Using an expression here it will force the creation of a calculated property insted of a normal one")]
        [CriteriaOptionsAttribute("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        string AliasExpression { get; set; }

        [Category("Appearance")]
        [DataSourceProperty("PropertyEditorsType")]
        [TypeConverter(typeof(StringToTypeConverterBase))]
        [ModelBrowsable(typeof(RuntimeOnlyCalculator))]
        new Type PropertyEditorType { get; set; }
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
            if (InterfaceBuilder.RuntimeMode) {
                CreateMemberInfo(runtimeCalculatedMember);
                return runtimeCalculatedMember.ModelClass.TypeInfo.FindMember(runtimeCalculatedMember.Name);
            }
            return ModelMemberLogic.Get_MemberInfo(runtimeCalculatedMember);
        }

        static void CreateMemberInfo(IModelRuntimeCalculatedMember runtimeCalculatedMember) {
            XPClassInfo classInfo = FindXPClassInfo(runtimeCalculatedMember);
            var xpandCalcMemberInfo = (XpandCalcMemberInfo)classInfo.FindMember(runtimeCalculatedMember.Name);
            if (ValidState(runtimeCalculatedMember, xpandCalcMemberInfo)) {
                var xpClassInfo = FindXPClassInfo(runtimeCalculatedMember);
                var calcMemberInfo = (XpandCalcMemberInfo)xpClassInfo.FindMember(runtimeCalculatedMember.Name);
                if (calcMemberInfo == null) {
                    xpandCalcMemberInfo = CreateXpandCalcMemberInfo(runtimeCalculatedMember, xpClassInfo);
                }
            }
            UpdateAliasExpression(xpandCalcMemberInfo, runtimeCalculatedMember.AliasExpression);
        }

        static void UpdateAliasExpression(XpandCalcMemberInfo memberInfo, string aliasExpression) {
            if (memberInfo != null) {
                memberInfo.SetAliasExpression(aliasExpression);
            }
        }

        static XpandCalcMemberInfo CreateXpandCalcMemberInfo(IModelRuntimeCalculatedMember runtimeCalculatedMember, XPClassInfo xpClassInfo) {
            var xpandCalcMemberInfo = new XpandCalcMemberInfo(xpClassInfo, runtimeCalculatedMember.Name, runtimeCalculatedMember.Type, runtimeCalculatedMember.AliasExpression);
            var typesInfo = ((BaseInfo)runtimeCalculatedMember.ModelClass.TypeInfo).Store;
            typesInfo.RefreshInfo(xpClassInfo.ClassType);
            return xpandCalcMemberInfo;
        }

        static bool ValidState(IModelRuntimeCalculatedMember runtimeCalculatedMember, XpandCalcMemberInfo memberInfo) {
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

        static XPClassInfo FindXPClassInfo(IModelRuntimeCalculatedMember runtimeCalculatedMember) {
            var typeInfo = (TypeInfo)runtimeCalculatedMember.ModelClass.TypeInfo;
            return typeInfo.IsInterface ? FindDCXPClassInfo(typeInfo) : XpandModuleBase.Dictiorary.GetClassInfo(typeInfo.Type);
        }

        static XPClassInfo FindDCXPClassInfo(TypeInfo typeInfo) {
            var xpoTypeInfoSource = XpandModuleBase.XpoTypeInfoSource;
            var generatedEntityType = xpoTypeInfoSource.GetGeneratedEntityType(typeInfo.Type);
            return generatedEntityType == null ? null : xpoTypeInfoSource.XPDictionary.GetClassInfo(generatedEntityType);
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