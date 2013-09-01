using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace HtmlPropertyEditorTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class TestClass : BaseObject {
        
        string _text;

        public TestClass(Session session) : base(session) {
        }

        [Size(SizeAttribute.Unlimited)]
        [EditorAlias(EditorAliases.HtmlPropertyEditor)]
        public string Text {
            get { return _text; }
            set { SetPropertyValue("Text", ref _text, value); }
        }
    }
}