using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Attributes;
using System.Linq;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.Enums;
using eXpand.Persistent.Base.General;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.SystemModule{
    public partial class ProvidedAssociationsController : Controller{
        public ProvidedAssociationsController(){
            InitializeComponent();
            RegisterActions(components);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo){
            base.CustomizeTypesInfo(typesInfo);
            IEnumerable<IMemberInfo> memberInfos =
                typesInfo.PersistentTypes.SelectMany(typeInfo => typeInfo.OwnMembers);
            foreach (var memberInfo in memberInfos.Where(memberInfo => memberInfo.FindAttribute<ProvidedAssociationAttribute>()!=null))
            {
                var providedAssociationAttribute = memberInfo.FindAttribute<ProvidedAssociationAttribute>();
                var associationAttribute = memberInfo.FindAttribute<AssociationAttribute>();
                if (associationAttribute== null)
                    throw new NullReferenceException(memberInfo+" has no association attribute");
                XPCustomMemberInfo customMemberInfo = GetCustomMemberInfo(memberInfo, providedAssociationAttribute,associationAttribute);
                addAttributes(memberInfo, associationAttribute, providedAssociationAttribute, customMemberInfo);
            }
        }

        private XPCustomMemberInfo GetCustomMemberInfo(IMemberInfo memberInfo, ProvidedAssociationAttribute providedAssociationAttribute, AssociationAttribute associationAttribute)
        {
            XPCustomMemberInfo customMemberInfo;
            var typeToCreateOn = getTypeToCreateOn(memberInfo,associationAttribute);
            if (typeToCreateOn== null)
                throw new NotImplementedException();
            var propertyName = providedAssociationAttribute.ProvidedPropertyName??typeToCreateOn.Name+"s";
            if (memberInfo.IsAssociation || (memberInfo.IsList&&providedAssociationAttribute.RelationType==RelationType.ManyToMany)){
                customMemberInfo = XafTypesInfo.Instance.CreateCollection(typeToCreateOn, memberInfo.Owner.Type,
                                                                          propertyName,XafTypesInfo.XpoTypeInfoSource.XPDictionary);
            }
            else
                customMemberInfo = XafTypesInfo.Instance.CreateMember(typeToCreateOn, memberInfo.Owner.Type,
                                                                      propertyName,
                                                                      XafTypesInfo.XpoTypeInfoSource.XPDictionary);
            return customMemberInfo;
        }

        private void addAttributes(IMemberInfo memberInfo, AssociationAttribute associationAttribute, ProvidedAssociationAttribute providedAssociationAttribute, XPCustomMemberInfo customMemberInfo)
        {
            customMemberInfo.AddAttribute(getAssociationAttribute(customMemberInfo.IsCollection, memberInfo.Owner.Type,associationAttribute.Name));
            if (providedAssociationAttribute.AttributesFactoryProperty != null){
                var property = memberInfo.Owner.Type.GetProperty(providedAssociationAttribute.AttributesFactoryProperty, BindingFlags.Static|BindingFlags.Public);
                if (property== null)
                    throw new NullReferenceException(string.Format("Static propeprty {0} not found at {1}", providedAssociationAttribute.GetPropertyInfo(x=>x.AttributesFactoryProperty).Name, memberInfo.Owner.Type.FullName));
                var values = (IEnumerable<Attribute>) property.GetValue(null, null);
                foreach (Attribute attribute in values){
                    customMemberInfo.AddAttribute(attribute);
                }
            }
        }

        private Type getTypeToCreateOn(IMemberInfo memberInfo, AssociationAttribute associationAttribute)
        {
            if (!memberInfo.MemberType.IsGenericType)
                return string.IsNullOrEmpty(associationAttribute.ElementTypeName) ? memberInfo.MemberType : Type.GetType(associationAttribute.ElementTypeName);
            return memberInfo.MemberType.GetGenericArguments()[0];
        }

        private Attribute getAssociationAttribute(bool isCollection,Type collectionType, string associationName)
        {
            return isCollection?new AssociationAttribute(associationName,collectionType):new AssociationAttribute(associationName);
        }
    }
}