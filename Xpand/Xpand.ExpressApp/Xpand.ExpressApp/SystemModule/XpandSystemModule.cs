using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Core.ReadOnlyParameters;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.NodeUpdaters;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.ExpressApp.TranslatorProviders;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers;
using Xpand.Persistent.Base.General.Controllers.Actions;
using Xpand.Persistent.Base.General.Controllers.Dashboard;
using Xpand.Persistent.Base.General.Model;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Persistent.Base.Xpo;
using Xpand.Xpo.CustomFunctions;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.SystemModule {
    [ToolboxItem(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSystemModule : XpandModuleBase, ISequenceGeneratorUser, IModelXmlConverter, IDashboardInteractionUser, IModifyModelActionUser {
        public XpandSystemModule() {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
        }

        static XpandSystemModule() {
            ParametersFactory.RegisterParameter(new MonthAgoParameter());
            TranslatorProvider.RegisterProvider(new GoogleTranslatorProvider());
            if (!InterfaceBuilder.RuntimeMode)
                new XpandXpoTypeInfoSource((TypesInfo)XafTypesInfo.Instance).AssignAsPersistentEntityStore();
        }

        public override void Setup(XafApplication application) {
            base.Setup(application);
            if (RuntimeMode) {
                application.CustomProcessShortcut+=ApplicationOnCustomProcessShortcut;
                application.ListViewCreating+=ApplicationOnListViewCreating;
                application.DetailViewCreating+=ApplicationOnDetailViewCreating;
                application.CreateCustomCollectionSource += LinqCollectionSourceHelper.CreateCustomCollectionSource;
            }
        }

        void ApplicationOnCustomProcessShortcut(object sender, CustomProcessShortcutEventArgs args) {
            new ViewShortCutProccesor(Application).Proccess(args);
        }

        void ApplicationOnDetailViewCreating(object sender, DetailViewCreatingEventArgs args) {
            if (!(args.View is XpandDetailView))
                args.View = ViewFactory.CreateDetailView((XafApplication) sender, args.ViewID, args.ObjectSpace, args.Obj, args.IsRoot);
        }

        void ApplicationOnListViewCreating(object sender, ListViewCreatingEventArgs args) {
            if (!(args.View is XpandListView))
                args.View = ViewFactory.CreateListView((XafApplication) sender, args.ViewID, args.CollectionSource, args.IsRoot);
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            new FullTextContainsFunction().Register();
            if (Application != null && Application.Security != null) {
                CreatePessimisticLockingField(typesInfo);
            }
        }

        void CreatePessimisticLockingField(ITypesInfo typesInfo) {
            var typeInfos = typesInfo.PersistentTypes.Where(info => info.FindAttribute<PessimisticLockingAttribute>() != null);
            foreach (var typeInfo in typeInfos) {
                var optimisticLockingAttribute = typeInfo.FindAttribute<OptimisticLockingAttribute>();
                if (optimisticLockingAttribute == null || optimisticLockingAttribute.Enabled)
                    typeInfo.AddAttribute(new OptimisticLockingAttribute(false));
                var memberInfo = typeInfo.FindMember(PessimisticLockingViewController.LockedUser);
                if (memberInfo == null) {
                    memberInfo = typeInfo.CreateMember(PessimisticLockingViewController.LockedUser, Application.Security.UserType);
                    memberInfo.AddAttribute(new BrowsableAttribute(false));
                }
            }
        }

        protected override void RegisterEditorDescriptors(List<EditorDescriptor> editorDescriptors) {
            editorDescriptors.Add(new PropertyEditorDescriptor(new AliasRegistration(EditorAliases.TimePropertyEditor, typeof(DateTime), false)));
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ModelListViewLinqNodesGeneratorUpdater());
            updaters.Add(new ModelListViewLinqColumnsNodesGeneratorUpdater());
            updaters.Add(new ModelMemberGeneratorUpdater());
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelMemberViewItem, IModelMemberViewItemSortOrder>();
            extenders.Add<IModelListView, IModelListViewPropertyPathFilters>();
            extenders.Add<IModelListView, IModelListViewLinq>();
            extenders.Add<IModelClass, IModelClassProccessViewShortcuts>();
            extenders.Add<IModelDetailView, IModelDetailViewProccessViewShortcuts>();
            extenders.Add<IModelOptions, IModelOptionsClientSideSecurity>();
        }

        void IModelXmlConverter.ConvertXml(ConvertXmlParameters parameters) {
            ConvertXml(parameters);
            var currentVersion = new Version(XpandAssemblyInfo.Version);
            if (currentVersion>new Version("0.0.0.0")&&currentVersion<new Version("14.1.5.2")){
                if (string.CompareOrdinal("RuntimeCalculatedColumn", parameters.XmlNodeName)==0){
                    parameters.NodeType = typeof (IModelColumn);
                    string name = parameters.Values["Id"];
                    if (parameters.Values.ContainsKey("CalcPropertyName")) {
                        name = parameters.Values["CalcPropertyName"];
                        parameters.Values.Remove("CalcPropertyName");
                    }
                    parameters.Values.Add("PropertyName",name);
                }
                if (typeof(IModelListViewPreventDataLoading).IsAssignableFrom(parameters.NodeType) && parameters.Values.ContainsKey("PreventLoadingData")) {
                    if (parameters.Values["LoadWhenFiltered"] == "True") {
                        parameters.Values["LoadWhenFiltered"] = "FilterAndCriteria";
                    }
                    else if (parameters.Values["LoadWhenFiltered"] == "False") {
                        parameters.Values["LoadWhenFiltered"] = "No";
                    }
                }
            }
            if (currentVersion > new Version("14.1.5.1")){
                if (typeof (IModelListViewPreventDataLoading).IsAssignableFrom(parameters.NodeType)){
                    if (parameters.Values.ContainsKey("LoadWhenFiltered")){
                        string value = parameters.Values["LoadWhenFiltered"];
                        if (!string.IsNullOrEmpty(value)){
                            if (value == "No")
                                parameters.Values["PreventLoadingData"] = "Never";
                            else if (value=="Filter")
                                parameters.Values["PreventLoadingData"] = "FiltersEmpty";
                            else if (value == "FilterAndCriteria")
                                parameters.Values["PreventLoadingData"] = "SearchOrFiltersOrCriteriaEmpty";
                            else{
                                parameters.Values["PreventLoadingData"] = value;
                            }
                        }
                    }
                }
            }
            if (currentVersion > new Version("14.1.5.3")){
                if (typeof (IModelListViewPreventDataLoading).IsAssignableFrom(parameters.NodeType)){
                    if (parameters.Values.ContainsKey("SearchAndFiltersEmpty")) {
                        string value = parameters.Values["SearchAndFiltersEmpty"];
                        if (!string.IsNullOrEmpty(value)){
                            parameters.Values["SearchAndFiltersEmpty"] = "FiltersEmpty";
                        }
                    }
                    if (parameters.Values.ContainsKey("SearchAndFiltersAndCriteriaEmpty")) {
                        string value = parameters.Values["SearchAndFiltersAndCriteriaEmpty"];
                        if (!string.IsNullOrEmpty(value)){
                            parameters.Values["SearchAndFiltersAndCriteriaEmpty"] = "FiltersAndCriteriaEmpty";
                        }
                    }
                }
            }
        }
    }


    public interface ITestSupport {
        bool IsTesting { get; set; }
    }
}