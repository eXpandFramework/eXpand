using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;

namespace eXpand.ExpressApp.Win.PropertyEditors
{
    public class ButtonDetailViewItem : DetailViewItem
    {    
        public ButtonDetailViewItem(Type objectType, DictionaryNode info) :
            base(objectType, info) { }
        protected override object CreateControlCore()
        {
            var button = new Button();
            button.Click += button_Click;
            return button;
            
        }
        void button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello world!");
        }

    }
}