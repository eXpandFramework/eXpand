using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace eXpand.Persistent.Base.General
{
    public interface IModelRule
    {
        string ID { get; set; }

        ViewType ViewType { get; set; }

        /// <summary>
        /// Nesting of ListView, but not works in MainWindow...(Root)...
        /// </summary>
        Nesting Nesting { get; set; }

        /// <summary>
        /// Criteria to apply when show DetailView or filled ListView 
        /// </summary>
        string NormalCriteria { get; set; }

        /// <summary>
        /// Criteria to apply when show ListView empty
        /// </summary>
        string EmptyCriteria { get; set; }

        

        string Description { get; set; }

        ITypeInfo TypeInfo { get; set; }

        string ViewId { get; set; }
        
    }
}