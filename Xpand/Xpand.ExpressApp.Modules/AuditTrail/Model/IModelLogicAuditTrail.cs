using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.AuditTrail.Model {
    public interface IModelLogicAuditTrail : IModelNode {
        bool AuditSystemChanges { get; set; }
        IModelAuditTrailLogicRules Rules { get; }
        IModelViewContextsGroup ViewContextsGroup { get; }
        IModelFrameTemplateContextsGroup FrameTemplateContextsGroup { get; }
        IModelAuditTrailMembersContextGroup AuditTrailMembersContextGroup { get; }
    }

    public interface IModelAuditTrailMembersContextGroup : IModelNode, IModelList<IModelAuditTrailMembersContexts> {
    }

    public interface IModelAuditTrailMembersContexts : IModelNode, IModelList<IModelAuditTrailMembersContext> {
        
    }
    [KeyProperty("ClassName")]
    [ModelDisplayName("Class")]
    public interface IModelAuditTrailMembersContext:IModelNode {
        [Required]
        [ModelPersistentName("ClassName")]
        [DataSourceProperty("Application.BOModel")]
        IModelClass ModelClass { get; set; }
        [Browsable(false)]
        string ClassName { get; set; }

        IModelAuditTrailMembersContextMembers Members { get; }
    }

    [DomainLogic(typeof(IModelAuditTrailMembersContext))]
    public class ModelAuditTrailMembersContextDomainLogic {
        public static IModelClass Get_ModelClass(IModelAuditTrailMembersContext creatableItem) {
            return creatableItem.Application.BOModel[creatableItem.ClassName];
        }
        public static void Set_ModelClass(IModelAuditTrailMembersContext creatableItem, IModelClass modelClass) {
            creatableItem.ClassName = modelClass.Name;
        }
    }

    public interface IModelAuditTrailMembersContextMembers : IModelNode, IModelList<IModelAuditTrailMembersContextMember> {
         
    }

    [KeyProperty("ModelMemberName")]
    [DisplayProperty("ModelMemberName")]
    [ModelDisplayName("Member")]
    public interface IModelAuditTrailMembersContextMember:IModelNode {
        [Required]
        [ModelPersistentName("ModelMemberName")]
        [DataSourceProperty("ModelMembers")]
        IModelMember ModelMember { get; set; }
        [Browsable(false)]
        string ModelMemberName { get; set; }
        [Browsable(false)]
        IModelList<IModelMember> ModelMembers { get; }
    }
    [DomainLogic(typeof(IModelAuditTrailMembersContextMember))]
    public class ModelAuditTrailMembersContextMemberDomainLogic {
        public static IModelList<IModelMember> Get_ModelMembers(IModelAuditTrailMembersContextMember contextMember) {
            return new CalculatedModelNodeList<IModelMember>(((IModelAuditTrailMembersContext)contextMember.Parent.Parent).ModelClass.OwnMembers);
        }

        public static IModelMember Get_ModelMember(IModelAuditTrailMembersContextMember contextMember) {
            return ((IModelAuditTrailMembersContext)contextMember.Parent.Parent).ModelClass.FindOwnMember(contextMember.ModelMemberName);
        }

        public static void Set_ModelMember(IModelAuditTrailMembersContextMember contextMember,IModelMember modelMember) {
            contextMember.ModelMemberName=modelMember.Name;
        }
    }
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelAuditTrailLogicRules : IModelNode, IModelList<IModelAuditTrailRule> {
    }

}