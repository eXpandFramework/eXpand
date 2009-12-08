using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos {
    [DefaultProperty("AssociationName")]
    public class PersistentAssociationAttribute : PersistentAttributeInfo {
        private string _associationName;
        

        public PersistentAssociationAttribute(Session session) : base(session) {
        }

        public PersistentAssociationAttribute() {
        }
        #region IPersistentAssociationAttribute Members
        [VisibleInListView(true)]
        [RuleRequiredField(null,DefaultContexts.Save)]
        public string AssociationName {
            get { return _associationName; }
            set { SetPropertyValue("AssociationName", ref _associationName, value); }
        }
        
        

        private Type _elementType;
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(TypeValueConverter))]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type ElementType
        {
            get
            {
                return _elementType;
            }
            set {
                SetPropertyValue("ElementType", ref _elementType, value);
                if (value != null) _assemblyQualifiedName = value.AssemblyQualifiedName;
            }
        }

        private string _assemblyQualifiedName;
        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        [Size(SizeAttribute.Unlimited)]
        public string AssemblyQualifiedName
        {
            get
            {
                return _assemblyQualifiedName;
            }
            set
            {
                SetPropertyValue("AssemblyQualifiedName", ref _assemblyQualifiedName, value);
            }
        }
        

        public override AttributeInfo Create() {
            string typeName= Regex.Match(AssemblyQualifiedName, "([^,]*).*").Groups[1].Value;
            string withOutTypeName=Regex.Replace(AssemblyQualifiedName, "([^,]*), (.*)", "$2");
            string assemblyName = Regex.Match(withOutTypeName, "([^,]*).*").Groups[1].Value;
            var constructorInfo = typeof(AssociationAttribute).GetConstructor(new[] { typeof(string), typeof(string), typeof(string) });
            return new AttributeInfo(constructorInfo, AssociationName, assemblyName, typeName);
        }
        #endregion
    }
}