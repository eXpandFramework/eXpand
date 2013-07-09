using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.MapView.Web
{
    [ListEditor(typeof(IGeoCoded), true)]
    public class MapListEditor : ListEditor
    {
        public MapListEditor(IModelListView model) : base(model)
        {
            
        }

        protected override void AssignDataSourceToControl(object dataSource)
        {
        }

        public override DevExpress.ExpressApp.Templates.IContextMenuTemplate ContextMenuTemplate
        {
            get { return null; }
        }

        protected override object CreateControlsCore()
        {
            return new Artem.Google.UI.GoogleMap();
        }

        public override System.Collections.IList GetSelectedObjects()
        {
            return new object[0];
        }

        public override void Refresh()
        {
        }

        public override DevExpress.ExpressApp.SelectionType SelectionType
        {
            get { return SelectionType.FocusedObject; }
        }
    }
}
