using System.ComponentModel;
using DevExpress.Xpo;
using eXpand.Persistent.Base.Taxonomies;

namespace eXpand.Persistent.BaseImpl.Taxonomies{
    public class ValueTermAssignment : GenericWeakReference<ValueTerm>, IKeyedObject {
        private ValueTerm valueTerm;

        public ValueTermAssignment(Session session)
            : base(session) {}

        public ValueTermAssignment(Session session, object target)
            : base(session, target) {}

        [Association]
        public ValueTerm ValueTerm{
            get { return valueTerm; }
            set { SetPropertyValue("ValueTerm", ref valueTerm, value); }
        }
        #region Overrides of GenericWeakReference<ValueTerm>
        [Browsable(false)]
        public override ValueTerm Owner {
            get { return ValueTerm; }
        }

        public override void EvaluatePropertyValues(){
            base.EvaluatePropertyValues();
            if (ValueTerm != null){
                if (string.IsNullOrEmpty(Key)) Key = ValueTerm.Key;
                if (string.IsNullOrEmpty(Value)) Value = ValueTerm.Name;
            } 
        }

        #endregion
        #region Implementation of IKeyedObject
        
        private string _value;
        private string key;
        private KeyedObjectValidity validity;

        public string Value{
            get { return _value; }
            set { SetPropertyValue("Value", ref _value, value); }
        }
        
        public string Key{
            get { return key; }
            set { SetPropertyValue("Key", ref key, value); }
        }

        public KeyedObjectValidity Validity{
            get { return validity; }
            set { SetPropertyValue("Validity", ref validity, value); }
        }
        #endregion

        //[Browsable(false)]
        //ITreeNode ICategorizedItem.Category {
        //    get { return ValueTerm; }
        //    set { ValueTerm = (ValueTerm)value; }
        //}
    }
}