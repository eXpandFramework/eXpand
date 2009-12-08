using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using System;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{

    public class PersistentReferenceMemberInfo : PersistentMemberInfo, IPersistentReferenceMemberInfo {
        public PersistentReferenceMemberInfo(Session session) : base(session) { }


        public PersistentReferenceMemberInfo(Session session,PersistentAssociationAttribute persistentAssociationAttribute) : base(session)
        {
            TypeAttributes.Add(persistentAssociationAttribute);
        }
        
        
        Type _referenceType;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(null,DefaultContexts.Save)]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ReferenceType
        {
            get { return _referenceType; }
            set {
                SetPropertyValue("ReferenceType", ref _referenceType, value);
                if (value != null) _referenceTypeAssemblyQualifiedName = value.AssemblyQualifiedName;
            }
        }

        private string _referenceTypeAssemblyQualifiedName;
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        [Size(SizeAttribute.Unlimited)]
        public string ReferenceTypeAssemblyQualifiedName
        {
            get
            {
                return _referenceTypeAssemblyQualifiedName;
            }
            set
            {
                SetPropertyValue("ReferenceTypeAssemblyQualifiedName", ref _referenceTypeAssemblyQualifiedName, value);
            }
        }
        Type IPersistentReferenceMemberInfo.ReferenceType
        {
            get { return ReferenceType; }
            set { ReferenceType = value; }
        }
    }
}