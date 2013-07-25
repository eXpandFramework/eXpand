using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Model.RuntimeMembers {
    [ModelPersistentName("RuntimeMember")]
    [ModelDisplayName("Persistent")]
    public interface IModelMemberPersistent : IModelMemberEx {
        
        [Browsable((false))]
        bool DataStoreColumnCreated { get; set; }
    }

    public interface IModelOptionRuntimeMembers {
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [Description("If set when creating Runtime members it will throw any SqlExecutionErrorException/ConstrainsVionationException")]
        bool ThrowOnDbColumnCreationError { get; set; }
    }

    [DomainLogic(typeof(IModelMemberPersistent))]
    public class ModelMemberPersistentDomainLogic : ModelMemberExDomainLogicBase<IModelMemberPersistent> {
        public static IMemberInfo Get_MemberInfo(IModelMemberPersistent modelRuntimeMember) {
            return GetMemberInfo(modelRuntimeMember, 
                (persistent, info) => info.CreateCustomMember(persistent.Name, persistent.Type, false),
                persistent => true);
        }

    }

}