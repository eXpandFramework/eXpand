using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Web.PropertyEditors;

namespace Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors {
    [PropertyEditor(typeof(IList<ITypeWrapper>), true)]
    public class DashboardTypesEditor : ChooseFromListCollectionEditor {
        public DashboardTypesEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }
    }
}