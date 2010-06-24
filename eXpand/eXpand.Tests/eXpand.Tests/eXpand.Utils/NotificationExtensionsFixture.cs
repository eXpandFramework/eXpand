using System.ComponentModel;
using MbUnit.Framework;
using eXpand.Utils.Helpers;

namespace eXpand.Tests.eXpand.Utils
{
    [TestFixture]
    public class NotificationExtensionsFixture
    {
        [Test]
        public void SubscribeToChange()
        {
            var myClass = new MyClass();
            bool changed = false;
            myClass.SubscribeToPropertyChange(x=>x.PropertyName,sender => changed= true);
            myClass.PropertyName = "changed";
            
            Assert.AreEqual(true, changed);
        }
        [Test]
        public void Test()
        {
            var myClass = new MyClass();
            bool changedBoth = false;
            bool changed = false;
            myClass.SubscribeToPropertyChange(x => x.ChangeBothProperties, sender => changedBoth = true);
            myClass.SubscribeToPropertyChange(x => x.PropertyName, sender => changed = true);
            myClass.ChangeBothProperties = "change";

            Assert.AreEqual(true, changedBoth);
            Assert.AreEqual(true, changed);
        }

        private class MyClass:INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private string changeBothProperties;
            public string ChangeBothProperties
            {
                get { return changeBothProperties; }
                set
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(this.GetPropertyInfo(x => x.ChangeBothProperties).Name));
                        PropertyChanged.Notify(()=>PropertyName);
                    }
                    changeBothProperties = value;
                }
            }
            private string propertyName;
            public string PropertyName
            {
                get { return propertyName; }
                set
                {
                    if (PropertyChanged!= null)
                        PropertyChanged(this, new PropertyChangedEventArgs(this.GetPropertyInfo(x=>x.PropertyName).Name));
                    propertyName = value;
                }
            }
        }
    }
}