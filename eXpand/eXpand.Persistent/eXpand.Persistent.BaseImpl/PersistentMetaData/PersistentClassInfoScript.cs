using System.ComponentModel;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using eXpand.Persistent.Base.PersistentMetaData;

namespace eXpand.Persistent.BaseImpl.PersistentMetaData {
    [DefaultProperty("ScriptType")]
    [RuleCombinationOfPropertiesIsUnique(null, DefaultContexts.Save, "PersistentClassInfo,ScriptType")]
    public class PersistentClassInfoScript : BaseObject, IPersistentClassInfoScript {
        public PersistentClassInfoScript(Session session) : base(session) {
        }


        IPersistentClassInfo IPersistentClassInfoScript.PersistentClassInfo {
            get { return PersistentClassInfo; }
            set { PersistentClassInfo = value as PersistentClassInfo; }
        }
        private PersistentClassInfo _persistentClassInfo;
        [Browsable(false)]
        [Association("PersistentClassInfo-PersistentClassInfoScripts")]
        public PersistentClassInfo PersistentClassInfo
        {
            get
            {
                return _persistentClassInfo;
            }
            set
            {
                SetPropertyValue("PersistentClassInfo", ref _persistentClassInfo, value);
            }
        }

        private ScriptType _scriptType;
        public ScriptType ScriptType
        {
            get
            {
                return _scriptType;
            }
            set
            {
                SetPropertyValue("ScriptType", ref _scriptType, value);
            }
        }
        private string _script;
        [Size(SizeAttribute.Unlimited)]
        public string Script
        {
            get
            {
                return _script;
            }
            set
            {
                SetPropertyValue("Script", ref _script, value);
            }
        }
    }
}