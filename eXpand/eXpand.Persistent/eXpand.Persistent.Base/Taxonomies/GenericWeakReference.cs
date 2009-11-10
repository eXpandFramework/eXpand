using System.ComponentModel;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace eXpand.Persistent.Base.Taxonomies{
    [DefaultProperty("Caption"), NonPersistent]
    public abstract class GenericWeakReference<TObjectType> : XPWeakReference, IGenericWeakReference<TObjectType>, IGenericWeakReference where TObjectType : IXPObject {
        protected const int CaptionSize = 250;
        private string _associatedObjectName = "";
        

        protected GenericWeakReference(Session session)
            : base(session) {}

        protected GenericWeakReference(Session session, object target)
            : base(session, target) {}
        
        #region IGenericWeakReference<TObjectType> Members
        [Size(CaptionSize)]
        [Persistent]
        public string AssociatedObjectName {
            get { return _associatedObjectName.Substring(0, _associatedObjectName.Length > CaptionSize ? CaptionSize : _associatedObjectName.Length); }
            set { SetPropertyValue("AssociatedObjectName", ref _associatedObjectName, value); }
        }

        //[Size(CaptionSize)]
        //[Persistent]
        //public string Caption {
        //    get { return _caption.Substring(0, _caption.Length > CaptionSize ? CaptionSize : _caption.Length); }
        //    set { SetPropertyValue("Caption", ref _caption, value); }
        //}

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

        private const string DefaultCaptionFormat = "{AssociatedObjectName}";
              
        [Size(CaptionSize)]
        [Persistent]
        public string Caption {
            get{
                var caption = ObjectFormatter.Format(DefaultCaptionFormat, this, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty);
                return caption.Substring(0, caption.Length > CaptionSize ? CaptionSize : caption.Length); 
            }
        }
    }
}