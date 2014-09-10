using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.Persistent.Base.RuntimeMembers.Model.Collections;
using Xpand.Persistent.Base.Xpo;
using Xpand.Persistent.Base.Xpo.MetaData;
using Xpand.Xpo;
using Xpand.Xpo.MetaData;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.RuntimeMembers {
    public class RuntimeMemberBuilder {
        public static event EventHandler<CustomCreateMemberArgs> CustomCreateMember;

        static void OnCustomCreateMember(CustomCreateMemberArgs e) {
            EventHandler<CustomCreateMemberArgs> handler = CustomCreateMember;
            if (handler != null) handler(null, e);
        }

        
        private static IEnumerable<IModelMemberEx> GetMembersEx(IModelApplication model) {
            return model.BOModel.SelectMany(modelClass => modelClass.AllMembers).OfType<IModelMemberEx>().Distinct();
        }

        public static void CreateRuntimeMembers(IModelApplication model) {
            using (var objectSpace = CreateObjectSpace()) {
                Tracing.Tracer.LogVerboseSubSeparator("RuntimeMembers Creation started");
                var modelMemberOneToManyCollections = new List<IModelMemberOneToManyCollection>();
                var xpObjectSpace = objectSpace as XPObjectSpace;
                var modelMemberExs = GetMembersEx(model).ToArray();
                foreach (var memberEx in modelMemberExs) {
                    var customCreateMemberArgs = new CustomCreateMemberArgs(memberEx);
                    OnCustomCreateMember(customCreateMemberArgs);
                    if (!customCreateMemberArgs.Handled) {
                        var modelMemberOneToManyCollection = memberEx as IModelMemberOneToManyCollection;
                        if (modelMemberOneToManyCollection == null) {
                            CreateXpandCustomMemberInfo(memberEx, xpObjectSpace);
                        }
                        else {
                            modelMemberOneToManyCollections.Add(modelMemberOneToManyCollection);
                        }
                    }
                }
                RefreshTypes(model.GetTypesInfo(), modelMemberExs.Select(ex => ex.ModelClass.TypeInfo).Distinct());
                CreateAssociatedCollectionMembers(modelMemberOneToManyCollections, xpObjectSpace);
                RefreshTypes(model.GetTypesInfo(), modelMemberOneToManyCollections.Select(collection => collection.CollectionType.TypeInfo).Distinct());
            }
            Tracing.Tracer.LogVerboseSubSeparator("RuntimeMembers Creation finished");
        }

        static void CreateAssociatedCollectionMembers(IEnumerable<IModelMemberOneToManyCollection> modelMemberOneToManyCollections, XPObjectSpace xpObjectSpace) {
            foreach (var modelMemberOneToManyCollection in modelMemberOneToManyCollections) {
                CreateXpandCustomMemberInfo(modelMemberOneToManyCollection, xpObjectSpace);
            }
        }

        static void RefreshTypes(ITypesInfo typesInfo, IEnumerable<ITypeInfo> typeInfos) {
            foreach (var typeInfo in typeInfos) {
                typesInfo.RefreshInfo(typeInfo);
            }
        }

        static IObjectSpace CreateObjectSpace() {
            return XpandModuleBase.ObjectSpaceCreated?ApplicationHelper.Instance.Application.ObjectSpaceProvider.CreateObjectSpace():null;
        }

        static void CreateXpandCustomMemberInfo(IModelMemberEx modelMemberEx, XPObjectSpace objectSpace) {
            try {
                Type classType = modelMemberEx.ModelClass.TypeInfo.Type;
                XPClassInfo xpClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(classType);
                lock (xpClassInfo) {
                    var customMemberInfo = xpClassInfo.FindMember(modelMemberEx.Name) as XPCustomMemberInfo;
                    if (customMemberInfo == null) {
                        customMemberInfo= CreateMemberInfo(modelMemberEx, xpClassInfo);
                        ((IModelTypesInfoProvider) modelMemberEx.Application).TypesInfo.RefreshInfo(classType);
                        AddAttributes(modelMemberEx, customMemberInfo);
                    }
                    var xpandCustomMemberInfo = customMemberInfo as XpandCustomMemberInfo;
                    if (xpandCustomMemberInfo != null) {
                        CreateColumn(modelMemberEx as IModelMemberPersistent, objectSpace, xpandCustomMemberInfo);
                        CreateForeignKey(modelMemberEx as IModelMemberOneToManyCollection, objectSpace, xpandCustomMemberInfo);
                        UpdateMember(modelMemberEx, customMemberInfo);
                    }
                }
            }
            catch (Exception exception) {
                throw new Exception(
                    ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId>.GetExceptionMessage(
                        ExceptionId.ErrorOccursWhileAddingTheCustomProperty,
                        modelMemberEx.MemberInfo.MemberType,
                        ((IModelClass) modelMemberEx.Parent).Name,
                        modelMemberEx.Name,
                        exception.Message));
            }
        }

        static void CreateForeignKey(IModelMemberOneToManyCollection modelMemberOneToManyCollection, XPObjectSpace objectSpace,  XpandCustomMemberInfo customMemberInfo) {
            if (CanCreateForeignKey(modelMemberOneToManyCollection, objectSpace)) {
                var throwUnableToCreateDbObjectException = ((IModelOptionMemberPersistent) modelMemberOneToManyCollection.Application.Options).ThrowUnableToCreateDbObjectException;
                var xpCustomMemberInfo = customMemberInfo.GetAssociatedMember() as XPCustomMemberInfo;
                if (xpCustomMemberInfo == null) throw new NullReferenceException("xpCustomMemberInfo");
                objectSpace.CreateForeignKey(xpCustomMemberInfo,throwUnableToCreateDbObjectException);
                modelMemberOneToManyCollection.AssociatedMember.DataStoreForeignKeyCreated = true;
                modelMemberOneToManyCollection.DataStoreForeignKeyCreated = true;
            }
        }

        static bool CanCreateForeignKey(IModelMemberOneToManyCollection modelMemberOneToManyCollection, XPObjectSpace objectSpace) {
            return CanCreateDbArtifact(modelMemberOneToManyCollection, objectSpace)&&!modelMemberOneToManyCollection.AssociatedMember.DataStoreForeignKeyCreated;
        }

        static void CreateColumn(IModelMemberPersistent modelMemberPersistent, XPObjectSpace objectSpace, 
                                 XpandCustomMemberInfo customMemberInfo) {
            if (CanCreateColumn(modelMemberPersistent, objectSpace)) {
                objectSpace.CreateColumn(customMemberInfo, ((IModelOptionMemberPersistent)modelMemberPersistent.Application.Options).ThrowUnableToCreateDbObjectException);
                modelMemberPersistent.DataStoreColumnCreated = true;
                modelMemberPersistent.DataStoreForeignKeyCreated = customMemberInfo.HasAttribute(typeof(AssociationAttribute));
            }
        }

        static bool CanCreateColumn(IModelMemberPersistent modelMemberPersistent, XPObjectSpace objectSpace) {
            return CanCreateDbArtifact(modelMemberPersistent, objectSpace) && !modelMemberPersistent.DataStoreColumnCreated && modelMemberPersistent.MemberInfo.IsPersistent;
        }

        static bool CanCreateDbArtifact(IModelMemberEx modelMemberEx, XPObjectSpace objectSpace) {
            return modelMemberEx != null && objectSpace != null && (modelMemberEx.CreatedAtDesignTime.HasValue&&!modelMemberEx.CreatedAtDesignTime.Value);
        }

        static void UpdateMember(IModelMemberEx modelMemberEx, XPMemberInfo xpMemberInfo) {
            var modelRuntimeCalculatedMember = modelMemberEx as IModelMemberCalculated;
            if (modelRuntimeCalculatedMember != null) {
                ((XpandCalcMemberInfo)xpMemberInfo).SetAliasExpression(modelRuntimeCalculatedMember.AliasExpression);
            }
        }

        static void AddAttributes(IModelMemberEx modelMemberEx, XPCustomMemberInfo memberInfo) {
            if (modelMemberEx.Size != 0)
                memberInfo.AddAttribute(new SizeAttribute(modelMemberEx.Size));
            if (modelMemberEx is IModelMemberNonPersistent && !(modelMemberEx is IModelMemberCalculated))
                memberInfo.AddAttribute(new NonPersistentAttribute());
        }

        static XpandCustomMemberInfo CreateMemberInfo(IModelMemberEx modelMemberEx, XPClassInfo xpClassInfo) {
            var calculatedMember = modelMemberEx as IModelMemberCalculated;
            if (calculatedMember != null)
                return xpClassInfo.CreateCalculabeMember(calculatedMember.Name, calculatedMember.Type, calculatedMember.AliasExpression);
            var modelMemberOrphanedColection = modelMemberEx as IModelMemberOrphanedColection;
            if (modelMemberOrphanedColection != null) {
                return xpClassInfo.CreateCollection(modelMemberOrphanedColection.Name, modelMemberOrphanedColection.CollectionType.TypeInfo.Type,
                                                    modelMemberOrphanedColection.Criteria);
            }
            var modelMemberOneToManyCollection = modelMemberEx as IModelMemberOneToManyCollection;
            if (modelMemberOneToManyCollection!=null) {
                var elementType = modelMemberOneToManyCollection.CollectionType.TypeInfo.Type;
                var associationAttribute = new AssociationAttribute(modelMemberOneToManyCollection.AssociationName, elementType);
                var xpandCollectionMemberInfo = xpClassInfo.CreateCollection(modelMemberOneToManyCollection.Name, elementType, null, associationAttribute);
                modelMemberOneToManyCollection.AssociatedMember.ModelClass.TypeInfo.FindMember(modelMemberOneToManyCollection.AssociatedMember.Name).AddAttribute(associationAttribute);
                return xpandCollectionMemberInfo;
            }
            var modelMemberModelMember = modelMemberEx as IModelMemberModelMember;
            if (modelMemberModelMember != null){
                var memberInfo = ModelMemberModelMemberDomainLogic.Get_MemberInfo(modelMemberModelMember);
                return (XpandCustomMemberInfo) xpClassInfo.FindMember(memberInfo.Name);
            }
            return xpClassInfo.CreateCustomMember(modelMemberEx.Name, modelMemberEx.Type, modelMemberEx is IModelMemberNonPersistent);
        }
    }

    public class CustomCreateMemberArgs : HandledEventArgs {
        readonly IModelMemberEx _modelMemberEx;

        public CustomCreateMemberArgs(IModelMemberEx modelMemberEx) {
            _modelMemberEx = modelMemberEx;
        }

        public IModelMemberEx ModelMemberEx {
            get { return _modelMemberEx; }
        }
    }
}