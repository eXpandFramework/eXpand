using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Validation;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using FeatureCenter.Module.Validation;
using Xpand.ExpressApp.Win.ListEditors;

namespace FeatureCenter.Module.Win {

    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();

        }
    }
}
