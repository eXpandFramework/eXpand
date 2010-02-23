using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.Editors;

namespace eXpand.ExpressApp.Win.PropertyEditors
{
    public class ButtonDetailViewItem : ActionButtonDetailItem
    {    
        public ButtonDetailViewItem(Type objectType, DictionaryNode info) :
            base(objectType, info) { }
        protected override object CreateControlCore()
        {
            var button = new Button {Text = SimpleAction.Caption};
            button.Click += (sender, args) => ExecuteAction();
            return button;
        }
    }
}