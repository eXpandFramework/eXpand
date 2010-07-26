using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects.ValueConverters;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    
    [CreatableItem(false), NavigationItem("Default"), HideFromNewMenu]
    [Custom("Caption", Caption), Custom("IsClonable", "True"), VisibleInReports(false)]
    public class ModelDifferenceObject : eXpandCustomObject {
        public const string Caption = "Application Difference";
        DifferenceType _differenceType;
        bool _disabled;
        int combineOrder;
        DateTime dateCreated;
        string _name;
        PersistentApplication persistentApplication;

        public ModelDifferenceObject(Session session) : base(session) {
        }

        [Delayed, Size(SizeAttribute.Unlimited), ValueConverter(typeof (ModelValueConverter))]
        [RuleRequiredField]
        public ModelApplicationBase Model {
            get {
                var modelApplicationBase = GetDelayedPropertyValue<ModelApplicationBase>("Model");
                if (modelApplicationBase != null) {
                    modelApplicationBase.Id = Name;
                }
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

        [Size(SizeAttribute.Unlimited), NonPersistent, NonCloneable, VisibleInListView(false)]
        public string XmlContent {
            get { return Model.Xml; }
            set {
                var oldValue = XmlContent;
                var currentAspect = Model.CurrentAspect;
                Model=(ModelApplicationBase) new ModelValueConverter().ConvertFromStorageType("");
                if (!(string.IsNullOrEmpty(value))) new ModelXmlReader().ReadFromString(Model, currentAspect, value);
                OnChanged("XmlContent", oldValue, value);
            }
        }

        #endregion

        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }


        private  ModelDifferenceObject InitializeMembers(XafApplication application, string name)
        {
            string uniqueName = application.GetType().FullName;
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) { Name = application.Title, UniqueName = uniqueName };
            DateCreated = DateTime.Now;
            Name = name;
            return this;
        }

        public virtual ModelApplicationBase[] GetAllLayers()
        {
            return GetAllLayers(new List<ModelDifferenceObject>().AsEnumerable());
        }

        public ModelApplicationBase[] GetAllLayers(IEnumerable<ModelDifferenceObject> differenceObjects)
        {
            var layers = differenceObjects.Select(differenceObject => differenceObject.Model).ToList();
            layers.Add(Model);
            return layers.ToArray();
        }

        public virtual ModelDifferenceObject InitializeMembers(string name)
        {
            return InitializeMembers(ModuleBase.Application, name);
        }
    }
}