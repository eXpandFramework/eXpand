using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors{
    public class StringLookupPropertyEditor : StringPropertyEditorBase
    {
        private List<ComboBoxItem> comboBoxItems;

        public StringLookupPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }

        protected override List<ComboBoxItem> ComboBoxItems{
            get{
                if (comboBoxItems== null){
                    comboBoxItems=new List<ComboBoxItem>();
                    PropertyInfo propertyInfo = XpandReflectionHelper.GetPropertyInfo(ObjectType, PropertyName);
                    var xpView = new XPView(((ObjectSpace)helper.ObjectSpace).Session, propertyInfo.PropertyType);
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