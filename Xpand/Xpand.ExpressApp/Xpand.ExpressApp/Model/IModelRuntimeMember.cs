using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Xpo;

namespace Xpand.ExpressApp.Model {
    public interface IModelRuntimeMember : IModelMember {
    }

    public interface IModelRuntimeCalculatedMember : IModelRuntimeNonPersistentMebmer {
        [Required]
        [Category("eXpand")]
        [Description("Using an expression here it will force the creation of a calculated property insted of a normal one")]
        string AliasExpression { get; set; }
    }

    public interface IModelRuntimeNonPersistentMebmer:IModelRuntimeMember {
    }

    public interface IModelRuntimeOrphanedColection : IModelRuntimeNonPersistentMebmer {
        [Category("eXpand")]
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
            if (modelRuntimeOrphanedColection.CollectionType!= null) {
                var type = typeof(XPCollection<>).MakeGenericType(new[] { modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type });
                return type;
            }
            
            return null;
        }
        public static Type Get_PropertyEditorType(IModelRuntimeOrphanedColection modelRuntimeOrphanedColection) {
            if (modelRuntimeOrphanedColection.Name != null&&modelRuntimeOrphanedColection.Type!=null) {
                var xpClassInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type);
                if (xpClassInfo.FindMember(modelRuntimeOrphanedColection.Name)==null) {
                    xpClassInfo.CreateCollection(modelRuntimeOrphanedColection.Name, modelRuntimeOrphanedColection.CollectionType.TypeInfo.Type,
                                                 modelRuntimeOrphanedColection.Criteria);
                    XafTypesInfo.Instance.RefreshInfo(xpClassInfo.ClassType);
                    var memberInfo = modelRuntimeOrphanedColection.CollectionType.TypeInfo.FindMember(modelRuntimeOrphanedColection.Name);
                    modelRuntimeOrphanedColection.SetValue("MemberInfo",memberInfo);
                }
            }
            return ModelModelMemberEditorTypeLogic.Get_PropertyEditorType(modelRuntimeOrphanedColection);
        }
    }
}