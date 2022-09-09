using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Entity.Model;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Persistent.Base.RuntimeMembers;
using Xpand.Persistent.Base.RuntimeMembers.Model;
using Xpand.XAF.Modules.CloneModelView;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    

    [RuleCombinationOfPropertiesIsUnique("MDO_Unique_Name_Application", DefaultContexts.Save, nameof(Name)+"," +nameof(PersistentApplication)+","+nameof(DeviceCategory))]
    [CreatableItem(false), NavigationItem("Default"), HideFromNewMenu]
    [ModelDefault("Caption", Caption), ModelDefault("IsClonable", "True"), VisibleInReports(false)]
    [CloneModelView(CloneViewType.DetailView, "MDO_DetailView",true)]
    [CloneModelView(CloneViewType.ListView, "MDO_ListView_Tablet",true)]
    [CloneModelView(CloneViewType.ListView, "MDO_ListView_Desktop",true)]
    [CloneModelView(CloneViewType.ListView, "MDO_ListView_Mobile",true)]
    [CloneModelView(CloneViewType.ListView, "MDO_ListView_All",true)]
    [CloneModelView(CloneViewType.ListView, "MDO_ListView", true)]
    [Appearance("Disable DeviceCategory for win models", AppearanceItemType.ViewItem,
        "EndsWith([" + nameof(PersistentApplication) + "." + nameof(BaseObjects.PersistentApplication.ExecutableName) +"], '.exe')", 
        Enabled = false, TargetItems = nameof(DeviceCategory))]
    [RuleCombinationOfPropertiesIsUnique(nameof(PersistentApplication)+","+nameof(DifferenceType)+","+nameof(CombineOrder))]
    public class ModelDifferenceObject : XpandCustomObject, IXpoModelDifference {
        public  const string DefaultListViewName="MDO_ListView";
        public const string DefaultDetailViewName = "MDO_DetailView";
        public const string Caption = "Application Settings";
        DifferenceType _differenceType;
        bool _disabled;
        int _combineOrder;
        DateTime _dateCreated;
        string _name;
        PersistentApplication _persistentApplication;
        ModelApplicationBase _currentModel;
        string _preferredAspect;

        public ModelDifferenceObject(Session session)
            : base(session) {
        }
        public override string ToString() {
            return _persistentApplication != null ? _persistentApplication.Name + " " + Name : base.ToString();
        }

        [Association("ModelDifferenceObject-AspectObjects")]
        [Aggregated]
        [VisibleInDetailView(false)]
        public XPCollection<AspectObject> AspectObjects => GetCollection<AspectObject>("AspectObjects");

        public virtual IEnumerable<ModelApplicationBase> GetAllLayers(ModelApplicationBase master) {
            return GetAllLayers(new QueryModelDifferenceObject(Session).GetActiveModelDifferences(_persistentApplication.UniqueName, null)
                    .Where(differenceObject => differenceObject.Oid != Oid), master);
        }

        DeviceCategory  _deviceCategory ;
        public DeviceCategory  DeviceCategory {
            get => _deviceCategory;
            set => SetPropertyValue(nameof(DeviceCategory ), ref _deviceCategory , value);
        }

        protected IEnumerable<ModelApplicationBase> GetAllLayers(IEnumerable<ModelDifferenceObject> differenceObjects, ModelApplicationBase master) {
            var modelDifferenceObjects = differenceObjects.ToList();
            if (GetttingNonAppModels(modelDifferenceObjects))
                modelDifferenceObjects = modelDifferenceObjects.Where(o => o.CombineOrder < CombineOrder).ToList();
            var modelApplicationBases = modelDifferenceObjects.Distinct().Select(differenceObject => differenceObject.GetModel(master));
            modelApplicationBases = modelApplicationBases.Concat(new List<ModelApplicationBase> { GetModel(master) });
            return modelApplicationBases;
        }

        bool GetttingNonAppModels(IEnumerable<ModelDifferenceObject> differenceObjects) {
            return differenceObjects.Any(o => o is UserModelDifferenceObject || o is RoleModelDifferenceObject);
        }
        [ImmediatePostData]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [NonPersistent]
        [NonCloneable]
        public string PreferredAspect {
            get => _preferredAspect ?? CaptionHelper.DefaultLanguage;
            set => SetPropertyValue("PreferredAspect", ref _preferredAspect, value);
        }

        /// <summary>
        /// For interal use only, use the ModifyModel method instead or the XmlContent property
        /// </summary>
        public ModelApplicationBase GetModel(ModelApplicationBase master) {
            if (!master.IsMaster) {
                throw new ArgumentException("IsNotMaster", nameof(master));
            }
            if (master.LastLayer.Id != "After Setup")
                throw new ArgumentException("master.LastLayer", master.LastLayer.Id);
            Guard.ArgumentNotNull(Name, "Name");
            var layer = master.CreatorInstance.CreateModelApplication();
            layer.Id = $"{Name}-{DeviceCategory}";
            master.AddLayerBeforeLast(layer);
            var modelXmlReader = new ModelXmlReader();
            foreach (var aspectObject in AspectObjects) {
                if (!(string.IsNullOrEmpty(aspectObject.Xml)))
                    modelXmlReader.ReadFromString(layer, GetAspectName(aspectObject), aspectObject.Xml);
            }
            _currentModel = layer;

            return layer;
        }

        public void NotifyXmlContent() {
            OnChanged("XmlContent");
        }
        [VisibleInListView(false)]
        public ModelApplicationBase Model => _currentModel;

        public int CombineOrder {
            get => _combineOrder;
            set => SetPropertyValue(nameof(CombineOrder), ref _combineOrder, value);
        }


        [NonCloneable]
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Association(Associations.PersistentApplicationModelDifferenceObjects)]
        public PersistentApplication PersistentApplication {
            get => _persistentApplication;
            set => SetPropertyValue(nameof(PersistentApplication), ref _persistentApplication, value);
        }

        #region IXpoModelDifference Members

        [ModelDefault("GroupIndex", "0"), NonCloneable, VisibleInDetailView(false)]
        public DifferenceType DifferenceType {
            get => _differenceType;
            set => SetPropertyValue("DifferenceType", ref _differenceType, value);
        }


        [RuleRequiredField("ModelDiffsObject_Req_Name", DefaultContexts.Save)]
        public string Name {
            get => _name;
            set => SetPropertyValue(nameof(Name), ref _name, value);
        }

        public bool Disabled {
            get => _disabled;
            set => SetPropertyValue(nameof(Disabled), ref _disabled, value);
        }

        [ModelDefault("AllowEdit", "false"), RuleRequiredField(null, DefaultContexts.Save)]
        public DateTime DateCreated {
            get => _dateCreated;
            set => SetPropertyValue(nameof(DateCreated), ref _dateCreated, value);
        }
        /// <summary>
        /// For interal use only, use the ModifyModel method instead or the XmlContent property
        /// </summary>
        [Size(SizeAttribute.Unlimited)]
        [NonCloneable]
        [NonPersistent, VisibleInListView(false)][ImmediatePostData]
        public string XmlContent {
            get {
                var aspectObject = GetActiveAspect(PreferredAspect);
                return aspectObject?.Xml;
            }
            set {
                var currentModel = _currentModel;
                AspectObject aspectObject;
                string aspectObjectName;
                if (currentModel != null) {
                    aspectObject = GetActiveAspect(currentModel);
                    aspectObjectName = currentModel.CurrentAspect;
                } else {
                    aspectObject = GetActiveAspect(PreferredAspect);
                    aspectObjectName = PreferredAspect;
                }
                if (aspectObject == null) {
                    aspectObject = new AspectObject(Session) { Name = aspectObjectName };
                    AspectObjects.Add(aspectObject);
                }
                aspectObject.Xml = value;
                OnChanged("XmlContent", XmlContent, value);
            }
        }

        AspectObject GetActiveAspect(string preferredAspect) {
            return AspectObjects.SingleOrDefault(o => o.Name == preferredAspect);
        }
        #endregion
        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }


        public virtual ModelDifferenceObject InitializeMembers(string name, string applicationTitle, string uniqueName,DeviceCategory deviceCategory=DeviceCategory.All) {
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) { Name = applicationTitle, UniqueName = uniqueName };
            DateCreated = DateTime.Now;
            Name = name;
            DeviceCategory=deviceCategory;
            var aspectObject = new AspectObject(Session) { Name = CaptionHelper.DefaultLanguage };
            AspectObjects.Add(aspectObject);
            return this;
        }

        public ModelDifferenceObject InitializeMembers(string name, XafApplication application,DeviceCategory deviceCategory=DeviceCategory.All) {
            var applicationTitle = application.Title;
            var title = ((IModelOptionsModelDifference) application.Model.Options).ApplicationTitle;
            if (!string.IsNullOrEmpty(title)) {
                applicationTitle = title;
            }
            return InitializeMembers(name, applicationTitle, application.GetType().FullName,deviceCategory);
        }


        public void CreateAspects(ModelApplicationBase model, ModelApplicationBase master) {
            var applicationBase = GetModel(master);
            new ModelXmlReader().ReadFromModel(applicationBase, model);
            CreateAspectsCore(model);
            if (applicationBase != null) {
                var lastLayer = master.LastLayer;
                ModelApplicationHelper.RemoveLayer(master);
                ModelApplicationHelper.RemoveLayer(master);
                ModelApplicationHelper.AddLayer(master, lastLayer);
            }
        }

        public void CreateAspects(ModelApplicationBase model) {
            var master = (ModelApplicationBase)model.Master;
            CreateAspects(model, master);
        }


        public void CreateAspectsFromPath(string diffDefaultName){
            Tracing.Tracer.LogVerboseValue("diffDefaultName", diffDefaultName);
            var applicationFolder = PathHelper.GetApplicationFolder();
            Tracing.Tracer.LogVerboseValue("applicationfodler",applicationFolder);
            var fileModelStore = new FileModelStore(applicationFolder, diffDefaultName);
            var aspects = fileModelStore.GetAspects().Concat(new[] { "" }).ToArray();
            Tracing.Tracer.LogVerboseValue("aspects.count", aspects.Length);
            foreach (var aspect in aspects) {
                var aspectFileName = Path.Combine(applicationFolder, fileModelStore.GetFileNameForAspect(aspect));
                Tracing.Tracer.LogVerboseValue("aspectFileName", aspectFileName);
                Tracing.Tracer.LogVerboseValue("aspectFileNameExists", File.Exists(aspectFileName));
                if (File.Exists(aspectFileName)) {
                    using (Stream stream = File.Open(aspectFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        ModelXmlReader reader = new ModelXmlReader();
                        Encoding aspectFileEncoding = reader.GetStreamEncoding(stream) ?? Encoding.UTF8;
                        using (StreamReader streamReader = new StreamReader(stream, aspectFileEncoding)) {
                            var aspectName = GetAspectName(aspect);
                            CreateAspect(aspectName, streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }

        public void CreateAspectsCore(ModelApplicationBase model) {
            var modelXmlWriter = new ModelXmlWriter();
            for (int i = 0; i < model.AspectCount; i++) {
                var xml = modelXmlWriter.WriteToString(model, i);
                string name = GetAspectName(model.GetAspect(i));
                CreateAspect(name, xml);
            }
        }

        private void CreateAspect(string name, string xml){
            AspectObjects.Filter = CriteriaOperator.Parse("Name=?", name);
            if (AspectObjects.Count == 0)
                AspectObjects.Add(new AspectObject(Session){Name = name});

            AspectObjects[0].Xml = xml;
            AspectObjects.Filter = null;
        }

        public string GetAspectName(AspectObject aspectObject) {
            return aspectObject.Name == CaptionHelper.DefaultLanguage ? "" : aspectObject.Name;
        }

        string GetAspectName(string name) {
            return name == "" ? CaptionHelper.DefaultLanguage : name;
        }

        public void ModifyModel(ModelDifferenceObject modelDifferenceObject, Action<IModelApplication> action) {
            var xafApplication = ApplicationHelper.Instance.Application;
            var existingMembers = xafApplication.Model.BOModel.SelectMany(modelClass => modelClass.OwnMembers).OfType<IModelMemberEx>().ToArray();
            var modelApplicationBase = ((ModelApplicationBase)xafApplication.Model);
            var lastLayer = modelApplicationBase.LastLayer;
            ModelApplicationHelper.RemoveLayer(modelApplicationBase);
            var afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetupLayer.Id = "After Setup";
            ModelApplicationHelper.AddLayer(modelApplicationBase, afterSetupLayer);
            var mdoModel = modelDifferenceObject.GetModel(modelApplicationBase);
            ModelApplicationHelper.RemoveLayer(modelApplicationBase);
            var modelApplication = (IModelApplication)mdoModel;
            action(modelApplication);
            var modelMemberExs = modelApplication.BOModel.SelectMany(modelClass => modelClass.OwnMembers).OfType<IModelMemberEx>();
            var newMembers = modelMemberExs.Except(existingMembers).ToArray();
            if (newMembers.Any()) {
                var objectSpaceProviders = xafApplication.ObjectSpaceProviders;
                var nonThreadSafeProviders = objectSpaceProviders.OfType<XPObjectSpaceProvider>().Any(provider => !provider.ThreadSafe);
                var xpandObjectSpaceProviders = objectSpaceProviders.OfType<XpandObjectSpaceProvider>().Any();
                if (!nonThreadSafeProviders && !xpandObjectSpaceProviders)
                    throw new ProviderNotSupportedException($"Use a non ThreadSafe {nameof(XPObjectSpaceProvider)} or the {nameof(XpandObjectSpaceProvider)}");
            }
            foreach (var modelMemberEx in newMembers) {
                if (string.IsNullOrEmpty(modelMemberEx.Id()))
                    modelMemberEx.SetValue("Id", "Test");
                modelMemberEx.SetValue("IsCustom", "True");
                modelMemberEx.CreatedAtDesignTime = false;
            }

            RuntimeMemberBuilder.CreateRuntimeMembers(modelApplication);
            ModelApplicationHelper.AddLayer(modelApplicationBase, lastLayer);
        }

        AspectObject GetActiveAspect(ModelApplicationBase modelApplicationBase) {
            return AspectObjects.FirstOrDefault(o => o.Name == GetAspectName(modelApplicationBase.CurrentAspect));
        }
        
    }
}