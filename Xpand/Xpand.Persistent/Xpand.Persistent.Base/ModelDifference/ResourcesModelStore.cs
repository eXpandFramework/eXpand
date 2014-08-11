using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.ModelDifference{
    public class ResourcesModelStore : ModelStoreBase{
        private readonly Assembly _assembly;
        private readonly bool _loadDefaulModel;
        private readonly string _prefix;

        public ResourcesModelStore(Assembly assembly, string prefix, bool loadDefaulModel){
            _assembly = assembly;
            _prefix = prefix;
            _loadDefaulModel = loadDefaulModel;
        }

        public ResourcesModelStore(Assembly assembly, string prefix){
            _assembly = assembly;
            _prefix = prefix;
        }

        public override bool ReadOnly{
            get { return true; }
        }

        public override string Name{
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<ResourceLoadedArgs> ResourceLoading;
        public event EventHandler<ResourceLoadedArgs> ResourceLoaded;

        public void OnResourceLoaded(ResourceLoadedArgs e){
            EventHandler<ResourceLoadedArgs> handler = ResourceLoaded;
            if (handler != null) handler(this, e);
        }

        protected void OnResourceLoading(ResourceLoadedArgs e){
            EventHandler<ResourceLoadedArgs> handler = ResourceLoading;
            if (handler != null) handler(this, e);
        }

        public override void Load(ModelApplicationBase model){
            foreach (string resourceName in _assembly.GetManifestResourceNames().Where(Predicate())){
                ReadFromResource(model, resourceName, "");
            }
        }

        private Func<string, bool> Predicate(){
            return
                s =>
                    ((s.StartsWith(_prefix) ||
                      (!(s.StartsWith(_prefix)) && s.IndexOf("." + _prefix, StringComparison.Ordinal) > -1)) &&
                     ((CanLoadDefault(s)) && s.EndsWith(".xafml")));
        }

        private bool CanLoadDefault(string s){
            if (_loadDefaulModel)
                return true;
            return !(s.EndsWith(ModelDiffDefaultName + ".xafml"));
        }

        private void ReadFromResource(ModelNode rootNode, string resourceName, string aspect){
            var resourceLoadedArgs = new ResourceLoadedArgs(resourceName);
            OnResourceLoading(resourceLoadedArgs);
            if (!(resourceLoadedArgs.Cancel)){
                if (resourceLoadedArgs.Model != null)
                    rootNode = resourceLoadedArgs.Model;
                new ModelXmlReader().ReadFromResource(rootNode, aspect, _assembly, resourceName);
                OnResourceLoaded(resourceLoadedArgs);
            }
        }

        public override string ToString(){
            return base.ToString() + "(" + Name + ")";
        }
    }

    public class ResourceLoadedArgs : CancelEventArgs{
        private readonly string _resourceName;

        public ResourceLoadedArgs(string resourceName){
            _resourceName = resourceName;
        }


        public string ResourceName{
            get { return _resourceName; }
        }

        public ModelApplicationBase Model { get; set; }
    }
}