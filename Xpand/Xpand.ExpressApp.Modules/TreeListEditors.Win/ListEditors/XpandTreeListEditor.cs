using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils;
using Xpand.ExpressApp.TreeListEditors.Win.Core;

namespace Xpand.ExpressApp.TreeListEditors.Win.ListEditors {
    [ListEditor(typeof(ITreeNode), true)]
    public class XpandTreeListEditor : DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditor {
        private ObjectTreeList treeList;
        public XpandTreeListEditor(DevExpress.ExpressApp.Model.IModelListView model)
            : base(model) {
        }

        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new XpandTreeListEditorModelSynchronizerList(this, Model);
        }

        protected override object CreateControlsCore() {
            treeList = (ObjectTreeList)base.CreateControlsCore();
            treeList.NodeCellStyle += treeList_NodeCellStyle;
            return treeList;
        }

        public override void Dispose() {
            try {
                if (treeList != null) {
                    treeList.NodeCellStyle -= treeList_NodeCellStyle;
                }
            } finally {
                base.Dispose();
            }
        }

        private void treeList_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e) {
            OnViewItemCreated(new ViewItemCreatedEventArgs(e.Column.FieldName, new AppearanceViewElelemt(e.Appearance, e), ((ObjectTreeListNode)e.Node).Object));
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public class AppearanceViewElelemt : IAppearanceFormat {
        private readonly AppearanceObject appearanceObject;
        private readonly object viewElement;
        public AppearanceViewElelemt(AppearanceObject appearanceObject, object viewElement) {
            this.appearanceObject = appearanceObject;
            this.viewElement = viewElement;
        }
        #region IAppearanceFormat Members
        public FontStyle FontStyle {
            get { return appearanceObject.Font.Style; }
            set { appearanceObject.Font = new Font(appearanceObject.Font, value); }
        }
        public Color FontColor {
            get { return appearanceObject.ForeColor; }
            set { appearanceObject.ForeColor = value; }
        }
        public Color BackColor {
            get { return appearanceObject.BackColor; }
            set { appearanceObject.BackColor = value; }
        }
        #endregion
        #region IViewElementProvider Members
        public object ViewElement {
            get { return viewElement; }
        }
        #endregion
    }
}
