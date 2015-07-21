using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.PropertyEditors.ASPxSearchLookupPropertyEditor{
    [NavigationItem]
    public class ASPxSearchLookupPropertyEditorLookupObject : BaseObject{
        private string _name;

        public ASPxSearchLookupPropertyEditorLookupObject(Session session) : base(session){
        }

        
        public string Name{
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }
    }

    [DefaultClassOptions]
    public class ASPxSearchLookupPropertyEditorObject : BaseObject{
        private ASPxSearchLookupPropertyEditorLookupObject _asPxSearchLookupPropertyEditorLookup;

        public ASPxSearchLookupPropertyEditorObject(Session session) : base(session){
        }

        [RuleRequiredField]
        public ASPxSearchLookupPropertyEditorLookupObject ASPxSearchLookupPropertyEditorLookup{
            get { return _asPxSearchLookupPropertyEditorLookup; }
            set{
                SetPropertyValue("ASPxSearchLookupPropertyEditorLookup", ref _asPxSearchLookupPropertyEditorLookup,
                    value);
            }
        }
    }
}