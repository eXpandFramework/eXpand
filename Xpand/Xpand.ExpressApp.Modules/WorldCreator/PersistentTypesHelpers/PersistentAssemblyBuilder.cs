using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using Xpand.ExpressApp.WorldCreator.Core;
using Xpand.ExpressApp.WorldCreator.Observers;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Utils.ExpressionBuilder;

namespace Xpand.ExpressApp.WorldCreator.PersistentTypesHelpers {
    public interface IClassInfoHandler {
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo referenceClassInfo, bool createAssociation);
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo referenceClassInfo);
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType);
        IPersistentReferenceMemberInfo CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType, bool createAssociation);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc, bool createAssociation);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc);
        void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceClassInfoFunc, bool createAssociation);
        void CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func);
        void CreateSimpleMembers(DBColumnType xpoDataType, Func<IPersistentClassInfo, IEnumerable<string>> func);
        void CreateDefaultClassOptions(Func<IPersistentClassInfo, bool> func);
        void CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo);

        void CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo,
                                    string associationName);

        void SetInheritance(Func<IPersistentClassInfo, Type> func);
        void SetInheritance(Func<IPersistentClassInfo, IPersistentClassInfo> func);
        void CreateTemplateInfos(Func<IPersistentClassInfo, List<ITemplateInfo>> func);
    }

    public interface IPersistentAssemblyBuilder : IClassInfoHandler {
        IPersistentAssemblyInfo PersistentAssemblyInfo { get; }
        IClassInfoHandler CreateClasses(IEnumerable<string> classNames);
    }

    public class PersistentAssemblyBuilder : Builder<IPersistentAssemblyInfo>, IPersistentAssemblyBuilder {
        readonly IPersistentAssemblyInfo _persistentAssemblyInfo;
        IEnumerable<IPersistentClassInfo> _persistentClassInfos;
        readonly IObjectSpace _objectSpace;

        public PersistentAssemblyBuilder(IPersistentAssemblyInfo persistentAssemblyInfo) {
            _persistentAssemblyInfo = persistentAssemblyInfo;
            _objectSpace = XPObjectSpace.FindObjectSpaceByObject(persistentAssemblyInfo);
        }

        public IPersistentAssemblyInfo PersistentAssemblyInfo {
            get { return _persistentAssemblyInfo; }
        }

        internal static PersistentAssemblyBuilder BuildAssembly() {
            return BuildAssembly(GetUniqueAssemblyName());
        }

        static string GetUniqueAssemblyName() {
            return "a" + Guid.NewGuid().ToString().Replace("-", "");
        }

        internal static PersistentAssemblyBuilder BuildAssembly(XPObjectSpace objectSpace) {
            return BuildAssembly(objectSpace, GetUniqueAssemblyName());
        }
        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        internal static PersistentAssemblyBuilder BuildAssembly(string name) {
            var objectSpace = new XPObjectSpaceProvider(new MemoryDataStoreProvider()).CreateObjectSpace();
            return BuildAssembly(objectSpace, name);
        }


        public static PersistentAssemblyBuilder BuildAssembly(IObjectSpace objectSpace, string name) {
            new PersistentReferenceMemberInfoObserver(objectSpace);
            new CodeTemplateInfoObserver(objectSpace);
            new CodeTemplateObserver(objectSpace);
            var assemblyInfo =
                (IPersistentAssemblyInfo)objectSpace.CreateObject(WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssemblyInfo>());
            assemblyInfo.Name = name;
            return new PersistentAssemblyBuilder(assemblyInfo);
        }

        public IClassInfoHandler CreateClasses(IEnumerable<string> classNames) {
            _persistentClassInfos = classNames.Select(s => {
                var persistentClassInfo = (IPersistentClassInfo)((XPObjectSpace)_objectSpace).Session.FindObject(WCTypesInfo.Instance.FindBussinessObjectType<IPersistentClassInfo>(), CriteriaOperator.Parse("Name=?", s)) ?? _objectSpace.CreateWCObject<IPersistentClassInfo>();
                persistentClassInfo.Name = s;
                persistentClassInfo.PersistentAssemblyInfo = _persistentAssemblyInfo;
                _persistentAssemblyInfo.PersistentClassInfos.Add(persistentClassInfo);
                persistentClassInfo.SetDefaultTemplate(TemplateType.Class);
                return persistentClassInfo;
            }).ToList();
            return this;
        }


        void IClassInfoHandler.CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc, bool createAssociation) {
            foreach (IPersistentClassInfo info in _persistentClassInfos) {
                IEnumerable<IPersistentClassInfo> persistentClassInfos = referenceClassInfoFunc.Invoke(info);
                if (persistentClassInfos != null) {
                    foreach (IPersistentClassInfo persistentClassInfo in persistentClassInfos) {
                        ((IClassInfoHandler)this).CreateRefenenceMember(info, persistentClassInfo.Name, persistentClassInfo, createAssociation);
                    }
                }
            }
        }

        void IClassInfoHandler.CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<IPersistentClassInfo>> referenceClassInfoFunc) {
            ((IClassInfoHandler)this).CreateReferenceMembers(referenceClassInfoFunc, false);
        }

        void IClassInfoHandler.CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceTypeFunc) {
            CreateReferenceMembers(referenceTypeFunc, false);
        }

        public void CreateReferenceMembers(Func<IPersistentClassInfo, IEnumerable<Type>> referenceClassInfoFunc, bool createAssociation) {
            foreach (IPersistentClassInfo info in _persistentClassInfos) {
                IEnumerable<Type> types = referenceClassInfoFunc.Invoke(info);
                if (types != null) {
                    foreach (Type type in types) {
                        IPersistentReferenceMemberInfo persistentReferenceMemberInfo =
                            ((IClassInfoHandler)this).CreateRefenenceMember(info, type.Name, type, createAssociation);
                        persistentReferenceMemberInfo.SetReferenceTypeFullName(type.FullName);
                    }
                }
            }
        }


        void IClassInfoHandler.CreateSimpleMembers<T>(Func<IPersistentClassInfo, IEnumerable<string>> func) {
            var xpoDataType = (DBColumnType)Enum.Parse(typeof(DBColumnType), typeof(T).Name);
            ((IClassInfoHandler)this).CreateSimpleMembers(xpoDataType, func);
        }

        public void CreateSimpleMembers(DBColumnType xpoDataType, Func<IPersistentClassInfo, IEnumerable<string>> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                IEnumerable<string> invoke = func.Invoke(persistentClassInfo);
                if (invoke != null) {
                    foreach (string name in invoke) {
                        var persistentCoreTypeMemberInfo = (IPersistentCoreTypeMemberInfo)_objectSpace.CreateObject(WCTypesInfo.Instance.FindBussinessObjectType<IPersistentCoreTypeMemberInfo>());
                        persistentCoreTypeMemberInfo.Name = name;
                        persistentClassInfo.OwnMembers.Add(persistentCoreTypeMemberInfo);
                        persistentCoreTypeMemberInfo.DataType = xpoDataType;
                        persistentCoreTypeMemberInfo.SetDefaultTemplate(TemplateType.XPReadWritePropertyMember);
                    }
                }
            }

        }

        void IClassInfoHandler.CreateDefaultClassOptions(Func<IPersistentClassInfo, bool> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                if (func.Invoke(persistentClassInfo))
                    persistentClassInfo.TypeAttributes.Add((IPersistentAttributeInfo)
                        _objectSpace.CreateObject(WCTypesInfo.Instance.FindBussinessObjectType<IPersistentDefaulClassOptionsAttribute>()));
            }
        }

        void IClassInfoHandler.CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo) {
            ((IClassInfoHandler)this).CreateCollectionMember(persistentClassInfo, name, refenceClassInfo, null);
        }

        void IClassInfoHandler.CreateCollectionMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo refenceClassInfo, string associationName) {
            var persistentCollectionMemberInfo = createPersistentAssociatedMemberInfo<IPersistentCollectionMemberInfo>(name, persistentClassInfo,
                                                                                      WCTypesInfo.Instance.FindBussinessObjectType<IPersistentCollectionMemberInfo>(),
                                                                                      associationName, TemplateType.XPCollectionMember, true);
            persistentCollectionMemberInfo.SetCollectionTypeFullName(refenceClassInfo.PersistentAssemblyInfo.Name + "." + refenceClassInfo.Name);
        }

        void IClassInfoHandler.SetInheritance(Func<IPersistentClassInfo, Type> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                var invoke = func.Invoke(persistentClassInfo);
                if (invoke != null) persistentClassInfo.BaseTypeFullName = invoke.FullName;
            }
        }

        void IClassInfoHandler.SetInheritance(Func<IPersistentClassInfo, IPersistentClassInfo> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                var classInfo = func.Invoke(persistentClassInfo);
                if (classInfo != null)
                    persistentClassInfo.BaseTypeFullName = classInfo.PersistentAssemblyInfo.Name + "." + classInfo.Name;
            }
        }

        void IClassInfoHandler.CreateTemplateInfos(Func<IPersistentClassInfo, List<ITemplateInfo>> func) {
            foreach (var persistentClassInfo in _persistentClassInfos) {
                List<ITemplateInfo> templateInfos = func.Invoke(persistentClassInfo);
                if (templateInfos.Count > 1)
                    Debug.Print("");

                foreach (var templateInfo in templateInfos) {
                    persistentClassInfo.TemplateInfos.Add(templateInfo);
                }
            }
        }


        IPersistentReferenceMemberInfo IClassInfoHandler.CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, IPersistentClassInfo referenceClassInfo, bool createAssociation) {
            var persistentReferenceMemberInfo =
                    createPersistentAssociatedMemberInfo<IPersistentReferenceMemberInfo>(name, persistentClassInfo,
                                    WCTypesInfo.Instance.FindBussinessObjectType<IPersistentReferenceMemberInfo>(), TemplateType.XPReadWritePropertyMember, createAssociation);
            persistentReferenceMemberInfo.SetReferenceTypeFullName(persistentClassInfo.PersistentAssemblyInfo.Name + "." + referenceClassInfo.Name);
            return persistentReferenceMemberInfo;
        }

        IPersistentReferenceMemberInfo IClassInfoHandler.CreateRefenenceMember(IPersistentClassInfo info, string name, IPersistentClassInfo referenceClassInfo) {
            return ((IClassInfoHandler)this).CreateRefenenceMember(info, name, referenceClassInfo, false);
        }

        IPersistentReferenceMemberInfo IClassInfoHandler.CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType, bool createAssociation) {
            var persistentReferenceMemberInfo =
                createPersistentAssociatedMemberInfo<IPersistentReferenceMemberInfo>(name, persistentClassInfo,
                                                                                     WCTypesInfo.Instance.FindBussinessObjectType<IPersistentReferenceMemberInfo>(),
                                                                                     TemplateType.XPReadWritePropertyMember, createAssociation);
            persistentReferenceMemberInfo.SetReferenceTypeFullName(referenceType.FullName);
            return persistentReferenceMemberInfo;
        }

        IPersistentReferenceMemberInfo IClassInfoHandler.CreateRefenenceMember(IPersistentClassInfo persistentClassInfo, string name, Type referenceType) {
            return ((IClassInfoHandler)this).CreateRefenenceMember(persistentClassInfo, name, referenceType, false);
        }

        TPersistentAssociatedMemberInfo createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(
            string name, IPersistentClassInfo info, Type infoType, string assocaitionName, TemplateType templateType,
            bool createAssociationAttribute)
            where TPersistentAssociatedMemberInfo : IPersistentAssociatedMemberInfo {
            var persistentReferenceMemberInfo = (TPersistentAssociatedMemberInfo)_objectSpace.CreateObject(infoType);
            persistentReferenceMemberInfo.Name = name;
            persistentReferenceMemberInfo.RelationType = RelationType.OneToMany;
            info.OwnMembers.Add(persistentReferenceMemberInfo);
            persistentReferenceMemberInfo.SetDefaultTemplate(templateType);
            if (createAssociationAttribute) {
                var persistentAssociationAttribute =
                    (IPersistentAssociationAttribute)
                    _objectSpace.CreateObject(WCTypesInfo.Instance.FindBussinessObjectType<IPersistentAssociationAttribute>());
                persistentAssociationAttribute.AssociationName = assocaitionName ?? persistentReferenceMemberInfo.Name;
                persistentReferenceMemberInfo.TypeAttributes.Add(persistentAssociationAttribute);
            }
            return persistentReferenceMemberInfo;
        }

        TPersistentAssociatedMemberInfo createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(string name, IPersistentClassInfo info, Type infoType, TemplateType templateType, bool createAssociation)
            where TPersistentAssociatedMemberInfo : IPersistentAssociatedMemberInfo {
            return createPersistentAssociatedMemberInfo<TPersistentAssociatedMemberInfo>(name, info, infoType, null, templateType, createAssociation);
        }
    }
}