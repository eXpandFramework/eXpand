using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core.DictionaryHelpers;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.WorldCreator.Core;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.ExpressApp.WorldCreator {
    public abstract class WorldCreatorModuleBase:ModuleBase {
        string _connectionString;

        List<Type> _definedModules;

        public List<Type> DefinedModules
        {
            get { return _definedModules; }
        }

        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            if (Application == null)
                return;
            TypesInfo.Instance.AddTypes(GetAdditionalClasses());

            var unitOfWork = new UnitOfWork { ConnectionString = _connectionString };
            unitOfWork.UpdateSchema();
            AddDynamicModules(moduleManager, unitOfWork, TypesInfo.Instance.PersistentAssemblyInfoType);
            Application.SetupComplete += (sender, args) => mergeTypes(unitOfWork);
            var existentTypesMemberCreator = new ExistentTypesMemberCreator();
            existentTypesMemberCreator.CreateMembers(unitOfWork, TypesInfo.Instance);
            Application.ObjectSpaceProvider.CreateUpdatingSession().UpdateSchema();

        }

        public void AddDynamicModules(ApplicationModulesManager moduleManager, UnitOfWork unitOfWork, Type persistentAssemblyInfoType)
        {

            List<IPersistentAssemblyInfo> persistentAssemblyInfos =
                new XPCollection(unitOfWork, persistentAssemblyInfoType).Cast<IPersistentAssemblyInfo>().Where(info => !info.DoNotCompile && moduleManager.Modules.Where(@base => @base.Name == "Dynamic" + info.Name + "Module").FirstOrDefault() ==
                                                                                                                       null).ToList();
            _definedModules = new CompileEngine().CompileModules(persistentAssemblyInfos,GetPath());
            foreach (var definedModule in _definedModules)
            {
                moduleManager.AddModule(definedModule);
            }
            unitOfWork.CommitChanges();
        }

        public abstract string GetPath();

        void mergeTypes(UnitOfWork unitOfWork)
        {
            IEnumerable<Type> persistentTypes =
                _definedModules.Select(type => type.Assembly).SelectMany(
                    assembly => assembly.GetTypes().Where(type => typeof(IXPSimpleObject).IsAssignableFrom(type)));
            IDbCommand dbCommand =
                ((ISqlDataStore)XpoDefault.GetConnectionProvider(_connectionString, AutoCreateOption.DatabaseAndSchema)).CreateCommand();
            new XpoObjectMerger().MergeTypes(unitOfWork, persistentTypes.ToList(), dbCommand);
        }



        public IEnumerable<Type> GetAdditionalClasses()
        {
            return Application.Modules.SelectMany(@base => @base.AdditionalBusinessClasses);
        }

        public override void UpdateModel(Dictionary model)
        {
            base.UpdateModel(model);
            if (Application != null)
            {
                ShowOwnerForExtendedMembers(model);
                removeDynamicAssemblyFromImageSources(model);
                enableCloning(model);
            }
        }

        void enableCloning(Dictionary model)
        {

            foreach (var propertyInfo in typeof(ITypesInfo).GetProperties())
            {
                var type = (Type)propertyInfo.GetValue(TypesInfo.Instance, null);
                var classInfoNodeWrapper = new ApplicationNodeWrapper(model).BOModel.FindClassByType(type);
                classInfoNodeWrapper.Node.SetAttribute("IsClonable", true);
            }
        }


        void ShowOwnerForExtendedMembers(Dictionary dictionary)
        {
            foreach (ListViewInfoNodeWrapper listViewInfoNodeWrapper in GetListViewInfoNodeWrappers(dictionary))
            {
                ColumnInfoNodeWrapper columnInfoNodeWrapper = listViewInfoNodeWrapper.Columns.FindColumnInfo("Owner");
                if (columnInfoNodeWrapper != null) columnInfoNodeWrapper.VisibleIndex = 2;
            }
        }

        IEnumerable<ListViewInfoNodeWrapper> GetListViewInfoNodeWrappers(Dictionary dictionary)
        {
            var wrapper = new ApplicationNodeWrapper(dictionary);
            List<ListViewInfoNodeWrapper> wrappers =
                wrapper.Views.GetListViews(TypesInfo.Instance.ExtendedReferenceMemberInfoType);
            wrappers.AddRange(wrapper.Views.GetListViews(TypesInfo.Instance.ExtendedCollectionMemberInfoType));
            wrappers.AddRange(wrapper.Views.GetListViews(TypesInfo.Instance.ExtendedCoreMemberInfoType));
            return wrappers;
        }

        void removeDynamicAssemblyFromImageSources(Dictionary model)
        {
            DictionaryNode imageSourcesNode = model.RootNode.FindChildNode("ImageSources");
            foreach (Type definedModule in DefinedModules)
            {
                string name = new AssemblyName(definedModule.Assembly.FullName + "").Name;
                DictionaryElement dictionaryElement =
                    imageSourcesNode.FindChildElementByPath("AssemblyResourceImageSource[@AssemblyName='" + name + "']");
                imageSourcesNode.RemoveChildNode((DictionaryNode)dictionaryElement);
            }
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider +=
                (sender, args) => _connectionString = getConnectionStringWithOutThreadSafeDataLayerInitialization(args);
        }

        string getConnectionStringWithOutThreadSafeDataLayerInitialization(CreateCustomObjectSpaceProviderEventArgs args)
        {
            return args.Connection != null ? args.Connection.ConnectionString : args.ConnectionString;
        }
    }
}