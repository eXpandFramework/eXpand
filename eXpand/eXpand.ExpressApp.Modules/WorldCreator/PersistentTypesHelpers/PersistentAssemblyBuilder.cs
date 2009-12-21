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
    public interface IMemberHandler
    {
        void CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc);
        void CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc);
        void CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func);
    }

    public class PersistentAssemblyBuilder : Builder<IPersistentAssemblyInfo>, IMemberHandler
    {
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

        public IMemberHandler CreateClasses(IEnumerable<string> classNames)
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

        void IMemberHandler.CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc)
        {
            foreach (IPersistentClassInfo info in _persistentClassInfos) {
                IEnumerable<IPersistentClassInfo> persistentClassInfos = referenceClassInfoFunc.Invoke(info);
                if (persistentClassInfos != null) {
                    foreach (IPersistentClassInfo persistentClassInfo in persistentClassInfos) {
                        IPersistentReferenceMemberInfo persistentReferenceMemberInfo = createMember(info,persistentClassInfo.Name);
                        persistentReferenceMemberInfo.ReferenceTypeFullName =
                            persistentClassInfo.PersistentAssemblyInfo.Name + "." + persistentClassInfo.Name;
                    }
                }
            }
        }

        void IMemberHandler.CreateRefenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc)
        {
            foreach (var info in _persistentClassInfos){
                IEnumerable<Type> types = referenceTypeFunc.Invoke(info);
                if (types != null){
                    foreach (var type in types) {
                        IPersistentReferenceMemberInfo persistentReferenceMemberInfo = createMember(info, type.Name);
                        persistentReferenceMemberInfo.ReferenceTypeFullName = type.FullName;
                    }
                }
            }
        }

        public void CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func) {
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


        IPersistentReferenceMemberInfo createMember(IPersistentClassInfo info, string name)
        {
            var persistentReferenceMemberInfo =(IPersistentReferenceMemberInfo)_objectSpace.CreateObject(TypesInfo.Instance.PersistentReferenceInfoType);
            persistentReferenceMemberInfo.Name = name;
            persistentReferenceMemberInfo.RelationType=RelationType.OneToMany;
            info.OwnMembers.Add(persistentReferenceMemberInfo);
            persistentReferenceMemberInfo.SetDefaultTemplate(TemplateType.ReadWriteMember);
            var persistentAssociationAttribute =
                (IPersistentAssociationAttribute)
                _objectSpace.CreateObject(TypesInfo.Instance.PersistentAssociationAttributeType);
            persistentAssociationAttribute.AssociationName = persistentReferenceMemberInfo.Name;
            persistentReferenceMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            return persistentReferenceMemberInfo;
        }
    }
}