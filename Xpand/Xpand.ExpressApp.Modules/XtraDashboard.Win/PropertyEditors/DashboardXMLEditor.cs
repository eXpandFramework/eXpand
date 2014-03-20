using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;

namespace Xpand.ExpressApp.XtraDashboard.Win.PropertyEditors{
    [PropertyEditor(typeof(string), false)]
    [RichEditPropertyEditorAttribute("xml", false, true,"ControlText")]
    public class DashboardXMLEditor:RichEditWinPropertyEditor{
        public DashboardXMLEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }
    }
}