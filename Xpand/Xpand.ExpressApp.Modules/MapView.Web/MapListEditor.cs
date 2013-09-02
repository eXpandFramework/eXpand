using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web;

namespace Xpand.ExpressApp.MapView.Web
{
    [ListEditor(typeof(IGeoCoded), true)]
    public class MapListEditor : ListEditor
    {
        public MapListEditor(IModelListView model)
            : base(model)
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
            return new MapControl();
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
