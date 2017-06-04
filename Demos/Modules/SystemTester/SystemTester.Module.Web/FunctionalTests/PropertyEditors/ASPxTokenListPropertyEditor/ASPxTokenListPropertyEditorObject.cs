using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.Persistent.Base.General;

namespace SystemTester.Module.Web.FunctionalTests.PropertyEditors.ASPxTokenListPropertyEditor{
    [NavigationItem("PropertyEditors")]
    public class ASPxTokenListPropertyEditorObject : BaseObject{

        public ASPxTokenListPropertyEditorObject(Session session) : base(session){
        }

        string _name;

        public string Name{
            get{ return _name; }
            set{ SetPropertyValue(nameof(Name), ref _name, value); }
        }

        [VisibleInDetailView(false)]
        public string ChildsText {
            get{ return string.Join(",", Childs.Select(o => o.Name)) ; }
            
        }
        [Association("ASPxTokenListPropertyEditorObject-ASPxTokenListPropertyEditorChildObjects")]
        [EditorAlias(EditorAliases.TokenList)]
        public XPCollection<ASPxTokenListPropertyEditorChildObject> Childs => GetCollection<ASPxTokenListPropertyEditorChildObject>(nameof(Childs));
    }

    
    public class ASPxTokenListPropertyEditorChildObject : BaseObject{
        

        public ASPxTokenListPropertyEditorChildObject(Session session) : base(session){
        }

        string _name;

        public string Name{
            get{ return _name; }
            set{ SetPropertyValue(nameof(Name), ref _name, value); }
        }
        ASPxTokenListPropertyEditorObject  _aSPxTokenListPropertyEditorObject ;

        [Association("ASPxTokenListPropertyEditorObject-ASPxTokenListPropertyEditorChildObjects")]
        public ASPxTokenListPropertyEditorObject  ASPxTokenListPropertyEditorObject {
            get{ return _aSPxTokenListPropertyEditorObject ; }
            set{ SetPropertyValue(nameof(ASPxTokenListPropertyEditorObject ), ref _aSPxTokenListPropertyEditorObject , value); }
        }
    }
}