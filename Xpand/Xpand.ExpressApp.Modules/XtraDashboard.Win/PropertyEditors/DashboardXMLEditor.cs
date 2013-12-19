using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.PropertyEditors;

namespace Xpand.ExpressApp.XtraDashboard.Win.PropertyEditors{
    [PropertyEditor(typeof(string), false)]
    public class DashboardXMLEditor:RichEditWinPropertyEditor{
        public DashboardXMLEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }

        public override string GetRichEditHighLightExtension(){
            return "xml";
        }
    }
}