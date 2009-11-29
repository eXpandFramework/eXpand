using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    [DefaultProperty("Name"), NonPersistent]
    public abstract class BaseTaxonomy : XPLiteObject, IBaseTaxonomy, ITaxonomyManager {
        private string description;
        private bool forceReevaluationOfPaths;
        private string group;
        private string key;
        private string name;
        
        protected BaseTaxonomy(Session session) : base(session){}

        [Key]
        public string Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        public string Name{
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        public string Group{
            get { return group; }
            set { SetPropertyValue("Group", ref group, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Description{
            get { return description; }
            set { SetPropertyValue("Description", ref description, value); }
        }
        [Browsable(false), MemberDesignTimeVisibility(false)]
        public abstract IList<IValueTerm> Terms { get; }
        [Browsable(false), MemberDesignTimeVisibility(false)]
        public abstract IList<IStructuralTerm> Structure { get; }

        protected override void OnChanged(string propertyName, object oldValue, object newValue){
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == "Key" && !string.IsNullOrEmpty(key) & !IsLoading){
                forceReevaluationOfPaths = true;
            }
        }

        protected override void OnSaving(){
            if (forceReevaluationOfPaths){
                ReevaluatePaths();
            }
        }

        [Action]
        public void ReevaluatePaths(){
            foreach (IValueTerm child in Terms){
                child.EvaluateTermPropertyValues(true);
            }
            forceReevaluationOfPaths = false;
        }

        #region Implementation of ITaxonomyManager
        
        public string[] GetPathSegments(string termPath){
            return TaxonomyManager.GetPathSegments(this.Key, termPath);
        }

        public string StructurePath(string termPath){
            return TaxonomyManager.StructurePath(this.Key, termPath);
        }

        public abstract IStructuralTerm AddStructuralTerm(string termPath, string termName, Type[] types);
        public abstract IValueTerm AddValueTerm(string termPath, string termName);
        public abstract IValueTerm AddValueTerm(string termPath, string termName, IStructuralTerm structuralTerm);
        #endregion
    }
}