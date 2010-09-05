using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.Attributes;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [RuleCombinationOfPropertiesIsUnique("MDO_Unique_Name_Application",DefaultContexts.Save, "Name,PersistentApplication")]
    [CreatableItem(false), NavigationItem("Default"), HideFromNewMenu]
    [Custom("Caption", Caption), Custom("IsClonable", "True"), VisibleInReports(false)]
    public class ModelDifferenceObject : XpandCustomObject, IXpoModelDifference
    {
        public const string Caption = "Application Difference";
        DifferenceType _differenceType;
        bool _disabled;
        int combineOrder;
        DateTime dateCreated;
        string _name;
        PersistentApplication persistentApplication;
        ModelApplicationBase _currentModel;
        string _preferredAspect;

        public ModelDifferenceObject(Session session) : base(session) {
        }

        [Association("ModelDifferenceObject-AspectObjects")]
        [Aggregated]
        public XPCollection<AspectObject> AspectObjects {
            get { return GetCollection<AspectObject>("AspectObjects"); }
        }

        public virtual ModelApplicationBase[] GetAllLayers(ModelApplicationBase master)
        {
            return GetAllLayers(new List<ModelDifferenceObject>().AsEnumerable(),master);
        }

        protected ModelApplicationBase[] GetAllLayers(IEnumerable<ModelDifferenceObject> differenceObjects, ModelApplicationBase master) {
            var layers = differenceObjects.Select(differenceObject => differenceObject.GetModel(master)).ToList();
            layers.Add(GetModel(master));
            return layers.ToArray();
        }
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [NonPersistent]
        [NonCloneable]
        public string PreferredAspect
        {
            get { return _preferredAspect; }
            set
            {
                SetPropertyValue("PreferredAspect", ref _preferredAspect, value);
                _currentModel.CurrentAspectProvider.CurrentAspect = value;
            }
        }
        public ModelApplicationBase GetModel(ModelApplicationBase master)
        {
            if (!master.IsMaster){
                throw new ArgumentException("IsNotMaster","master");
            }
            Guard.ArgumentNotNull(Name,"Name");
            var layer = master.CreatorInstance.CreateModelApplication();
            layer.Id = Name;
            master.AddLayerBeforeLast(layer);
            var modelXmlReader = new ModelXmlReader();
            foreach (var aspectObject in AspectObjects) {
                if (!(string.IsNullOrEmpty(aspectObject.Xml)))
                    modelXmlReader.ReadFromString(layer,GetAspectName(aspectObject),aspectObject.Xml);
            }
            _currentModel = layer;
            
            return layer;
        }

        public void NotifyXmlContent()
        {
            OnChanged("XmlContent");    
        }
        [VisibleInListView(false)]
        [NonPersistent]
        public ModelApplicationBase Model {
            get {
                return null;
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
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref persistentApplication,value);
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
        [NonPersistent,   VisibleInListView(false)]
        public string XmlContent {
            get {
                if (_currentModel != null) return _currentModel.Xml;
                return null;
            }
            set {
                var currentModel = _currentModel;
                if (currentModel == null) throw new ArgumentNullException();
                var aspectObject = GetActiveAspect(currentModel);
                if (aspectObject == null) {
                    aspectObject = new AspectObject(Session){Name = currentModel.CurrentAspect};
                    AspectObjects.Add(aspectObject);
                }
                aspectObject.Xml = value;
                OnChanged("XmlContent",XmlContent,value);
            }
        }

        #endregion

        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }


        public virtual ModelDifferenceObject InitializeMembers(string name, string applicationTitle, string uniqueName)
        {
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) { Name = applicationTitle, UniqueName = uniqueName };
            DateCreated = DateTime.Now;
            Name = name;
            var aspectObject = new AspectObject(Session) { Name = CaptionHelper.DefaultLanguage };
            AspectObjects.Add(aspectObject);
            return this;
        }

        public ModelDifferenceObject InitializeMembers(string name) {
            return InitializeMembers(name, XpandModuleBase.Application.Title, XpandModuleBase.Application.GetType().FullName);
        }

        public void CreateAspects(ModelApplicationBase model) {
            var master = (ModelApplicationBase) model.Master;
            ModelApplicationBase applicationBase = null;
            if (model.Id != DifferenceType.User.ToString()){
                applicationBase = GetModel(master);
                new ModelXmlReader().ReadFromModel(applicationBase,model);
                UpdateAspects(applicationBase);
            }
            CreateAspectsCore(model);
            if (applicationBase != null) master.RemoveLayer(applicationBase);
        }

        public void UpdateAspects(ModelApplicationBase model) {
            var modelXmlWriter = new ModelXmlWriter();
            for (int i = 0; i < model.AspectCount; i++){
                var xml = modelXmlWriter.WriteToString(model, i);
                if (!(string.IsNullOrEmpty(xml))){
                    AspectObjects.Filter = CriteriaOperator.Parse("Name=?", GetAspectName(model.GetAspect(i)));
                    if (AspectObjects.Count == 1){
                        var aspectObject = AspectObjects[0];
                        aspectObject.Xml = xml;
                    }
                    AspectObjects.Filter = null;
                }
            }

        }

        void CreateAspectsCore(ModelApplicationBase model) {
            var modelXmlWriter = new ModelXmlWriter();
            for (int i = 0; i < model.AspectCount; i++) {
                var xml = modelXmlWriter.WriteToString(model,i);
                if (!(string.IsNullOrEmpty(xml))) {
                    AspectObjects.Filter = CriteriaOperator.Parse("Name=?", GetAspectName(model.GetAspect(i)));
                    if (AspectObjects.Count==0) {
                        var aspectObject =new AspectObject(Session) {Name = model.GetAspect(i), Xml = xml};
                        AspectObjects.Add(aspectObject);
                    }
                    AspectObjects.Filter = null;
                }
            }
        }

        public string GetAspectName(AspectObject aspectObject) {
            return aspectObject.Name==CaptionHelper.DefaultLanguage ? "" : aspectObject.Name;
        }

        string  GetAspectName(string name) {
            return name=="" ? CaptionHelper.DefaultLanguage : name;
        }

        AspectObject GetActiveAspect(ModelApplicationBase modelApplicationBase) {
            return AspectObjects.Where(o => o.Name == GetAspectName(modelApplicationBase.CurrentAspect)).FirstOrDefault();
        }
    }

}