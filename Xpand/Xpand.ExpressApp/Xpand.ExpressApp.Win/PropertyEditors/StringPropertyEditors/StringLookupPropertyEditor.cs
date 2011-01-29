using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    [PropertyEditor(typeof(string),false)]
    public class StringLookupPropertyEditor : StringPropertyEditorBase {
        private List<ComboBoxItem> comboBoxItems;

        public StringLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override List<ComboBoxItem> ComboBoxItems {
            get {
                if (comboBoxItems == null) {
                    comboBoxItems = new List<ComboBoxItem>();
                    var xpView = new XPView(((ObjectSpace)helper.ObjectSpace).Session, MemberInfo.GetOwnerInstance(CurrentObject).GetType());
                    xpView.AddProperty(PropertyName, PropertyName, true);
                    comboBoxItems = new List<ComboBoxItem>();
                    foreach (ViewRecord record in xpView)
                        comboBoxItems.Add(new ComboBoxItem(record[0]));
                }
                return comboBoxItems;
            }
        }
    }
}