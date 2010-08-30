using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;

namespace eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors{
    public class _StringLookupPropertyEditor : StringPropertyEditorBase{
        private List<ComboBoxItem> comboBoxItems;

        public _StringLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        protected override List<ComboBoxItem> ComboBoxItems
        {
            get
            {
                if (comboBoxItems == null){
                    var xpView = new XPView(helper.ObjectSpace.Session, ObjectType);
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