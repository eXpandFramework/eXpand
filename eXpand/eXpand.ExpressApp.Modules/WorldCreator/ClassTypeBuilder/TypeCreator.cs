using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;

namespace eXpand.ExpressApp.WorldCreator.ClassTypeBuilder {
    public class TypeCreator {
        private readonly ITypesInfo _typesInfo;
        private readonly UnitOfWork _unitOfWork;

        public TypeCreator(ITypesInfo typesInfo, UnitOfWork unitOfWork)
        {
            _typesInfo = typesInfo;
            _unitOfWork = unitOfWork;
        }

        private bool memberExists(IExtendedMemberInfo info)
        {
            return XafTypesInfo.Instance.FindTypeInfo(info.Owner).FindMember(info.Name) != null;
        }

        private void createReferenceMembers()
        {
            var extendedReferenceMemberInfos = new XPCollection(_unitOfWork, _typesInfo.ExtendedReferenceMemberInfoType).Cast<IExtendedReferenceMemberInfo>().Where(info => !memberExists(info)).ToList();
            foreach (IExtendedReferenceMemberInfo extendedMemberInfo in extendedReferenceMemberInfos)
            {
                var classInfo = XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(extendedMemberInfo.Owner);
                var customMemberInfo = classInfo.CreateMember(extendedMemberInfo.Name, extendedMemberInfo.ReferenceType);
                createAttributes(extendedMemberInfo, customMemberInfo);
                XafTypesInfo.Instance.RefreshInfo(classInfo.ClassType);
            }
        }

        private void createCollections()
        {
            var extendedCollectionMemberInfos = new XPCollection(_unitOfWork, _typesInfo.ExtendedCollectionMemberInfoType).Cast<IExtendedCollectionMemberInfo>().Where(info => !memberExists(info)).ToList();
            foreach (IExtendedCollectionMemberInfo extendedMemberInfo in extendedCollectionMemberInfos) {
                XPCustomMemberInfo customMemberInfo =
                    XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(extendedMemberInfo.Owner).CreateMember(
                        extendedMemberInfo.Name, typeof (XPCollection), true);
                createAttributes(extendedMemberInfo, customMemberInfo);
            }
        }

        private void createAttributes(IExtendedMemberInfo extendedMemberInfo, XPCustomMemberInfo customMemberInfo)
        {
            foreach (IPersistentAttributeInfo typeAttribute in extendedMemberInfo.TypeAttributes) {
                AttributeInfo attributeInfo = typeAttribute.Create();
                customMemberInfo.AddAttribute((Attribute)Activator.CreateInstance(attributeInfo.Constructor.DeclaringType,attributeInfo.InitializedArgumentValues));
            }
        }

        public void CreateExtendedTypes()
        {
            createCollections();
            createReferenceMembers();
        }

        public Type GetDynamicModule()
        {
            var collection = new XPCollection(_unitOfWork, _typesInfo.PersistentTypesInfoType);
            var types = new List<TypeInfo>();
            var builder = PersistentTypeBuilder.BuildClass();
            var persistentClassInfos = collection.Cast<IPersistentClassInfo>();
            var assemblyNames = persistentClassInfos.GroupBy(info => info.AssemblyName).Select(grouping => grouping.Key);
            Type moduleType = null;
            foreach (var assembly in assemblyNames)
            {
                ITypeDefineBuilder defineBuilder = builder.WithAssemblyName(assembly);
                moduleType = defineBuilder.ModuleBuilder.GetTypes().Where(type => typeof(ModuleBase).IsAssignableFrom(type)).Single();
                string s = assembly;
                var classInfos = persistentClassInfos.Where(info => info.AssemblyName == s);
                types.AddRange(defineBuilder.Define(classInfos.ToList()));
                UpdateModuleInfo(_unitOfWork, moduleType.FullName,assembly,new Version(0,0,0,DateTime.Now.Second).ToString());
                if ((ConfigurationManager.AppSettings["SaveDynamicAssembly"]+"").ToLower()=="true")
                    defineBuilder.AssemblyBuilder.Save(assembly);

            }
            
            return moduleType;

        }

        private void UpdateModuleInfo(Session session,string moduleName, string assemblyName, string version) {
            var moduleInfo = (ModuleInfo)session.FindObject(typeof(ModuleInfo), new BinaryOperator("Name", moduleName)) ??new ModuleInfo(session) {Name = moduleName};
            moduleInfo.AssemblyFileName = assemblyName;
            moduleInfo.Version = version;
            _unitOfWork.CommitChanges();
        }
    }
}