using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [ControllerStateRule("ClassInfoGraphNode+NewObjectViewController", typeof(NewObjectViewController), "1=1", "1=1", ControllerState.Disabled)]
    [ControllerStateRule("ClassInfoGraphNode+DeleteObjectsViewController", typeof(DeleteObjectsViewController), "1=1", "1=1", ControllerState.Disabled)]
    [DefaultProperty("Name")]
    public class ClassInfoGraphNode : XpandBaseCustomObject, IClassInfoGraphNode {

        private string _name;
        private SerializationConfiguration _serializationConfiguration;
        private SerializationStrategy _serializationStrategy;
        public ClassInfoGraphNode(Session session) : base(session) { }

        //[Appearance("RuleObjectCanNotBeKey", AppearanceItemType.ViewItem, null, Enabled = false, TargetItems = "Key")]
        //public bool RuleObjectCanNotBeKey()
        //{
        //    return XafTypesInfo.Instance.PersistentTypes.Any(info => info.Name == TypeName);
        //}

        private NodeType _nodeType;
        [VisibleInListView(false)]
        [ModelDefault("AllowEdit", "false")]
        public NodeType NodeType {
            get { return _nodeType; }
            set { SetPropertyValue("NodeType", ref _nodeType, value); }
        }
        [VisibleInDetailView(false)]
        [ModelDefault("AllowEdit", "false")]
        public string Name {
            get { return _name; }
            set { SetPropertyValue("Name", ref _name, value); }
        }

        [Association]
        public SerializationConfiguration SerializationConfiguration {
            get { return _serializationConfiguration; }
            set { SetPropertyValue("SerializationConfiguration", ref _serializationConfiguration, value); }
        }

        ISerializationConfiguration IClassInfoGraphNode.SerializationConfiguration {
            get { return SerializationConfiguration; }
            set { SerializationConfiguration = value as SerializationConfiguration; }
        }
        [VisibleInListView(false)]
        public SerializationStrategy SerializationStrategy {
            get { return _serializationStrategy; }
            set { SetPropertyValue("SerializationStrategy", ref _serializationStrategy, value); }
        }

        private bool _key;
        [VisibleInListView(false)]
        public bool Key {
            get { return _key; }
            set { SetPropertyValue("Key", ref _key, value); }
        }

        #region ITreeNode Members

        private string _typeName;

        [Browsable(false)]
        public string TypeName {
            get { return _typeName; }
            set { SetPropertyValue("TypeName", ref _typeName, value); }
        }
        #endregion
        
        }
    }
