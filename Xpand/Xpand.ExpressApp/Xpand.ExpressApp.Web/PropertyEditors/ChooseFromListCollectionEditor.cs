using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(IList<>), false)]
    public class ChooseFromListCollectionEditor : WebPropertyEditor, IChooseFromListCollectionEditor {
        public ChooseFromListCollectionEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) {
        }

        protected override object GetControlValueCore() {
            throw new NotImplementedException();
        }

        protected override WebControl CreateEditModeControlCore() {
            throw new NotImplementedException();
        }

        protected override void ReadEditModeValueCore() {
            throw new NotImplementedException();
        }
    }
}
