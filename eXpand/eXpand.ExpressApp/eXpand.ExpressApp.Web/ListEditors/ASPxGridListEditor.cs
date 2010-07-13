﻿using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ListEditors;

namespace eXpand.ExpressApp.Web.ListEditors
{
    public class ASPxGridListEditorSynchronizer : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditorSynchronizer
    {
        public ASPxGridListEditorSynchronizer(DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor gridListEditor, IModelListView model) : base(gridListEditor, model) {
            Add(new GridViewOptionsModelSynchronizer(gridListEditor.Grid, model));
        }
    }
    public class ASPxGridListEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor
    {
        public ASPxGridListEditor(IModelListView info) : base(info) {
        }
        protected override DevExpress.ExpressApp.Core.ModelSynchronizer CreateModelSynchronizer()
        {
            return new ASPxGridListEditorSynchronizer(this, Model);
        }
    }
}
