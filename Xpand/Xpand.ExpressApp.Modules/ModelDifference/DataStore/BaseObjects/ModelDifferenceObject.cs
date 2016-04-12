using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [RuleCombinationOfPropertiesIsUnique("MDO_Unique_Name_Application", DefaultContexts.Save, "Name,PersistentApplication")]
    [CreatableItem(false), NavigationItem("Default"), HideFromNewMenu]
    [ModelDefault("Caption", Caption), ModelDefault("IsClonable", "True"), VisibleInReports(false)]
    [CloneView(CloneViewType.DetailView, "MDO_DetailView",true)]
    [CloneView(CloneViewType.ListView, "MDO_ListView",true)]
    [FriendlyKeyProperty("Name")]
    public class ModelDifferenceObject : XpandCustomObject, IXpoModelDifference, ISupportSequenceObject {
        public  const string DefaultListViewName="MDO_ListView";
        public const string DefaultDetailViewName = "MDO_DetailView";
        public const string Caption = "Application Difference";
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
        public XPCollection<AspectObject> AspectObjects {
            get { return GetCollection<AspectObject>("AspectObjects"); }
        }

        public virtual IEnumerable<ModelApplicationBase> GetAllLayers(ModelApplicationBase master) {
            return GetAllLayers(new QueryModelDifferenceObject(Session).GetActiveModelDifferences(_persistentApplication.UniqueName, null).Where(differenceObject => differenceObject.Oid != Oid), master);
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
            get { return _preferredAspect ?? CaptionHelper.DefaultLanguage; }
            set {
                SetPropertyValue("PreferredAspect", ref _preferredAspect, value);
            }
        }
        public ModelApplicationBase GetModel(ModelApplicationBase master) {
            if (!master.IsMaster) {
                throw new ArgumentException("IsNotMaster", "master");
            }
            if (master.LastLayer.Id != "After Setup")
                throw new ArgumentException("master.LastLayer", master.LastLayer.Id);
            Guard.ArgumentNotNull(Name, "Name");
            var layer = master.CreatorInstance.CreateModelApplication();
            layer.Id = Name;
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
        [NonPersistent]
        public ModelApplicationBase Model {
            get {
                return _currentModel;
            }
        }


        public int CombineOrder {
            get { return _combineOrder; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _combineOrder, value); }
        }
        [NonCloneable]
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Association(Associations.PersistentApplicationModelDifferenceObjects)]
        public PersistentApplication PersistentApplication {
            get { return _persistentApplication; }
            set {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _persistentApplication, value);
            }
        }

        #region IXpoModelDifference Members

        [ModelDefault("GroupIndex", "0"), NonCloneable, VisibleInDetailView(false)]
        public DifferenceType DifferenceType {
            get { return _differenceType; }
            set { SetPropertyValue("DifferenceType", ref _differenceType, value); }
        }


        [RuleRequiredField("ModelDiffsObject_Req_Name", DefaultContexts.Save)]
        public string Name {
            get { return _name; }
            set {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _name, value);
            }
        }

        public bool Disabled {
            get { return _disabled; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _disabled, value); }
        }

        [ModelDefault("AllowEdit", "false"), RuleRequiredField(null, DefaultContexts.Save)]
        public DateTime DateCreated {
            get { return _dateCreated; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _dateCreated, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        [NonCloneable]
        [NonPersistent, VisibleInListView(false)]
        public string XmlContent {
            get {
                if (_currentModel != null) return _currentModel.Xml;
                var aspectObject = GetActiveAspect(PreferredAspect);
                return aspectObject != null ? aspectObject.Xml : null;
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
        protected override void OnSaving() {
            base.OnSaving();
            if (Session.IsNewObject(this)) {
                SequenceGenerator.GenerateSequence(this);
            }
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }


        public virtual ModelDifferenceObject InitializeMembers(string name, string applicationTitle, string uniqueName) {
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) { Name = applicationTitle, UniqueName = uniqueName };
            DateCreated = DateTime.Now;
            Name = name;
            var aspectObject = new AspectObject(Session) { Name = CaptionHelper.DefaultLanguage };
            AspectObjects.Add(aspectObject);
            return this;
        }

        public ModelDifferenceObject InitializeMembers(string name, XafApplication application) {
            return InitializeMembers(name, application.Title, application.GetType().FullName);
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


        public void CreateAspectsCore(ModelApplicationBase model) {
            var modelXmlWriter = new ModelXmlWriter();
            for (int i = 0; i < model.AspectCount; i++) {
                var xml = modelXmlWriter.WriteToString(model, i);
                string name = GetAspectName(model.GetAspect(i));
                AspectObjects.Filter = CriteriaOperator.Parse("Name=?", name);
                if (AspectObjects.Count == 0)
                    AspectObjects.Add(new AspectObject(Session) { Name = name });

                AspectObjects[0].Xml = xml;
                AspectObjects.Filter = null;
            }
        }

        public string GetAspectName(AspectObject aspectObject) {
            return aspectObject.Name == CaptionHelper.DefaultLanguage ? "" : aspectObject.Name;
        }

        string GetAspectName(string name) {
            return name == "" ? CaptionHelper.DefaultLanguage : name;
        }

        AspectObject GetActiveAspect(ModelApplicationBase modelApplicationBase) {
            return AspectObjects.FirstOrDefault(o => o.Name == GetAspectName(modelApplicationBase.CurrentAspect));
        }

        long ISupportSequenceObject.Sequence {
            get { return _combineOrder; }
            set { _combineOrder = (int)value; }
        }

        string ISupportSequenceObject.Prefix {
            get { return null; }
        }
    }
}