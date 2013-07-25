using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Xpo;

namespace Xpand.ExpressApp.Model.RuntimeMembers.Collections {
    [ModelDisplayName("OrphanedColection")]
    [ModelPersistentName("RuntimeOrphanedColection")]
    public interface IModelMemberOrphanedColection : IModelMemberNonPersistent {
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [CriteriaOptions("ModelClass.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        string Criteria { get; set; }
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [Required]
        [DataSourceProperty("Application.BOModel")]
        [RefreshProperties(RefreshProperties.All)]
        IModelClass CollectionType { get; set; }
    }
    [DomainLogic(typeof(IModelMemberOrphanedColection))]
    public class ModelMemberOrphanedColectionDomainLogic:ModelMemberExDomainLogicBase<IModelMemberOrphanedColection> {
        public static IMemberInfo Get_MemberInfo(IModelMemberOrphanedColection orphanedColection) {
            return GetMemberInfo(orphanedColection,
                (colection, info) => info.CreateCollection(colection.Name, colection.CollectionType.TypeInfo.Type, colection.Criteria),
                colection => colection.CollectionType!=null);
        }

        public static Type Get_Type(IModelMemberOrphanedColection orphanedColection) {
            return orphanedColection.CollectionType != null ? typeof(XPCollection<>).MakeGenericType(new[] { orphanedColection.CollectionType.TypeInfo.Type }) : null;
        }
    }

}