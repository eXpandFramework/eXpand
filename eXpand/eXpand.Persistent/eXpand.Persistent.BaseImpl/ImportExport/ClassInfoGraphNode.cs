using System.ComponentModel;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.Persistent.Base.General;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.Persistent.Base.ImportExport;

namespace eXpand.Persistent.BaseImpl.ImportExport
{

    public class ClassInfoGraphNode : BaseObject, IClassInfoGraphNode
    {
        private ClassInfoGraphNode _parent;
        private string name;
        private SerializationConfiguration serializationConfiguration;
        private SerializationStrategy serializationStrategy;
        public ClassInfoGraphNode(Session session) : base(session) { }

        private NodeType _nodeType;
        [Custom(PropertyInfoNodeWrapper.AllowEditAttribute,"false")]
        public NodeType NodeType {
            get { return _nodeType; }
            set { SetPropertyValue("NodeType", ref _nodeType, value); }
        }
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

        public SerializationStrategy SerializationStrategy
        {
            get { return serializationStrategy; }
            set { SetPropertyValue("SerializationStrategy", ref serializationStrategy, value); }
        }

        private bool _key;
        public bool Key {
            get { return _key; }
            set { SetPropertyValue("Key", ref _key, value); }
        }

        [Association]
        public ClassInfoGraphNode Parent
        {
            get { return _parent; }
            set { SetPropertyValue("Parent", ref _parent, value); }
        }

        [Association]
        public XPCollection<ClassInfoGraphNode> Children
        {
            get { return GetCollection<ClassInfoGraphNode>("Children"); }
        }
        #region ITreeNode Members
        IBindingList ITreeNode.Children
        {
            get { return Children; }
        }

        string ITreeNode.Name
        {
            get { return Name; }
        }

        ITreeNode ITreeNode.Parent
        {
            get { return Parent; }
        }
        #endregion
    }
}