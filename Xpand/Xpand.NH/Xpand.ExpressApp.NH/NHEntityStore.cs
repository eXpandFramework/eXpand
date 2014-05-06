using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.NH.Core;

namespace Xpand.ExpressApp.NH
{
    public class NHEntityStore : ReflectionTypeInfoSource, IEntityStore
    {
        private List<Type> types = new List<Type>();
        private readonly ITypesInfo typesInfo;
        private readonly IPersistenceManager persistenceManager;
        private IList<ITypeMetadata> metadata;

        public NHEntityStore(ITypesInfo typesInfo, IPersistenceManager persistenceManager)
        {
            Guard.ArgumentNotNull(typesInfo, "typesInfo");
            Guard.ArgumentNotNull(persistenceManager, "persistenceManager");

            this.typesInfo = typesInfo;
            this.persistenceManager = persistenceManager;
        }


        private IList<ITypeMetadata> Metadata
        {
            get
            {
                if (metadata == null)
                {
                    metadata = persistenceManager.GetMetadata();
                }

                return metadata;
            }
        }

        public bool CanRegister(Type type)
        {
            return true;
        }


        public void RegisterEntity(Type type)
        {
            types.Add(type);
            TypeInfo typeInfo = (TypeInfo)typesInfo.FindTypeInfo(type);
            typeInfo.Source = this;
            typeInfo.IsPersistent = true;
            typeInfo.Refresh();
            typeInfo.RefreshMembers();
        }

        public override bool RegisterNewMember(ITypeInfo owner, XafMemberInfo memberInfo)
        {
            return base.RegisterNewMember(owner, memberInfo);
        }

        public override void InitMemberInfo(object member, XafMemberInfo memberInfo)
        {
            base.InitMemberInfo(member, memberInfo);
            var typeMetadata = Metadata.FirstOrDefault(tm => tm.Type == memberInfo.Owner.Type);

            if (typeMetadata != null)
            {
                if (typeMetadata.KeyPropertyName == memberInfo.Name)
                {
                    memberInfo.IsKey = true;
                    memberInfo.AddAttribute(new VisibleInDetailViewAttribute(false));
                    memberInfo.AddAttribute(new VisibleInListViewAttribute(false));
                    memberInfo.Owner.KeyMember = memberInfo;
                    memberInfo.IsPersistent = true;
                }

                if (memberInfo.ListElementType != null)
                {
                    var associatedTypeInfo = typesInfo.FindTypeInfo(memberInfo.ListElementType);
                    memberInfo.IsAssociation = true;
                    //TODO: Throw appropriate exception, when Single() fails
                    memberInfo.AssociatedMemberInfo = (XafMemberInfo)associatedTypeInfo.Members.Single(m => m.MemberType == memberInfo.Owner.Type);
                    memberInfo.AssociatedMemberInfo.IsReferenceToOwner = true;
                    memberInfo.AssociatedMemberOwner = associatedTypeInfo.Type;
                    memberInfo.IsAggregated = true;
                }
                memberInfo.IsPersistent = !memberInfo.IsReadOnly;
            }
        }

        public IEnumerable<Type> RegisteredEntities
        {
            get { return types.AsReadOnly(); }
        }

        public void Reset()
        {
            types.Clear();
        }
    }
}
