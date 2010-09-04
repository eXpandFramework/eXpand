using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Web;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace eXpand.ExpressApp.ModelDifference.Web.Controllers{
    public class DataStoreKeyValueStorage : IKeyValueStorage
    {
        private static readonly object lockObj = new object();
        private static IValueManager<DataStoreKeyValueStorage> instanceManager;
        private readonly List<string> _keysToBeStoredInDataStore;
        private Session _session;
        private string _applicationName;

        public static DataStoreKeyValueStorage Instance
        {
            get
            {
                lock (lockObj){
                    if (instanceManager == null){
                        instanceManager = ValueManager.CreateValueManager<DataStoreKeyValueStorage>();
                    }
                    if (instanceManager.Value == null){
                        instanceManager.Value = new DataStoreKeyValueStorage();
                    }
                }
                return instanceManager.Value;
            }
        }
        private DataStoreKeyValueStorage()
        {
			_keysToBeStoredInDataStore = new List<string>();
		}

        public void RegisterKeyForCookies(string key,Session session,string applicationName)
        {
            _session = session;
            _applicationName = applicationName;
            lock (_keysToBeStoredInDataStore){
                if (!_keysToBeStoredInDataStore.Contains(key)){
                    _keysToBeStoredInDataStore.Add(key);
                }
            }
        }


        public string Load(string key){
            key = key.Replace("_Settings", "");
            lock (_keysToBeStoredInDataStore){
                if (_keysToBeStoredInDataStore.Contains(key)){
                    Dictionary dictionary = new QueryUserModelDifferenceObject(_session).GetActiveModelDifference(_applicationName).Model;
                    BaseViewInfoNodeWrapper wrapper = new ApplicationNodeWrapper(dictionary).Views.FindViewById(key);
                    if (wrapper != null) return new DictionaryXmlWriter().GetCurrentAspectXml(wrapper.Node);
                }
            }
            return null;
        }

        public void Save(string key, string value){
            UserModelDifferenceObject userModelDifferenceObject = new QueryUserModelDifferenceObject(_session).GetActiveModelDifference(_applicationName);
            Dictionary dictionary = userModelDifferenceObject.GetCombinedModel();
            DictionaryNode dictionaryNode = new DictionaryXmlReader().ReadFromString(value);

            var dictionary1 = new Dictionary(new DictionaryNode(ApplicationNodeWrapper.NodeName), userModelDifferenceObject.PersistentApplication.Model.Schema);
            dictionary1.RootNode.AddChildNode("Views").AddChildNode(dictionaryNode);
            dictionary.CombineWith(dictionary1);
            userModelDifferenceObject.Model = dictionary.GetDiffs();
            ObjectSpace.FindObjectSpace(userModelDifferenceObject).CommitChanges();
        }
    }
}