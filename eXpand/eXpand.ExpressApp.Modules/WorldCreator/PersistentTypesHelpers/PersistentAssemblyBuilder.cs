using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.ExpressApp.WorldCreator.Observers;
using eXpand.Persistent.Base.General;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Utils.ExpressionBuilder;
using eXpand.Xpo;

namespace eXpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public interface IClassHandler
    {
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name,IPersistentClassInfo referenceClassInfo);
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType);
        void CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc);
        void CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc);
        void CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func);
        void CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo);
        void CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo,
                                    string associationName);
    }

    public interface IPersistentAssemblyBuilder : IClassHandler {
        IPersistentAssemblyInfo PersistentAssemblyInfo { get; }
        IClassHandler CreateClasses(IEnumerable<string> classNames);
    }

    public class PersistentAssemblyBuilder : Builder<IPersistentAssemblyInfo>, IPersistentAssemblyBuilder {
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        IEnumerable<IPersistentClassInfo> _persistentClassInfos;
        readonly ObjectSpace _objectSpace;

        PersistentAssemblyBuilder(IPersistentAssemblyInfo persistentAssemblyInfo)
        {
            _persistentAssemblyInfo = persistentAssemblyInfo;
            _objectSpace = ObjectSpace.FindObjectSpace(persistentAssemblyInfo);
        }

        public IPersistentAssemblyInfo PersistentAssemblyInfo
        {
            get { return _persistentAssemblyInfo; }
        }

        public static PersistentAssemblyBuilder BuildAssembly(ObjectSpace objectSpace, string name)
        {
            new PersistentReferenceMemberInfoObserver(objectSpace);
            new CodeTemplateInfoObserver(objectSpace);
            new CodeTemplateObserver(objectSpace);
            var assemblyInfo =
                (IPersistentAssemblyInfo) objectSpace.CreateObject(TypesInfo.Instance.PersistentAssemblyInfoType);
            assemblyInfo.Name = name;            
            return new PersistentAssemblyBuilder(assemblyInfo);
        }

        public IClassHandler CreateClasses(IEnumerable<string> classNames)
        {
            _persistentClassInfos = classNames.Select(s =>
            {
                var persistentClassInfo = (IPersistentClassInfo) _objectSpace.CreateObject(TypesInfo.Instance.PersistentTypesInfoType);
                persistentClassInfo.Name = s;
                persistentClassInfo.PersistentAssemblyInfo=_persistentAssemblyInfo;
                _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
                persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
                return persistentClassInfo;
            }).ToList();
            return this;
        }


        void IClassHandler.CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc){
            foreach (IPersistentClassInfo info in _persistentClassInfos) {
                IEnumerable<IPersistentClassInfo> persistentClassInfos = referenceClassInfoFunc.Invoke(info);
                if (persistentClassInfos != null) {
                    foreach (IPersistentClassInfo persistentClassInfo in persistentClassInfos) {
                        ((IClassHandler)this).CreateRefenenceMember(info, persistentClassInfo.Name, persistentClassInfo);
                    }
                }
            }
        }

        void IClassHandler.CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc)
        {
            foreach (var info in _persistentClassInfos){
                IEnumerable<Type> types = referenceTypeFunc.Invoke(info);
                if (types != null){
                    foreach (var type in types) {
                        IPersistentReferenceMemberInfo persistentReferenceMemberInfo = ((IClassHandler) this).CreateRefenenceMember(info, type.Name,type);
                        persistentReferenceMemberInfo.ReferenceTypeFullName = type.FullName;
                    }
                }
            }
        }

        void IClassHandler.CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func) {
            foreach (var persistentClassInfo in _persistentClassInfos){
                IEnumerable<string> invoke = func.Invoke(persistentClassInfo);
                if (invoke!= null) {
                    foreach (string name in invoke) {
                        var persistentCoreTypeMemberInfo = (IPersistentCoreTypeMemberInfo)_objectSpace.CreateObject(TypesInfo.Instance.PersistentCoreTypeInfoType);
                        persistentCoreTypeMemberInfo.Name=name;
                        persistentClassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
                        persistentCoreTypeMemberInfo.DataType = (XPODataType) Enum.Parse(typeof (XPODataType), typeof (T).Name);
                        persistentCoreTypeMemberInfo.SetDefaultTemplate(TemplateType.ReadWriteMember);                        
                    }
                }
            }

        }

        void IClassHandler.CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name,IPersistentClassInfo refenceClassInfo) {
            ((IClassHandler) this).CreateCollectionMember(persistentClassInfo, name, refenceClassInfo, null);
        }

        void IClassHandler.CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo, string associationName) {
            var persistentCollectionMemberInfo =
                createPersistentAssociatedMemberInfo<IPersistentCollectionMemberInfo>(name, persistentClassInfo,
                                                                                      TypesInfo.Instance.PersistentCollectionInfoType,
                                                                                      associationName,TemplateType.ReadOnlyMember);
            persistentCollectionMemberInfo.CollectionTypeFullName = refenceClassInfo.PersistentAssemblyInfo.Name + "." +
                                                                    refenceClassInfo.Name;
        }


        IPersistentReferenceMemberInfo IClassHandler.CreateRefenenceMember(IPersistentClassInfo info, string name,IPersistentClassInfo referenceClassInfo)
        {
            var persistentReferenceMemberInfo =
                createPersistentAssociatedMemberInfo<IPersistentReferenceMemberInfo>(name, info,
                                TypesInfo.Instance.PersistentReferenceInfoType,TemplateType.ReadWriteMember);
            persistentReferenceMemberInfo.ReferenceTypeFullName = info.PersistentAssemblyInfo.Name + "." + referenceClassInfo.Name;
            return persistentReferenceMemberInfo;
        }

        IPersistentReferenceMemberInfo IClassHandler.CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType) {
            var persistentReferenceMemberInfo =
                createPersistentAssociatedMemberInfo<IPersistentReferenceMemberInfo>(name, persistentClassInfo,
                                                                                     TypesInfo.Instance.PersistentReferenceInfoType,
                                                                                     TemplateType.ReadWriteMember);
            persistentReferenceMemberInfo.ReferenceTypeFullName = referenceType.FullName;
            return persistentReferenceMemberInfo;
        }

        TPersistentAssociatedMemberInfo createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(
            string name, IPersistentClassInfo info, Type infoType, string assocaitionName, TemplateType templateType)
            where TPersistentAssociatedMemberInfo : IPersistentAssociatedMemberInfo {
            var persistentReferenceMemberInfo = (TPersistentAssociatedMemberInfo)_objectSpace.CreateObject(infoType);
            persistentReferenceMemberInfo.Name = name;
            persistentReferenceMemberInfo.RelationType=RelationType.OneToMany;
            info.OwnMembers.Add(persistentReferenceMemberInfo);
            persistentReferenceMemberInfo.SetDefaultTemplate(templateType);
            var persistentAssociationAttribute =
                (IPersistentAssociationAttribute)
                _objectSpace.CreateObject(TypesInfo.Instance.PersistentAssociationAttributeType);
            persistentAssociationAttribute.AssociationName = assocaitionName??persistentReferenceMemberInfo.Name;
            persistentReferenceMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            return persistentReferenceMemberInfo;
        }

        TPersistentAssociatedMemberInfo createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(
            string name, IPersistentClassInfo info, Type infoType, TemplateType templateType)
            where TPersistentAssociatedMemberInfo : IPersistentAssociatedMemberInfo {
            return createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(name, info, infoType, null, templateType);
        }
    }
}