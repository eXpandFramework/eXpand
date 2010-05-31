using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {

    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "PersistentApplication;DifferenceType", TargetCriteria = "DifferenceType=0 AND Disabled=false", SkipNullOrEmptyValues = false)]
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
            get { return this.Model.Xml; }
            set {
                var oldValue = XmlContent;
                if (!string.IsNullOrEmpty(value))
                    new ModelXmlReader().ReadFromString(Model, (Model.Master as ModelApplicationBase).CurrentAspect, value);
                
                OnChanged("XmlContent", oldValue, value);
            }
        }

        #endregion

        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }

        public virtual ModelDifferenceObject InitializeMembers(string applicationName, string uniqueName)
        {
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) { Name = applicationName, UniqueName = uniqueName };
            ModelDifferenceObjectBuilder.SetUp(this, applicationName);
            return this;
        }

        public virtual ModelApplicationBase[] GetAllLayers()
        {
            return this.GetAllLayers(new List<ModelDifferenceObject>().AsEnumerable());
        }

        public ModelApplicationBase[] GetAllLayers(IEnumerable<ModelDifferenceObject> differenceObjects)
        {
            var layers = new List<ModelApplicationBase>();
            foreach (ModelDifferenceObject differenceObject in differenceObjects)
            {
                layers.Add(differenceObject.Model);
            }

            layers.Add(this.Model);

            return layers.ToArray();
        }
    }
}