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
<<<<<<< HEAD

=======
>>>>>>> CodeDomApproachForWorldCreator
    public class PersistentReferenceMemberInfo : PersistentMemberInfo, IPersistentReferenceMemberInfo {
        public PersistentReferenceMemberInfo(Session session) : base(session) { }


        public PersistentReferenceMemberInfo(Session session,PersistentAssociationAttribute persistentAssociationAttribute) : base(session)
        {
            TypeAttributes.Add(persistentAssociationAttribute);
        }
<<<<<<< HEAD
        
        
=======


>>>>>>> CodeDomApproachForWorldCreator
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
<<<<<<< HEAD
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
=======
                _referenceTypeFullName = _referenceType != null ? _referenceType.FullName : null;
            }
        }
        private string _referenceTypeFullName;
        [Browsable(false)]
        public string ReferenceTypeFullName
        {
            get
            {
                return _referenceTypeFullName;
            }
            set
            {
                SetPropertyValue("ReferenceTypeFullName", ref _referenceTypeFullName, value);
            }
        }

>>>>>>> CodeDomApproachForWorldCreator
    }
}