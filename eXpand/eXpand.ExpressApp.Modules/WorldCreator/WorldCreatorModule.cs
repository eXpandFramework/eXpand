using System;
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using eXpand.ExpressApp.WorldCreator.ClassTypeBuilder;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.ExpressApp.Core;
using System.Linq;

namespace eXpand.ExpressApp.WorldCreator
{
    public sealed partial class WorldCreatorModule : ModuleBase
    {
        private Info _info;

        public WorldCreatorModule()
        {
            InitializeComponent();
        }

        public void SyncModel(IPersistentClassInfo persistentClassInfo, Dictionary dictionary)
        {
            XPClassInfo xpClassInfo = persistentClassInfo.PersistentTypeClassInfo;
            Type type = xpClassInfo.ClassType;
            var wrapper = new ApplicationNodeWrapper(dictionary);
            var applicationNodeWrapper =new ApplicationNodeWrapper(dictionary.Clone());
            XafTypesInfo.Instance.RegisterEntity(type);
            applicationNodeWrapper.Load(type);
            var classInfoNodeWrapper = applicationNodeWrapper.BOModel.FindClassByType(persistentClassInfo.PersistentTypeClassInfo.ClassType);
            wrapper.BOModel.Node.AddChildNode(classInfoNodeWrapper.Node);
            var baseViewInfoNodeWrappers = applicationNodeWrapper.Views.GetDetailViews(classInfoNodeWrapper).Cast<BaseViewInfoNodeWrapper>().ToList();
            baseViewInfoNodeWrappers.AddRange(applicationNodeWrapper.Views.GetListViews(classInfoNodeWrapper).Cast<BaseViewInfoNodeWrapper>());
            foreach (BaseViewInfoNodeWrapper infoNodeWrapper in baseViewInfoNodeWrappers) {
                if (wrapper.Views.Node.FindChildNode(infoNodeWrapper.Node) == null)
                    wrapper.Views.Node.AddChildNode(infoNodeWrapper.Node);
            }            
            
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.LoggingOn+=ApplicationOnLoggingOn;
        }

        private class Info {
            public Info(IEnumerable<Type> types) {
                PersistentTypesInfoType =types.Where(type => typeof (IPersistentClassInfo).IsAssignableFrom(type)).GroupBy(type => type).Select(grouping => grouping.Key).Single();
                ExtendedCollectionMemberInfoType = types.Where(type => typeof(IPersistentCollectionMemberInfo).IsAssignableFrom(type)).GroupBy(type => type).Select(grouping => grouping.Key).Single();
                ExtendedReferenceMemberInfoType = types.Where(type => typeof(IPersistentReferenceMemberInfo).IsAssignableFrom(type)).GroupBy(type => type).Select(grouping => grouping.Key).Single();
            }

            public Type PersistentTypesInfoType { get; private set; }
            public Type ExtendedReferenceMemberInfoType { get; private set; }
            public Type ExtendedCollectionMemberInfoType { get; private set; }    
        }
        private void ApplicationOnLoggingOn(object sender, LogonEventArgs args) {
            var objectSpace = Application.CreateObjectSpace();
            _info = new Info(Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses));
            addDynamicTypes(objectSpace);
//            addExtendedTypes(objectSpace);
        }

        private void addExtendedTypes(ObjectSpace objectSpace) {
            createCollections(objectSpace);
            createReferenceMembers(objectSpace);
        }

        private void createReferenceMembers(ObjectSpace objectSpace) {
            var extendedReferenceMemberInfos = new XPCollection(objectSpace.Session, _info.ExtendedReferenceMemberInfoType).Cast<IExtendedReferenceMemberInfo>().Where(info => !memberExists(info)).ToList();
            foreach (IExtendedReferenceMemberInfo extendedMemberInfo in extendedReferenceMemberInfos) {
                throw new NotImplementedException();
//                IPersistentAssociationAttribute associationAttribute =extendedMemberInfo.TypeAttributes.OfType<IPersistentAssociationAttribute>().ToList()[0];
//                createReferenceMember(extendedMemberInfo, associationAttribute);
            }
            SyncModel(extendedReferenceMemberInfos.Cast<IExtendedMemberInfo>().ToList());
        }

