using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [CreatableItem(false), NavigationItem("Default"), HideFromNewMenu]
    [Custom("Caption", Caption), Custom("IsClonable", "True"), VisibleInReports(false)]
    public class ModelDifferenceObject : DifferenceObject, IXpoModelDifference {
        public const string Caption = "Application Difference";
        DifferenceType _differenceType;
        bool _disabled;
        int combineOrder;
        DateTime dateCreated;
        string name;
        PersistentApplication persistentApplication;

        public ModelDifferenceObject(Session session) : base(session) {
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
            get { return name; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref name, value); }
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

        [Size(SizeAttribute.Unlimited), NonPersistent, NonCloneable, VisibleInListView(false), ImmediatePostData]
        public string XmlContent {
            get { return Model.Xml; }
            set {
                var oldValue = XmlContent;
                if (!string.IsNullOrEmpty(value))
                    new ModelXmlReader().ReadFromString(Model, ((ModelApplicationBase) Model.Master).CurrentAspect, value);
                
                OnChanged("XmlContent", oldValue, value);
            }
        }

        #endregion

        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }
        private string _modelId;

        [Browsable(false)]
        public string ModelId {
            get { return _modelId; }
            set { SetPropertyValue("ModelId", ref _modelId, value); }
        }

        private  ModelDifferenceObject InitializeMembers(string applicationName, string uniqueName, string modelId)
        {
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) { Name = applicationName, UniqueName = uniqueName };
            DateCreated = DateTime.Now;
            Name = "AutoCreated " + DateTime.Now;
            ModelId = modelId;
            Model = ModuleBase.ModelApplicationCreator.CreateModelApplication();
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

        public virtual ModelDifferenceObject InitializeMembers(string modelId)
        {
            return InitializeMembers(ModuleBase.Application.Title, ModuleBase.Application.GetType().FullName, modelId);
        }
    }
}