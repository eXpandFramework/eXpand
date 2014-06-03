using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.NH.Core;
using System.ComponentModel;

namespace Xpand.ExpressApp.NH
{
    public class NHEntityStore : ReflectionTypeInfoSource, IEntityStore, ITypeInfoSource
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

        public new void EnumMembers(TypeInfo info, EnumMembersHandler handler)
        {
            foreach (var property in GetPropertyDescriptors(info.Type))
                handler(property, property.Name);
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
            typeInfo.IsPersistent = Metadata.Any(tm => tm.Type == type);

            typeInfo.Refresh();
            typeInfo.RefreshMembers();
        }

        public override bool RegisterNewMember(ITypeInfo owner, XafMemberInfo memberInfo)
        {
            return base.RegisterNewMember(owner, memberInfo);
        }


        public override void InitTypeInfo(TypeInfo info)
        {
            base.InitTypeInfo(info);
        }

        public override void InitMemberInfo(object member, XafMemberInfo memberInfo)
        {
            base.InitMemberInfo(member, memberInfo);
            var typeMetadata = Metadata.FirstOrDefault(tm => memberInfo.Owner.Type.IsAssignableFrom(tm.Type));

            PropertyDescriptor descriptor = member as PropertyDescriptor;
            if (descriptor != null)
            {
                foreach (Attribute attr in descriptor.Attributes)
                    if (!memberInfo.Attributes.Any(a => a.GetType() == attr.GetType()))
                        memberInfo.AddAttribute(attr);
            }

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

                if (typeMetadata.RelationProperties.Contains(memberInfo.Name))
                {

                    if (memberInfo.ListElementType != null)
                    {
                        var associatedTypeInfo = typesInfo.FindTypeInfo(memberInfo.ListElementType);

                        memberInfo.AssociatedMemberInfo = (XafMemberInfo)associatedTypeInfo.Members.FirstOrDefault(m => m.MemberType == memberInfo.Owner.Type);
                        if (memberInfo.AssociatedMemberInfo != null)
                        {
                            memberInfo.IsAssociation = true;
                            memberInfo.AssociatedMemberInfo.IsReferenceToOwner = true;
                            memberInfo.AssociatedMemberOwner = associatedTypeInfo.Type;
                            memberInfo.IsAggregated = true;
                        }
                    }
                    else
                        memberInfo.IsAssociation = true;
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
