using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.Attributes;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;
using eXpand.Utils.Helpers;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects {
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "PersistentApplication;DifferenceType",
        TargetCriteria = "DifferenceType=0 AND Disabled=false", SkipNullOrEmptyValues = false)]
    [CreatableItem(false)]
    [NavigationItem("Default")]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, Caption)]
    [Custom("IsClonable", "True")]
    [VisibleInReports(false)]
    [HideFromNewMenu]
    public class ModelDifferenceObject : DifferenceObject, IXpoModelDifference {
        public const string Caption = "Application Difference";
        DifferenceType _differenceType;
        bool _disabled;
        string _preferredAspect = DictionaryAttribute.DefaultLanguage;
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

        [Browsable(false)]
        [MemberDesignTimeVisibility(false)]
        public string CurrentLanguage {
            get {
                if (_preferredAspect == DictionaryAttribute.DefaultLanguage)
                    return _preferredAspect;
                return _preferredAspect.IndexOf(" ") > -1
                           ? _preferredAspect.Substring(0, _preferredAspect.IndexOf(" "))
                           : _preferredAspect;
            }
        }
        #region IXpoModelDifference Members
        [VisibleInDetailView(false)]
        [Custom(ColumnInfoNodeWrapper.GroupIndexAttribute, "0")]
        [NonCloneable]
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

        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        [RuleRequiredField(null, DefaultContexts.Save)]
        public DateTime DateCreated {
            get { return dateCreated; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref dateCreated, value); }
        }

        [VisibleInListView(false)]
        [NonPersistent]
        [NonCloneable]
        public string PreferredAspect {
            get { return _preferredAspect; }
            set {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _preferredAspect, value);
                setCurrentAspect(Model);
                setCurrentAspect(PersistentApplication.Model);
                OnChanged(this.GetPropertyInfo(x => x.Model).Name);
            }
        }

        [Size(-1)]
        [NonPersistent]
        [VisibleInListView(false)]
        [NonCloneable]
        public string XmlContent {
            get { return new DictionaryXmlWriter().GetAspectXml(Model.GetAspectIndex(CurrentLanguage), Model.RootNode); }
            set {
                var newDictionary = new Dictionary(new DictionaryXmlReader().ReadFromString(value),Model.Schema);
                newDictionary.Validate();
                string xmlContent = XmlContent;
                Model=newDictionary;
//                string xmlContent = XmlContent;
//                Dictionary dictionary = GetCombinedModel();
//                dictionary.CombineWith(newDictionary);
//                Model = dictionary.GetDiffs();
                OnChanged("XmlContent",xmlContent,value);
            }
        }
        #endregion
        public override void AfterConstruction() {
            base.AfterConstruction();
            _differenceType = DifferenceType.Model;
        }

        void setCurrentAspect(Dictionary dictionary) {
            dictionary.AddAspect(CurrentLanguage, new DictionaryNode(ApplicationNodeWrapper.NodeName));
            dictionary.CurrentAspectProvider.CurrentAspect = CurrentLanguage;
        }

        public virtual Dictionary GetCombinedModel() {
            return GetCombinedModel(PersistentApplication.Model.Clone());
        }

        public virtual ModelDifferenceObject InitializeMembers(string applicationName, string uniqueName) {
            PersistentApplication = new QueryPersistentApplication(Session).Find(uniqueName) ??
                                    new PersistentApplication(Session) {Name = applicationName, UniqueName = uniqueName};
            ModelDifferenceObjectBuilder.SetUp(this, applicationName);
            return this;
        }

        public Dictionary GetCombinedModel(IEnumerable<ModelDifferenceObject> differenceObjects) {
            Dictionary clone = PersistentApplication.Model.Clone();
            foreach (
                ModelDifferenceObject differenceObject in
                    differenceObjects.Where(diffsObject => diffsObject.Model != null)) {
                clone.CombineWith(differenceObject.Model);
            }

            clone.ResetIsModified();
            clone.CombineWith(Model);
            return clone;
        }

        public Dictionary GetCombinedModel(Dictionary dictionary) {
            Dictionary clone = dictionary.Clone();
            clone.ResetIsModified();
            clone.CombineWith(Model);
            return clone;
        }

        public void SetModelDirty() {
            OnChanged(this.GetPropertyInfo(x => x.Model).Name);
        }
    }
}