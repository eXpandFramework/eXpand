using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;

namespace eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors{
    public class StringGroupValuePropertyEditor : StringPropertyEditorBase{
        private List<ComboBoxItem> comboBoxItems;

        public StringGroupValuePropertyEditor(Type objectType, DictionaryNode info)
            : base(objectType, info)
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