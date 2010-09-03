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
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;
using eXpand.Xpo;
using eXpand.ExpressApp.Core;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [CreatableItem(false), DevExpress.Persistent.Base.NavigationItem("Default"), HideFromNewMenu]
    [Custom("Caption", Caption), Custom("IsClonable", "True"), VisibleInReports(false)]
    public class ModelDifferenceObject : eXpandCustomObject, IXpoModelDifference
    {
        

        public const string Caption = "Application Difference";
        DifferenceType _differenceType;
        bool _disabled;
        int combineOrder;
        DateTime dateCreated;
        string _name;
        PersistentApplication persistentApplication;

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
            return layer;
        }
        [VisibleInListView(false)]
        [NonPersistent]
        public ModelApplicationBase Model {
            get {
                throw new NotImplementedException();
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

        [RuleUniqueValue("ModelDiffsObject_Uniq_Name", DefaultContexts.Save)]
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
                return null;
//                return Model.Xml;
            }
            set {
//                ModelApplicationBase masterModel = ModelDifferenceModule.MasterModel;
//                var modelApplicationBase = masterModel.CreatorInstance.CreateModelApplication();
//                masterModel.AddLayerBeforeLast(modelApplicationBase);
//                var modelXmlReader = new ModelXmlReader();
//                modelXmlReader.ReadFromModel(modelApplicationBase,Model,s => s!=Model.CurrentAspect);
//                if (!(string.IsNullOrEmpty(value)))
//                    modelXmlReader.ReadFromString(modelApplicationBase,Model.CurrentAspect,value);
//                Model = modelApplicationBase;
                OnChanged("XmlContent");
            }
        }

        #endregion

        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }


        public virtual ModelDifferenceObject InitializeMembers(string name, string applicationTitle, string uniqueName,
                                                               bool cretateModelInstance) {
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) { Name = applicationTitle, UniqueName = uniqueName };
            DateCreated = DateTime.Now;
            Name = name;
            var aspectObject = new AspectObject(Session) { Name = CaptionHelper.DefaultLanguage };
            AspectObjects.Add(aspectObject);
//            if (cretateModelInstance) {
//                var masterModel = ModelDifferenceModule.MasterModel;
//                var modelApplication = masterModel.CreatorInstance.CreateModelApplication();
//                modelApplication.Id = name;
//                masterModel.AddLayerBeforeLast(modelApplication);
//                Model=modelApplication;
//            });
            return this;
        }
        public virtual ModelDifferenceObject InitializeMembers(string name, string applicationTitle, string uniqueName)
        {
            return InitializeMembers(name,applicationTitle, uniqueName, true);
        }

        public ModelDifferenceObject InitializeMembers(string name) {
            return InitializeMembers(name, ModuleBase.Application.Title, ModuleBase.Application.GetType().FullName);
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
                        AspectObjects.Add(aspectObject);
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

        string GetAspectName(AspectObject aspectObject) {
            return aspectObject.Name==CaptionHelper.DefaultLanguage ? "" : aspectObject.Name;
        }

        string  GetAspectName(string name) {
            return name=="" ? CaptionHelper.DefaultLanguage : name;
        }
    }

}