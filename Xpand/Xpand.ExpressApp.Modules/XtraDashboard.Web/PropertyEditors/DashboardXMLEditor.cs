using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {
    [PropertyEditor(typeof(string), EditorAliases.DashboardXMLEditor, false)]
    public class DashboardXMLEditor : ExpressApp.Web.PropertyEditors.ASPxStringPropertyEditor {
        public DashboardXMLEditor(Type objectType, IModelMemberViewItem model):base(objectType, model) {
        }
    }

}
