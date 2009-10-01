using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Enums;
using AggregatedAttribute=DevExpress.ExpressApp.DC.AggregatedAttribute;

namespace eXpand.ExpressApp.SystemModule{
    public partial class ProvideAssociationsController : Controller{
        private List<XPCustomMemberInfo> providedMembers;

        public ProvideAssociationsController(){
            InitializeComponent();
            RegisterActions(components);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo){
            base.CustomizeTypesInfo(typesInfo);

            ProvideDynamicAssociationsToDictionary(XafTypesInfo.XpoTypeInfoSource.XPDictionary);
        }

        public void ProvideDynamicAssociationsToDictionary(XPDictionary xpDictionary){
            providedMembers = new List<XPCustomMemberInfo>();

            foreach (XPClassInfo classInfo in xpDictionary.Classes){
                bool shouldRefresh = false;

                if (classInfo.HasAttribute(typeof (ProvidedMemberAttribute))){
                    shouldRefresh = true;
                    var attribute = (ProvidedMemberAttribute) classInfo.GetAttributeInfo(typeof (ProvidedMemberAttribute));
                    XPClassInfo provisionedClass = xpDictionary.GetClassInfo(attribute.ProvisionedClassType);
                    if (attribute.ProvisionedAttributes == null){
                        provisionedClass.CreateMember(attribute.ProvidedPropertyName, classInfo, new BrowsableAttribute(true));
                    }
                    else{
                        provisionedClass.CreateMember(attribute.ProvidedPropertyName, classInfo, attribute.ProvisionedAttributes);
                    }
                    XafTypesInfo.Instance.RefreshInfo(provisionedClass.ClassType);
                }

                foreach (XPMemberInfo memberInfo in classInfo.Members){
                    if (memberInfo.HasAttribute(typeof (ProvidedAssociationAttribute))){
                        shouldRefresh = true;
                        ProvidedAssociationProcessInfo token = CreateProvidingProcessToken(xpDictionary, memberInfo);
                        ProvideAssociation(token);
                        XafTypesInfo.Instance.RefreshInfo(token.ProviderClass.ClassType);
                    }
                }

                if (shouldRefresh){
                    XafTypesInfo.Instance.RefreshInfo(classInfo.ClassType);
                }
            }
        }

/*
        private void ProvideMember(ProvidedAssociationProcessInfo token) {
            if (token.ProviderClass.FindMember(token.ProvidedAccociationAttribute.ProvidedPropertyName) == null) {
                XPCustomMemberInfo providedMember = CreateProvidedMember(token);
                providedMembers.Add(providedMember);
            }
        }
*/

        private void ProvideAssociation(ProvidedAssociationProcessInfo token){
            if (token.ProviderClass.FindMember(token.ProvidedAccociationAttribute.ProvidedPropertyName) == null){
                XPCustomMemberInfo providedMember = CreateProvidedMember(token);
                providedMembers.Add(providedMember);
            }
        }

        private XPCustomMemberInfo CreateProvidedMember(ProvidedAssociationProcessInfo token){
            if (token.AssociationAttribute != null){
                XPCustomMemberInfo providedMember = token.ProviderClass.CreateMember(token.ProvidedAccociationAttribute.ProvidedPropertyName,
                                                                                     token.ProvidedPropertyType, token.ProvidedPropertyType.Equals(typeof (XPCollection)));

                var providedAssociation = new AssociationAttribute(token.AssociationAttribute.Name, token.ProviderMember.Owner.ClassType);
                providedMember.AddAttribute(providedAssociation);

                if (token.AggregatedAttribute != null){
                    providedMember.AddAttribute(token.AggregatedAttribute);
                }
                return providedMember;
            }
            else{
                XPCustomMemberInfo providedMember = token.ProviderClass.CreateMember(token.ProvidedAccociationAttribute.ProvidedPropertyName,
                                                                                     token.ProvidedPropertyType, token.ProvidedPropertyType.Equals(typeof (XPCollection)));
                throw new NotImplementedException();
                var providedAssociation = new AssociationAttribute(token.AssociationAttribute.Name, token.ProviderMember.Owner.ClassType);
                providedMember.AddAttribute(providedAssociation);

                if (token.AggregatedAttribute != null){
                    providedMember.AddAttribute(token.AggregatedAttribute);
                }
                return providedMember;
            }
        }

