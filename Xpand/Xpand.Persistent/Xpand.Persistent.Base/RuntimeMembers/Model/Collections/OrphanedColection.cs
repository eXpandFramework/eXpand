using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.Persistent.Base.RuntimeMembers.Model.Collections {

    [ModelDisplayName("OrphanedColection")]
    [ModelPersistentName("RuntimeOrphanedColection")]
    public interface IModelMemberOrphanedColection :  IModelMemberColection {
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [CriteriaOptions("CollectionType.TypeInfo")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(System.Drawing.Design.UITypeEditor))]
        string Criteria { get; set; }
    }
    [DomainLogic(typeof(IModelMemberOrphanedColection))]
    public class ModelMemberOrphanedColectionDomainLogic:ModelMemberExDomainLogicBase<IModelMemberOrphanedColection> {
        public static IMemberInfo Get_MemberInfo(IModelMemberOrphanedColection orphanedColection) {
            return GetMemberInfo(orphanedColection,
                (colection, info) => info.CreateCollection(colection.Name, colection.CollectionType.TypeInfo.Type, colection.Criteria),
                colection => colection.CollectionType!=null);
        }

    }

}