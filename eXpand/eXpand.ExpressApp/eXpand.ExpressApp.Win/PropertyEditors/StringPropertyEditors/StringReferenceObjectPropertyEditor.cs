using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using eXpand.Xpo;

namespace eXpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors{
    public class StringReferenceObjectPropertyEditor : StringPropertyEditorBase
    {
        private List<ComboBoxItem> comboBoxItems;

        public StringReferenceObjectPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }

        protected override List<ComboBoxItem> ComboBoxItems{
            get{
                if (comboBoxItems== null){
                    comboBoxItems=new List<ComboBoxItem>();
                    PropertyInfo propertyInfo = ReflectorHelper.GetPropertyInfo(ObjectType, PropertyName);
                    var xpView = new XPView(helper.ObjectSpace.Session, propertyInfo.PropertyType);
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