        private ProvidedAssociationProcessInfo CreateProvidingProcessToken(XPDictionary dictionary, XPClassInfo classInfo){
            var token = new ProvidedAssociationProcessInfo{
                                                              ProvidedMemberAttribute =
                                                                  (ProvidedMemberAttribute)
                                                                  classInfo.GetAttributeInfo(
                                                                      typeof (ProvidedMemberAttribute)),
                                                              ProviderClass = classInfo,
                                                              Dictionary = dictionary
                                                          };

            token.ProvidedPropertyType = token.ProviderClass.ClassType;
            return token;
        }

        private ProvidedAssociationProcessInfo CreateProvidingProcessToken(XPDictionary dictionary, XPMemberInfo memberInfo){
            var token = new ProvidedAssociationProcessInfo{
                                                              ProvidedAccociationAttribute =
                                                                  (ProvidedAssociationAttribute)
                                                                  memberInfo.GetAttributeInfo(
                                                                      typeof (ProvidedAssociationAttribute)),
                                                              AssociationAttribute =
                                                                  (AssociationAttribute)
                                                                  memberInfo.GetAttributeInfo(
                                                                      typeof (AssociationAttribute)),
                                                              AggregatedAttribute =
                                                                  (AggregatedAttribute)
                                                                  (memberInfo.HasAttribute(typeof (AggregatedAttribute))
                                                                       ? memberInfo.GetAttributeInfo(
                                                                             typeof (AggregatedAttribute))
                                                                       : null),
                                                              ProviderMember = memberInfo,
                                                              Dictionary = dictionary
                                                          };

            DiscoverProviderClass(token);
            DiscoverProvidedPropertyType(token);
            return token;
        }

        private void DiscoverProvidedPropertyType(ProvidedAssociationProcessInfo token){
            switch (token.ProvidedAccociationAttribute.ManyToManyInfoType){
                case ManyToManyInfoType.Undefined:
                    token.ProvidedPropertyType = token.ProviderMember.IsCollection
                                                     ? token.ProviderMember.Owner.ClassType
                                                     : typeof (XPCollection);
                    break;
                case ManyToManyInfoType.ManyToMany:
                    token.ProvidedPropertyType = typeof (XPCollection);
                    break;
                case ManyToManyInfoType.ManyToManyWithInfo:
                    token.ProvidedPropertyType = token.ProvidedAccociationAttribute.ProvidedAssociationMemberType;
                    break;
                default:
                    token.ProvidedPropertyType = null;
                    break;
            }
        }

        private void DiscoverProviderClass(ProvidedAssociationProcessInfo token){
            token.ProviderClass = token.ProviderMember.IsCollection ? token.Dictionary.GetClassInfo(token.ProviderMember.CollectionElementType.ClassType) : token.Dictionary.GetClassInfo(token.ProviderMember.MemberType);
        }

        public override void UpdateModel(Dictionary model){
            base.UpdateModel(model);
            foreach (XPCustomMemberInfo memberInfo in providedMembers){
                if (memberInfo.IsCollection){
                    ITypeInfo info = XafTypesInfo.Instance.FindTypeInfo(memberInfo.Owner.ClassType);
                    string listViewId = info.Name + "_" + memberInfo.Name + "_ListView";
                    var wrapper = ((ListViewInfoNodeWrapper) new ApplicationNodeWrapper(model).Views.FindViewById(listViewId));
                    if (wrapper == null){
                        ITypeInfo ownerInfo = XafTypesInfo.Instance.FindTypeInfo(memberInfo.CollectionElementType.ClassType);
                        string classListViewId = ownerInfo.Name + "_ListView";
                        var classWrapper = ((ListViewInfoNodeWrapper) new ApplicationNodeWrapper(model).Views.FindViewById(classListViewId));
                        DictionaryNode clone = classWrapper.Node.Clone();
                        clone.SetAttribute("ID", listViewId);
                        new ApplicationNodeWrapper(model).Views.Node.AddChildNode(clone);
                    }
                }
            }
            Active[string.Empty] = false;
        }
    }
}