using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpo;
using DevExpress.Persistent.Base.General;
using eXpand.Persistent.Base.Interfaces;

namespace eXpand.Persistent.BaseImpl
{
    [NonPersistent]
    public abstract class Category : XFPBaseObject, ITreeNode, INamedObject
    {  
        [NonPersistent]
        protected abstract ITreeNode Parent { get; }
        [NonPersistent]
        protected abstract IBindingList Children { get; }
        
        protected Category(Session session) : base(session) { }
        
        #region ITreeNode Members
        IBindingList ITreeNode.Children { get { return Children; } }
        
        ITreeNode ITreeNode.Parent { get { return Parent; } }
        #endregion

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                SetPropertyValue("Name", ref name, value);
            }
        }        
        [Persistent]
        protected int level;
        [PersistentAlias("level")]
        public int Level {
            get { return level; }
        }

        public static List<T> GetAllTreeNodes<T>(T category) where T : ITreeNode
        {
            var memberCategories = new List<T> { category };
            GetAllTreeNodes(category, memberCategories);
            return memberCategories;
        }
        public static List<T> GetAllTreeNodes<T>(List<T> categories) where T : ITreeNode
        {
            var memberCategories = new List<T>();
            foreach (var category in categories)
            {
                memberCategories.Add(category);
                GetAllTreeNodes(category, memberCategories);
            }

            return memberCategories;
        }
        private static void GetAllTreeNodes<T>(T memberCategory, IList memberCategories) where T : ITreeNode
        {
            foreach (T child in memberCategory.Children)
            {
                GetAllTreeNodes(child, memberCategories);
                memberCategories.Add(child);
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue){
            if (propertyName == "Parent" && !IsLoading && !IsSaving){
                level = (Parent == null ? 0 : ((Category) Parent).Level + 1);
            }
        }
    }
}