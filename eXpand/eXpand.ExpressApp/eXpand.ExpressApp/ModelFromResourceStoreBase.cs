using System;
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp {
    public abstract class ModelFromResourceStoreBase : DevExpress.ExpressApp.ModelStoreBase
    {
        public override string Name {
            get { return GetType().Name;  }
        }
        public override void Load(ModelApplicationBase model)
        {
            base.Load(model);
            Type resourceNameSpaceType = GetResourceNameSpaceType();
            Assembly assembly = GetType().Assembly;
            Stream manifestResourceStream = resourceNameSpaceType != null ? assembly.GetManifestResourceStream(resourceNameSpaceType, GetResourceName()) : assembly.GetManifestResourceStream(GetResourceName());
            if (manifestResourceStream != null){
                string readToEnd = new StreamReader(manifestResourceStream, Encoding.UTF8).ReadToEnd();
                var modelReader = new ModelXmlReader();
                modelReader.ReadFromString(model, String.Empty, readToEnd);
            }
        }

        protected abstract string GetResourceName();


        protected virtual Type GetResourceNameSpaceType() {
            return GetType();
        }
    }
}