        private void createCollections(ObjectSpace objectSpace) {
            var extendedCollectionMemberInfos = new XPCollection(objectSpace.Session, _info.ExtendedCollectionMemberInfoType).Cast<IExtendedCollectionMemberInfo>().Where(info => !memberExists(info)).ToList();
            foreach (IExtendedCollectionMemberInfo extendedMemberInfo in extendedCollectionMemberInfos) {
                throw new NotImplementedException();
//                IPersistentAssociationAttribute associationAttribute =extendedMemberInfo.TypeAttributes.OfType<IPersistentAssociationAttribute>().ToList()[0];
//                createCollection((extendedMemberInfo), associationAttribute);
            }
            SyncModel(extendedCollectionMemberInfos.Cast<IExtendedMemberInfo>().ToList());
        }

        private void SyncModel(List<IExtendedMemberInfo> extendedCollectionMemberInfos) {
            var wrapper = new ApplicationNodeWrapper(new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName),Application.Model.Schema));
            var applicationNodeWrapper = new ApplicationNodeWrapper(Application.Model);
            var owners = extendedCollectionMemberInfos.GroupBy(info => info.Owner).Select(grouping => grouping.Key).ToList();
            foreach (var owner in owners) {
                wrapper.Load(owner);
                var classInfoNodeWrapper = wrapper.BOModel.FindClassByType(owner);
                Type type = owner;
                foreach (var memberInfo in extendedCollectionMemberInfos.Where(info => info.Owner==type)) {
                    IExtendedMemberInfo info = memberInfo;
                    var propertyInfoNodeWrapper = classInfoNodeWrapper.Properties.Where(nodeWrapper => nodeWrapper.Name == info.Name).Single();
                    applicationNodeWrapper.BOModel.FindClassByType(owner).Node.AddChildNode(propertyInfoNodeWrapper.Node);
                }
            }
        }

        private bool memberExists(IExtendedMemberInfo info) {
            return XafTypesInfo.Instance.FindTypeInfo(info.Owner).FindMember(info.Name)!= null;
        }

        private void addDynamicTypes(ObjectSpace objectSpace) {
            var collection = new XPCollection(objectSpace.Session, _info.PersistentTypesInfoType);
            
//            XafTypesInfo.XpoTypeInfoSource.XPDictionary.AddClasses(collection.Cast<IPersistentClassInfo>().ToList());
            var types = new List<TypeInfo>();
            var builder = PersistentClassTypeBuilder.BuildClass();
            var persistentClassInfos = collection.Cast<IPersistentClassInfo>();
            var assemblies = persistentClassInfos.GroupBy(info => info.AssemblyName).Select(grouping => grouping.Key);
            foreach (var assembly in assemblies) {
                IClassDefineBuilder defineBuilder = builder.WithAssemblyName(assembly);
                string s = assembly;
                var classInfos = persistentClassInfos.Where(info => info.AssemblyName == s);
                types.AddRange(defineBuilder.Define(classInfos.ToList()));
#if DEBUG
                defineBuilder.AssemblyBuilder.Save(assembly+".dll");
#endif
            }

            objectSpace.Session.UpdateSchema(types.Select(info => info.Type).ToArray());
            foreach (IPersistentClassInfo classInfo in types.Select(info => info.PersistentClassInfo)){
                SyncModel(classInfo, Application.Model);
            }
        }


        private class PersistentClassOrderInfo
        {
            public PersistentClassOrderInfo(IPersistentClassInfo persistentClassInfo, int order) {
                IPersistentClassInfo = persistentClassInfo;
                Order = order;
            }

            public IPersistentClassInfo IPersistentClassInfo { get; set; }
            public int Order { get; set; }
        }
//        private void createReferenceMember(IExtendedReferenceMemberInfo extendedReferenceMemberInfo, IPersistentAssociationAttribute attribute) {
//            XafTypesInfo.Instance.CreateMember(extendedReferenceMemberInfo.Owner,
//                                               extendedReferenceMemberInfo.ReferenceType, attribute.AssociationName,
//                                               XafTypesInfo.XpoTypeInfoSource.XPDictionary);
//        }

//        private void createCollection(IExtendedCollectionMemberInfo extendedCollectionMemberInfo, IPersistentAssociationAttribute associationAttribute) {
//            
//            XafTypesInfo.Instance.CreateCollection(extendedCollectionMemberInfo.Owner, associationAttribute.ElementType,
//                                                   associationAttribute.AssociationName,
//                                                   XafTypesInfo.XpoTypeInfoSource.XPDictionary,extendedCollectionMemberInfo.Name);
//        }


        
    }
}
