using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;
using System.Linq;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public abstract class XpoModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore
    {
        public const string DisableDebuggerAttachedCheck = "DisableDebuggerAttachedCheck";

        protected XpoModelDictionaryDifferenceStore(Session session, XafApplication application) : base(session, application){
            
        }
        
        internal bool UseModelFromPath()
        {
            return IsDebuggerAttached && !debuggerAttachedDisabled();
        }


        private bool debuggerAttachedDisabled()
        {
            string setting = ConfigurationManager.AppSettings[DisableDebuggerAttachedCheck];
            if (string.IsNullOrEmpty(setting))
                return false;
            return setting.ToLower() == "true";
        }
        
        protected internal List<string> GetModelPaths(){
            List<string> paths = Directory.GetFiles(Path.GetDirectoryName(GetPath())).Where(
                s => s.EndsWith(".xafml")).ToList();
            return paths;
        }

        protected internal abstract string GetPath();

        protected override Dictionary LoadDifferenceCore(Schema schema)
        {
            var dictionary = new Dictionary(new DictionaryNode("Application"), schema);
                    
            if ((UseModelFromPath())){
                foreach (var s in GetModelPaths().Where(s => Path.GetFileName(s).ToLower().StartsWith("model") && s.IndexOf(".User.") == -1))
                {
                    var dictionaryNode = new DictionaryXmlReader().ReadFromFile(s);
                    string replace = s.Replace(".xafml", "");
                    string aspect = DictionaryAttribute.DefaultLanguage;
                    if (replace.IndexOf("_") > -1)
                        aspect = replace.Substring(replace.IndexOf("_") + 1);
                    dictionary.AddAspect(aspect, dictionaryNode);
                }    
                SaveDifference(dictionary);
                return dictionary;
            }
            var activeDifferenceObject = GetActiveDifferenceObject();
            if (activeDifferenceObject!= null)
                dictionary = new Dictionary(activeDifferenceObject.Model.RootNode, schema);
            
            SaveDifference(dictionary);
            return dictionary;
        }


        public override DifferenceType DifferenceType
        {
            get { return DifferenceType.Model; }
        }

        public bool IsDebuggerAttached{
            get { return Debugger.IsAttached; }
        }

        protected internal override ModelDifferenceObject GetActiveDifferenceObject(){
            return new QueryModelDifferenceObject(Session).GetActiveModelDifference(((IApplicationUniqueName) Application).UniqueName);
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(Session session)
        {
            return new ModelDifferenceObject(session);
        }
    }
}