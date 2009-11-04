using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using MbUnit.Framework;
using eXpand.Utils.Helpers;

namespace Fixtures.eXpand.Utils
{
    [TestFixture]
    public class ReflectionExtensionsFixture
    {
        public string field;
        public string StringPropertyName { get; set; }

        [Test]
        public void Property_Info_Can_Be_Found()
        {
            PropertyInfo property = this.GetPropertyInfo(p => p.StringPropertyName);

            Assert.AreEqual("StringPropertyName", property.Name);
        }

        [Test]
        public void Method_Info_Can_Be_Found()
        {
            MethodInfo methodInfo = this.GetMethodInfo(p => p.PrivateMethod());

            Assert.AreEqual("PrivateMethod", methodInfo.Name);
        }

        private void PrivateMethod()
        {
        }

        [Test]
        public void FieldInfo_can_be_Found()
        {
            FieldInfo info = this.GetFieldInfo(f => field);
            Assert.AreEqual("field", info.Name);
        }
        [Test]
        public void Notification_Event_Will_Raized_On_Property_Change()
        {
            var client = new Client();
            client.PropertyChanged += (sender, args) => { };
            var stopwatch=new Stopwatch();
            client.Name = "initialize";
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
                client.Name = "name" + i;
            long ticks = stopwatch.ElapsedMilliseconds;
            stopwatch=new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < 10000; i++)
                client.NameWithSR = "name" + i;

            Assert.AreApproximatelyEqual(ticks, stopwatch.ElapsedMilliseconds,ticks*30);
        }
        
    }

    public class Client : INotifyPropertyChanged
    {
        private string nameWithSR;
        public string NameWithSR
        {
            get { return nameWithSR; }
            set
            {
                this.SetProperty(() => NameWithSR, ref nameWithSR, value);
            }
        }
        private string name;


        public string Name
        {
            get { return name; }

            set
            {

                if (name == value)
                    return;
                name = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Name"));
            }
        }
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}