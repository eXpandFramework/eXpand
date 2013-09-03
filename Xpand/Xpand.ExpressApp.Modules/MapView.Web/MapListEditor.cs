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
using Xpand.ExpressApp.ListEditors;
using Xpand.ExpressApp.Web;

namespace Xpand.ExpressApp.MapView.Web
{
    [ListEditor(typeof(IMapAddress), true)]
    public class MapListEditor : ListEditor, IXpandListEditor
    {
        public MapListEditor(IModelListView model)
            : base(model)
        {

        }

        public event EventHandler<ViewControlCreatedEventArgs> ViewControlsCreated;

        protected override void AssignDataSourceToControl(object dataSource)
        {
            if (Control!=null)
                ((MapControl) Control).DataSource = dataSource;
        }

        public override DevExpress.ExpressApp.Templates.IContextMenuTemplate ContextMenuTemplate
        {
            get { return null; }
        }

        protected override object CreateControlsCore()
        {
            var mapControl =  new MapControl();
            mapControl.FocusedIndexChanged += (s, e) => OnFocusedObjectChanged();
            return mapControl;
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

        public void NotifyViewControlsCreated(XpandListView listView)
        {
            if (ViewControlsCreated != null)
                ViewControlsCreated(this, new ViewControlCreatedEventArgs(listView.IsRoot));
        }


        private  MapControl MapControl
        {
            get { return (MapControl) Control; }
        }
        public override object FocusedObject
        {
            get
            {
                if (MapControl != null)
                    return MapControl.FocusedObject;
                else
                    return null;
            }
            set
            {
            }
        }
    }
}
