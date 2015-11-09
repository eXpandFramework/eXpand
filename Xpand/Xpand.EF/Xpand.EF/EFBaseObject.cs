using DevExpress.ExpressApp;

namespace Xpand.ExpressApp {
    public class EFBaseObject : EFLiteObject, IXafEntityObject {
        public virtual void OnCreated() {  
        }

        public virtual void OnLoaded() {          
        }

        public virtual void OnSaving() {
        }
    }
}
