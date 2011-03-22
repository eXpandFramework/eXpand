using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Xpand.ExpressApp.PropertyEditors;
using System.Linq;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    [PropertyEditor(typeof(string), false)]
    public class StringLookupPropertyEditor : StringPropertyEditorBase, IStringLookupPropertyEditor {
        private List<ComboBoxItem> comboBoxItems;

        public StringLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override List<ComboBoxItem> ComboBoxItems {
            get {
                if (comboBoxItems == null) {
                    comboBoxItems=new List<ComboBoxItem>();
                    ComboBoxItemsBuilder.Create()
                        .WithPropertyEditor(this)
                        .Build((enumerable, b) => {
                            if (b)
                                CreateItems(enumerable, ((ComboBoxEdit)Control).Properties.Items);
                            else {
                                CreateItems(enumerable, comboBoxItems);
                            }
                        });
                }
                return comboBoxItems;
            }
        }

        void CreateItems(IEnumerable<string> enumerable1, ComboBoxItemCollection boxItems) {
            boxItems.Clear();
            boxItems.AddRange(enumerable1.Select(s => new ComboBoxItem(s)).ToList());
        }

        void CreateItems(IEnumerable<string> enumerable1, List<ComboBoxItem> boxItems) {
            boxItems.Clear();
            boxItems.AddRange(enumerable1.Select(s => new ComboBoxItem(s)).ToList());
        }

    }
}