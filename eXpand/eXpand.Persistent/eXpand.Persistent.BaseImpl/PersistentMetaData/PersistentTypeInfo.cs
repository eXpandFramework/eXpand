using System.Collections.Generic;
<<<<<<< HEAD
=======
using DevExpress.ExpressApp.NodeWrappers;
>>>>>>> CodeDomApproachForWorldCreator
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;
using eXpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using eXpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

<<<<<<< HEAD
namespace eXpand.Persistent.BaseImpl.PersistentMetaData
{
    public abstract class PersistentTypeInfo : BaseObject, IPersistentTypeInfo {
        
        protected PersistentTypeInfo(Session session) : base(session) { }

        
        
        string _name;
        
        [RuleRequiredField(null,DefaultContexts.Save)]
=======
namespace eXpand.Persistent.BaseImpl.PersistentMetaData {

    public abstract class PersistentTypeInfo : BaseObject, IPersistentTypeInfo {
        
        string _name;

        protected PersistentTypeInfo(Session session) : base(session) {
        }
        
        [Association("TypeAttributes")]
        [Aggregated]
        public XPCollection<PersistentAttributeInfo> TypeAttributes {
            get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); }
        }
        #region IPersistentTypeInfo Members
        
        [RuleRequiredField(null, DefaultContexts.Save)]
>>>>>>> CodeDomApproachForWorldCreator
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        IList<IPersistentAttributeInfo> IPersistentTypeInfo.TypeAttributes {
<<<<<<< HEAD
            get {
                return new ListConverter<IPersistentAttributeInfo, PersistentAttributeInfo>(TypeAttributes);
            }
        }

        [Association("TypeAttributes")][Aggregated]
        public XPCollection<PersistentAttributeInfo> TypeAttributes {
            get { return GetCollection<PersistentAttributeInfo>("TypeAttributes"); }
        }

    }
=======
            get { return new ListConverter<IPersistentAttributeInfo, PersistentAttributeInfo>(TypeAttributes); }
        }


                #endregion
    }

>>>>>>> CodeDomApproachForWorldCreator
}