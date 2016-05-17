using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;

namespace ModelDifferenceTester.Module.Web.Editors {
    [PropertyEditor(typeof(string),false)]
    public class SyntaxHighlightPropertyEditor :ASPxStringPropertyEditor{
        public SyntaxHighlightPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }

    }
}
