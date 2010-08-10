using System.IO;

namespace FeatureCenter.Module {
    public abstract class ModelApplicationFromStreamStoreBase : eXpand.ExpressApp.ModelDifference.Core.ModelApplicationFromStreamStoreBase
    {
        public override string Name
        {
            get {
                var ns = GetType().Namespace;
                return ns.Substring(ns.LastIndexOf(".")+1);
            }
        }
        protected override Stream GetStream()
        {
            return GetType().Assembly.GetManifestResourceStream(GetType(), Name+".xafml");
        }

    }
}