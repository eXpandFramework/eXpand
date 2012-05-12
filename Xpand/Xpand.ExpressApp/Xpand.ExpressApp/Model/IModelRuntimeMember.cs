using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.Xpo;

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
    [DomainLogic(typeof(IModelRuntimeOrphanedColection))]
    public class ModelRuntimeOrphanedColectionDomainLogic {
        public static Type Get_Type(IModelRuntimeOrphanedColection modelRuntimeOrphanedColection) {
            if (modelRuntimeOrphanedColection.CollectionType != null) {
                var type = typeof(XPCollection<>).MakeGenericType(new[] { modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type });
                return type;
            }

            return null;
        }

        public static IMemberInfo Get_MemberInfo(IModelRuntimeOrphanedColection modelRuntimeOrphanedColection) {
            Guard.ArgumentNotNull(modelRuntimeOrphanedColection.ModelClass, "modelMember.ModelClass");
            Guard.ArgumentNotNull(modelRuntimeOrphanedColection.ModelClass.TypeInfo, "modelMember.ModelClass.TypeInfo");
            IMemberInfo info = modelRuntimeOrphanedColection.ModelClass.TypeInfo.FindMember(modelRuntimeOrphanedColection.Name);
            if (info == null) {
                if (!string.IsNullOrEmpty(modelRuntimeOrphanedColection.Name) && modelRuntimeOrphanedColection.Type != null) {
                    var xpClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(modelRuntimeOrphanedColection.ModelClass.TypeInfo.Type);
                    if (xpClassInfo.FindMember(modelRuntimeOrphanedColection.Name) == null) {
                        xpClassInfo.CreateCollection(modelRuntimeOrphanedColection.Name, modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type, modelRuntimeOrphanedColection.Criteria);
                        ((BaseInfo)modelRuntimeOrphanedColection.ModelClass.TypeInfo).Store.RefreshInfo(modelRuntimeOrphanedColection.ModelClass.TypeInfo.Type);
                        info = modelRuntimeOrphanedColection.ModelClass.TypeInfo.FindMember(modelRuntimeOrphanedColection.Name);
                    }
                }
            }

            if (info != null)
                modelRuntimeOrphanedColection.SetValue("MemberInfo", info);

            return info;
        }
    }

}