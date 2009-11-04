using System.ComponentModel;
using System.Linq;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    [DefaultProperty("Caption"), NonPersistent]
    public abstract class GenericWeakReference<TObjectType> : XPWeakReference, IGenericWeakReference<TObjectType>, IGenericWeakReference where TObjectType : IXPObject {
        protected const int DisplayNameSize = 250;
        private string _associatedObjectName = "";
        private string _caption = "";

        protected GenericWeakReference(Session session)
            : base(session) {}

        protected GenericWeakReference(Session session, object target)
            : base(session, target) {}
        
        #region IGenericWeakReference<TObjectType> Members
        [Size(DisplayNameSize)]
        [Persistent]
        public string AssociatedObjectName {
            get { return _associatedObjectName.Substring(0, _associatedObjectName.Length > DisplayNameSize ? DisplayNameSize : _associatedObjectName.Length); }
            set { SetPropertyValue("AssociatedObjectName", ref _associatedObjectName, value); }
        }

        [Size(DisplayNameSize)]
        [Persistent]
        public string Caption {
            get { return _caption.Substring(0, _caption.Length > DisplayNameSize ? DisplayNameSize : _caption.Length); }
            set { SetPropertyValue("Caption", ref _caption, value); }
        }

        IXPObject IGenericWeakReference.Owner {
            get { return Owner; }
        }

        public static XPCollection<TWeakReference> GetReferencesOfTargetObject<TWeakReference>(Session session, object obj) where TWeakReference : GenericWeakReference<TObjectType> {
            var references = new XPQuery<TWeakReference>(session)
                                        .Where(wr=>wr.TargetType == session.GetObjectType(obj) 
                                                    && wr.TargetKey == KeyToString(session.GetKeyValue(obj)));
            return new XPCollection<TWeakReference>(session, references);
        }

        [Browsable(false)]
        public abstract TObjectType Owner { get; }
        public virtual void EvaluatePropertyValues(){
            if (Target != null) {
                AssociatedObjectName = Target.ToString();
                Caption = AssociatedObjectName;
            }
        }
        
        #endregion

        public XPBaseObject AssociatedObject {
            get {
                DevExpress.Xpo.Metadata.XPClassInfo info = TargetType.GetClassInfo();
                return (XPBaseObject) this.Session.GetObjectByKey(info, StringToKey(TargetKey));
            }
        }

        protected override void OnSaving() {
            EvaluatePropertyValues();
            base.OnSaving();
        }
    }
}