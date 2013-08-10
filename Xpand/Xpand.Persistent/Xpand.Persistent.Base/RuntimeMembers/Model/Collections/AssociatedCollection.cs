using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Linq;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.Persistent.Base.RuntimeMembers.Model.Collections {
    [ModelDisplayName("AssociatedCollection")]
    public interface IModelMemberOneToManyCollection : IModelMemberColection, IModelMemberDataStoreForeignKeyCreated {

        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [Required]
        [DataSourceProperty("AssociatedMembers")]
        IModelMemberDataStoreForeignKeyCreated AssociatedMember { get; set; }
        [Browsable(false)]
        IModelList<IModelMemberDataStoreForeignKeyCreated> AssociatedMembers { get; }
        [Required]
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        string AssociationName { get; set; }
    }

    public class ModelReadOnlDataStoreForeignKeyCreatedyCalculator:IModelIsReadOnly {
        public bool IsReadOnly(IModelNode node, string propertyName) {
            return ((IModelMemberOneToManyCollection)node).DataStoreForeignKeyCreated;
        }

        public bool IsReadOnly(IModelNode node) {
            throw new NotImplementedException();
        }
    }

    [DomainLogic(typeof(IModelMemberOneToManyCollection))]
    public class ModelMemberOneToManyDomainLogic:ModelMemberExDomainLogicBase<IModelMemberOneToManyCollection> {
        public static string Get_AssociationName(IModelMemberOneToManyCollection modelMemberOneToManyCollection) {
            if (modelMemberOneToManyCollection.AssociatedMember!=null)
                return String.Format("{0}-{1}s", modelMemberOneToManyCollection.ModelClass.Name, modelMemberOneToManyCollection.AssociatedMember.Name);
            return modelMemberOneToManyCollection.CollectionType==null ? String.Format("{0}-", modelMemberOneToManyCollection.ModelClass.Name) : String.Empty;
        }
        public static IModelList<IModelMemberDataStoreForeignKeyCreated> Get_AssociatedMembers(IModelMemberOneToManyCollection modelMemberOneToManyCollection) {
            if (modelMemberOneToManyCollection.DataStoreForeignKeyCreated)
                return new CalculatedModelNodeList<IModelMemberDataStoreForeignKeyCreated>(modelMemberOneToManyCollection.CollectionType.AllMembers.Cast<IModelMemberDataStoreForeignKeyCreated>());
            return modelMemberOneToManyCollection.CollectionType != null ?
                new CalculatedModelNodeList<IModelMemberDataStoreForeignKeyCreated>(modelMemberOneToManyCollection.CollectionType.AllMembers.Cast<IModelMemberDataStoreForeignKeyCreated>().Where(member => CanBeLinked(member, modelMemberOneToManyCollection.ModelClass))) :
                new CalculatedModelNodeList<IModelMemberDataStoreForeignKeyCreated>();
        }


        static bool CanBeLinked(IModelMemberDataStoreForeignKeyCreated modelMember, IModelClass modelClass) {
            return modelMember.MemberInfo != null && modelMember.MemberInfo.IsPersistent &&
                   !modelMember.MemberInfo.IsReadOnly && (modelMember.MemberInfo.FindAttribute<AssociationAttribute>() == null)
                   &&modelMember.MemberInfo.MemberTypeInfo==modelClass.TypeInfo;
        }

        public static IMemberInfo Get_MemberInfo(IModelMemberOneToManyCollection modelMemberOneToManyCollection) {
            return GetMemberInfo(modelMemberOneToManyCollection, CreateCollectionMemberInfo,
                collection => collection.AssociatedMember!=null && !string.IsNullOrEmpty(collection.AssociationName));
        }

        private static void CreateCollectionMemberInfo(IModelMemberOneToManyCollection collection,XPClassInfo info) {
            var associationAttribute = new AssociationAttribute(collection.AssociationName,collection.CollectionType.TypeInfo.Type);
            info.CreateCollection(collection.Name, collection.CollectionType.TypeInfo.Type, null, associationAttribute);
            var associatedClassInfo = info.Dictionary.GetClassInfo(collection.AssociatedMember.ModelClass.TypeInfo.Type);
            var associatedMemberInfo = associatedClassInfo.FindMember(collection.AssociatedMember.Name);
            associatedMemberInfo.AddAttribute(associationAttribute);
            ((BaseInfo)collection.ModelClass.TypeInfo).Store.RefreshInfo(collection.AssociatedMember.ModelClass.TypeInfo);
        }
    }
    [ModelAbstractClass]
    public interface IModelMemberColection : IModelMemberNonPersistent {
        [Category(ModelMemberExDomainLogic.AttributesCategory)]
        [Required]
        [DataSourceProperty("Application.BOModel")]
        [RefreshProperties(RefreshProperties.All)]
        IModelClass CollectionType { get; set; }
    }
    [DomainLogic(typeof(IModelMemberColection))]
    public class ModelMemberCollectionDomainLogic {
        public static Type Get_Type(IModelMemberColection orphanedColection) {
            return orphanedColection.CollectionType != null ? typeof(XPCollection<>).MakeGenericType(new[] { orphanedColection.CollectionType.TypeInfo.Type }) : null;
        }

    }
}
