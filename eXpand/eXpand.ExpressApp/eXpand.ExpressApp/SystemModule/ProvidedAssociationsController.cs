using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using System.Linq;
using eXpand.ExpressApp.Core;
using eXpand.Persistent.Base.General;

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
            foreach (var memberInfo in memberInfos.Where(memberInfo => memberInfo.FindAttribute<ProvidedAssociationAttribute>()!=null)){
                var providedAssociationAttribute = memberInfo.FindAttribute<ProvidedAssociationAttribute>();
                var associationAttribute = memberInfo.FindAttribute<AssociationAttribute>();
                if (associationAttribute== null)
                    throw new NullReferenceException(memberInfo+" has no association attribute");
                CreateMemberInfo(memberInfo, providedAssociationAttribute,associationAttribute);
            }
        }

        private void CreateMemberInfo(IMemberInfo memberInfo, ProvidedAssociationAttribute providedAssociationAttribute, AssociationAttribute associationAttribute)
        {
            var typeToCreateOn = getTypeToCreateOn(memberInfo,associationAttribute);
            if (typeToCreateOn== null)
                throw new NotImplementedException();
            
            if (memberInfo.IsAssociation || (memberInfo.IsList&&providedAssociationAttribute.RelationType==RelationType.ManyToMany)){
                XafTypesInfo.Instance.CreateCollection(typeToCreateOn, memberInfo.Owner.Type, associationAttribute.Name,
                                                       providedAssociationAttribute.ProvidedPropertyName ??
                                                       memberInfo.Owner.Type.Name + "s",
                                                       XafTypesInfo.XpoTypeInfoSource.XPDictionary);
            }
            else {
                XafTypesInfo.Instance.CreateMember(typeToCreateOn, memberInfo.Owner.Type, associationAttribute.Name,
                                                   providedAssociationAttribute.ProvidedPropertyName ??
                                                   memberInfo.Owner.Type.Name,
                                                   XafTypesInfo.XpoTypeInfoSource.XPDictionary);
                
            }
            return;
        }

        private Type getTypeToCreateOn(IMemberInfo memberInfo, AssociationAttribute associationAttribute)
        {
            if (!memberInfo.MemberType.IsGenericType)
                return string.IsNullOrEmpty(associationAttribute.ElementTypeName) ? memberInfo.MemberType : Type.GetType(associationAttribute.ElementTypeName);
            return memberInfo.MemberType.GetGenericArguments()[0];
        }
    }
}