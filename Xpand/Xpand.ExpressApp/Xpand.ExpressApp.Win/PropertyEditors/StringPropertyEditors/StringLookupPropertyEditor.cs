using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Xpand.ExpressApp.PropertyEditors;
using System.Linq;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {
    [PropertyEditor(typeof(string), false)]
    public class StringLookupPropertyEditor : StringPropertyEditorBase, IStringLookupPropertyEditor {
        public event EventHandler<HandledEventArgs> ItemsCalculating;

        protected virtual void OnItemsCalculating(HandledEventArgs e) {
            EventHandler<HandledEventArgs> handler = ItemsCalculating;
            if (handler != null) handler(this, e);
        }
        public StringLookupPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
        
        protected override List<ComboBoxItem> ComboBoxItems {
            get {
                var comboBoxItems = new List<ComboBoxItem>();
                ComboBoxItemsBuilder.Create()
                    .WithPropertyEditor(this)
                    .Build((enumerable, b) => {
                        if (enumerable != null) {
                            if (b)
                                CreateItems(enumerable, ((ComboBoxEdit)Control).Properties.Items);
                            else {
                                CreateItems(enumerable, comboBoxItems);
                            }
                        }
                    }, () => {
                        var handledEventArgs = new HandledEventArgs();
                        OnItemsCalculating(handledEventArgs);
                        return handledEventArgs.Handled;
                    });
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
        #region Implementation of IObjectSpaceHolder
        public IObjectSpace ObjectSpace {
            get { return Helper.ObjectSpace; }
        }
        #endregion
    }
}