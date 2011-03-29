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
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [RuleCombinationOfPropertiesIsUnique("MDO_Unique_Name_Application", DefaultContexts.Save, "Name,PersistentApplication")]
    [CreatableItem(false), NavigationItem("Default"), HideFromNewMenu]
    [Custom("Caption", Caption), Custom("IsClonable", "True"), VisibleInReports(false)]
    public class ModelDifferenceObject : XpandCustomObject, IXpoModelDifference,ISupportSequenceObject {
        public const string Caption = "Application Difference";
        DifferenceType _differenceType;
        bool _disabled;
        int combineOrder;
        DateTime dateCreated;
        string _name;
        PersistentApplication persistentApplication;
        ModelApplicationBase _currentModel;
        string _preferredAspect;

        public ModelDifferenceObject(Session session)
            : base(session) {
        }
        public override string ToString() {
            return persistentApplication != null ? persistentApplication.Name + " " + Name : base.ToString();
        }

        [Association("ModelDifferenceObject-AspectObjects")]
        [Aggregated]
        public XPCollection<AspectObject> AspectObjects {
            get { return GetCollection<AspectObject>("AspectObjects"); }
        }

        public virtual IEnumerable<ModelApplicationBase> GetAllLayers(ModelApplicationBase master) {
            return GetAllLayers(new QueryModelDifferenceObject(Session).GetActiveModelDifferences(persistentApplication.UniqueName, null).Where(differenceObject => differenceObject.Oid != Oid), master);
        }

        protected IEnumerable<ModelApplicationBase> GetAllLayers(IEnumerable<ModelDifferenceObject> differenceObjects, ModelApplicationBase master) {
            if (GetttingNonAppModels(differenceObjects))
                differenceObjects = differenceObjects.Where(o => o.CombineOrder < CombineOrder);
            var modelApplicationBases = differenceObjects.Distinct().Select(differenceObject => differenceObject.GetModel(master));
            modelApplicationBases = modelApplicationBases.Concat(new List<ModelApplicationBase> { GetModel(master) });
            return modelApplicationBases;
        }

        bool GetttingNonAppModels(IEnumerable<ModelDifferenceObject> differenceObjects) {
            return differenceObjects.Where(o => o is UserModelDifferenceObject || o is RoleModelDifferenceObject).Count() > 0;
        }
        [ImmediatePostData]
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [NonPersistent]
        [NonCloneable]
        public string PreferredAspect {
            get { return _preferredAspect??CaptionHelper.DefaultLanguage; }
            set {
                SetPropertyValue("PreferredAspect", ref _preferredAspect, value);
            }
        }
        protected override void OnChanged(string propertyName, object oldValue, object newValue) {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName=="PreferredAspect") {
                
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
            get { return combineOrder; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref combineOrder, value); }
        }
        [NonCloneable]
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        [RuleRequiredField(null, DefaultContexts.Save)]
        [Association(Associations.PersistentApplicationModelDifferenceObjects)]
        public PersistentApplication PersistentApplication {
            get { return persistentApplication; }
            set {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref persistentApplication, value);
            }
        }

        #region IXpoModelDifference Members

        [Custom("GroupIndex", "0"), NonCloneable, VisibleInDetailView(false)]
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

        [Custom("AllowEdit", "false"), RuleRequiredField(null, DefaultContexts.Save)]
        public DateTime DateCreated {
            get { return dateCreated; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref dateCreated, value); }
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
                }
                else {
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
            return AspectObjects.Where(o => o.Name == preferredAspect).SingleOrDefault();
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

        public ModelDifferenceObject InitializeMembers(string name,XafApplication application) {
            return InitializeMembers(name, application.Title, application.GetType().FullName);
        }


        public void CreateAspects(ModelApplicationBase model, ModelApplicationBase master) {
            var applicationBase = GetModel(master);
            new ModelXmlReader().ReadFromModel(applicationBase, model);
            CreateAspectsCore(model);
            if (applicationBase != null) master.RemoveLayer(applicationBase);
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
            return AspectObjects.Where(o => o.Name == GetAspectName(modelApplicationBase.CurrentAspect)).FirstOrDefault();
        }

        long ISupportSequenceObject.Sequence {
            get { return combineOrder; }
            set { combineOrder=(int) value; }
        }

        string ISupportSequenceObject.Prefix {
            get { return null; }
        }
    }
}