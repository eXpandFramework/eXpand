﻿using System;
using System.IO;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.ModelDifference {
    public abstract class ModelApplicationFromStreamStoreBase {
        public virtual void Load(ModelApplicationBase model) {
            Load(model,GetAspect());
        }

        protected virtual string GetAspect() {
            return String.Empty;
        }

        public  virtual string Name {
            get { return GetType().Name; }
        }

        public virtual void  Load(ModelApplicationBase model, string aspect) {
            var stream = GetStream();
            if (stream== null)
                throw new NullReferenceException("Stream for "+GetType().FullName);
            new ModelXmlReader().ReadFromStream(model, aspect, stream);
        }

        protected abstract Stream GetStream();
    }
}