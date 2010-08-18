using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;
using eXpand.Xpo;

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

        public virtual ModelApplicationBase[] GetAllLayers()
        {
            return GetAllLayers(new List<ModelDifferenceObject>().AsEnumerable());
        }

        public ModelApplicationBase[] GetAllLayers(IEnumerable<ModelDifferenceObject> differenceObjects) {
            var layers = differenceObjects.Select(differenceObject => differenceObject.Model).ToList();
            layers.Add(Model);
            return layers.ToArray();
        }


        [VisibleInListView(false)]
        [Persistent, Delayed, Size(SizeAttribute.Unlimited), ValueConverter(typeof(ModelValueConverter))]
        public ModelApplicationBase Model {
            get {
                
                var modelApplicationBase = GetDelayedPropertyValue<ModelApplicationBase>("Model");
                modelApplicationBase.Id = Name;
                return modelApplicationBase;
            }
            set {
                SetDelayedPropertyValue("Model", value);
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
            get { return Model.Xml; }
            set {
                ModelApplicationBase masterModel = ModelDifferenceModule.MasterModel;
                var modelApplicationBase = masterModel.CreatorInstance.CreateModelApplication();
                masterModel.AddLayerBeforeLast(modelApplicationBase);
                for (int i = 0; i < Model.GetAspectNames().ToList().Count; i++) {
                    var aspect = Model.GetAspect(i);
                    if (aspect!=Model.CurrentAspect) {
                        var xml = new ModelXmlWriter().WriteToString(Model,i);
                        if (!(string.IsNullOrEmpty(xml)))
                            new ModelXmlReader().ReadFromString(modelApplicationBase, aspect,xml);
                    }
                }
                if (!(string.IsNullOrEmpty(value)))
                    new ModelXmlReader().ReadFromString(modelApplicationBase,Model.CurrentAspect,value);
                Model = modelApplicationBase;
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
            if (cretateModelInstance) {
                var masterModel = ModelDifferenceModule.MasterModel;
                var modelApplication = masterModel.CreatorInstance.CreateModelApplication();
                modelApplication.Id = name;
                masterModel.AddLayerBeforeLast(modelApplication);
                Model=modelApplication;
            }
            return this;
        }
        public virtual ModelDifferenceObject InitializeMembers(string name, string applicationTitle, string uniqueName)
        {
            return InitializeMembers(name,applicationTitle, uniqueName, true);
        }

        public ModelDifferenceObject InitializeMembers(string name) {
            return InitializeMembers(name, ModuleBase.Application.Title, ModuleBase.Application.GetType().FullName);
        }
    }

}