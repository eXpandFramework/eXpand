using System;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.Builders;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects{
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "PersistentApplication;DifferenceType", TargetCriteria = "DifferenceType=0 AND Disabled=false", SkipNullOrEmptyValues = false)]
    [CreatableItem(false)]
    [NavigationItem("Default")]
    [Custom(ClassInfoNodeWrapper.CaptionAttribute, Caption)]
    [Custom("IsClonable", "True")][VisibleInReports(false)]
    public class ModelDifferenceObject : DifferenceObject, IXpoModelDifference
    {
        public const string Caption = "Application Difference";
        private string _preferredAspect=DictionaryAttribute.DefaultLanguage;
        private DateTime dateCreated;
        [Persistent("DifferenceType")] protected DifferenceType differenceType;
        private bool _disabled;
        private string name;

        public ModelDifferenceObject(Session session) : base(session)
        {
        }


        private PersistentApplication persistentApplication;

        


        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute,"false")]
        [Aggregated][NonCloneable]
        [ExpandObjectMembersAttribute(ExpandObjectMembers.Never)]
        [RuleRequiredField(null,DefaultContexts.Save)]
        [Association(Associations.PersistentApplicationModelDifferenceObjects)]
        public PersistentApplication PersistentApplication
        {
            get
            {
                return persistentApplication;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref persistentApplication, value);
            }
        }
        public override void AfterConstruction(){
            base.AfterConstruction();
            differenceType=DifferenceType.Model;
        }
        [VisibleInDetailView(false)]
        [Custom(ColumnInfoNodeWrapper.GroupIndexAttribute, "0")]
        [PersistentAlias("differenceType")]
        public DifferenceType DifferenceType
        {
            get { return differenceType; }
        }

        [RuleRequiredField(null, DefaultContexts.Save)]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref name, value); }
        }

        public bool Disabled
        {
            get { return _disabled; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _disabled, value); }
        }


        [RuleRequiredField(null,DefaultContexts.Save)]
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref dateCreated, value); }
        }

        [VisibleInListView(false)]
        [NonPersistent]
        [NonCloneable]
        public string PreferredAspect
        {
            get
            {
                return _preferredAspect;
            }
            set
            {
                SetPropertyValue(MethodBase.GetCurrentMethod().Name.Replace("set_", ""), ref _preferredAspect, value);
                setCurrentAspect(Model);
                setCurrentAspect(PersistentApplication.Model);
                SetModelDirty();
            }
        }

        private void setCurrentAspect(Dictionary dictionary){
            dictionary.AddAspect(CurrentLanguage,new DictionaryNode(ApplicationNodeWrapper.NodeName));
            dictionary.CurrentAspectProvider.CurrentAspect = CurrentLanguage;
        }
        [Browsable(false)][MemberDesignTimeVisibility(false)]
        public string CurrentLanguage
        {
            get{
                if (_preferredAspect==DictionaryAttribute.DefaultLanguage)
                    return _preferredAspect;
                return _preferredAspect.IndexOf(" ")>-1?_preferredAspect.Substring(0,_preferredAspect.IndexOf(" ")):_preferredAspect;
            }
        }

        [Size(-1)]
        [NonPersistent]
        [VisibleInListView(false)]
        [NonCloneable]
        public string XmlContent
        {
            get
            {
                return new DictionaryXmlWriter().GetAspectXml(CurrentLanguage, Model.RootNode);
            }
            set{
                Dictionary dictionary = GetModel();
                dictionary.Validate();
                Model.AddAspect(CurrentLanguage, new DictionaryXmlReader().ReadFromString(value));
                SetModelDirty();
            }
        }

        public Dictionary GetModel()
        {
            Dictionary dictionary = PersistentApplication.Model.Clone();
            dictionary.ResetIsModified();
            var combiner = new DictionaryCombiner(dictionary);
            combiner.AddAspects(Model);

            return dictionary;
        }



        public virtual ModelDifferenceObject InitializeMembers(string applicationName,string uniqueName){

            PersistentApplication =new QueryPersistentApplication(Session).Find(uniqueName)?? new PersistentApplication(Session){Name = applicationName,UniqueName = uniqueName};
            ModelDifferenceObjectBuilder.SetUp(this, applicationName);
            return this;
        }

        public void SetModelDirty(){
            OnChanged("Model");
        }
    }

}