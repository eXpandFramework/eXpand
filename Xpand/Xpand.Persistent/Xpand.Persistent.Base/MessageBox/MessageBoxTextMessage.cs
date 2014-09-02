using DevExpress.ExpressApp.Model;
using DevExpress.Xpo;

namespace Xpand.Persistent.Base.MessageBox {
    [NonPersistent]
    public class MessageBoxTextMessage {

        private readonly string _message;

        [ModelDefault("Caption", " ")]
        [Size(SizeAttribute.Unlimited)]
        public string Message {
            get { return _message; }
        }

        public MessageBoxTextMessage(string message) {
            _message = message;
        }
    }
}