using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelArtifactState.Attributes;
using eXpand.Persistent.Base.ImportExport;
using State = eXpand.ExpressApp.Security.Permissions.State;

namespace eXpand.Persistent.BaseImpl.ImportExport
{
    [Custom("DefaultListViewMasterDetailMode", "ListViewAndDetailView")]
    [ControllerStateRule("ClassInfoGraphNode+NewObjectViewController", typeof(NewObjectViewController), Nesting.Any, "1=1", "1=1", ViewType.Any, null, State.Disabled, null)]
    [ControllerStateRule("ClassInfoGraphNode+DeleteObjectsViewController", typeof(DeleteObjectsViewController), Nesting.Any, "1=1", "1=1", ViewType.Any, null, State.Disabled, null)]
    public class ClassInfoGraphNode : BaseObject, IClassInfoGraphNode
    {
        
        private string name;
        private SerializationConfiguration serializationConfiguration;
        private SerializationStrategy serializationStrategy;
        public ClassInfoGraphNode(Session session) : base(session) { }

        private NodeType _nodeType;
        [VisibleInListView(false)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute,"false")]
        public NodeType NodeType {
            get { return _nodeType; }
            set { SetPropertyValue("NodeType", ref _nodeType, value); }
        }
        [VisibleInDetailView(false)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute, "false")]
        public string Name
        {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }

        [Association]
        public SerializationConfiguration SerializationConfiguration
        {
            get { return serializationConfiguration; }
            set { SetPropertyValue("SerializationConfiguration", ref serializationConfiguration, value); }
        }

        ISerializationConfiguration IClassInfoGraphNode.SerializationConfiguration
        {
            get { return SerializationConfiguration; }
            set { SerializationConfiguration = value as SerializationConfiguration; }
        }
        [VisibleInListView(false)]
        public SerializationStrategy SerializationStrategy
        {
            get { return serializationStrategy; }
            set { SetPropertyValue("SerializationStrategy", ref serializationStrategy, value); }
        }

        private bool _key;
        [VisibleInListView(false)]
        public bool Key {
            get { return _key; }
            set { SetPropertyValue("Key", ref _key, value); }
        }

        #region ITreeNode Members
        private bool _naturalKey;
        [VisibleInListView(false)]
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute,"false")]
        public bool NaturalKey {
            get { return _naturalKey; }
            set { SetPropertyValue("NaturalKey", ref _naturalKey, value); }
        }
        private string _typeName;

        [Browsable(false)]
        public string TypeName {
            get { return _typeName; }
            set { SetPropertyValue("TypeName", ref _typeName, value); }
        }
        #endregion
    }
}