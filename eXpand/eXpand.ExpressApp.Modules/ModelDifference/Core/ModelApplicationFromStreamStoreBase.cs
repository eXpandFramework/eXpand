using System;
using System.IO;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.ModelDifference.Core {
    public abstract class ModelApplicationFromStreamStoreBase {
        public virtual void Load(ModelApplicationBase model) {
            Load(model,String.Empty);
        }

        public  virtual string Name {
            get { return GetType().Name; }
        }

        public virtual void  Load(ModelApplicationBase model, string aspect) {
            new ModelXmlReader().ReadFromStream(model, aspect, GetStream());
        }

        protected abstract Stream GetStream();
    }
}