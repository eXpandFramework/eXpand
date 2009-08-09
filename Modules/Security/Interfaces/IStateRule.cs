using DevExpress.ExpressApp;
using eXpand.ExpressApp.Security.Permissions;

namespace eXpand.ExpressApp.Security.Interfaces
{
    public interface IStateRule
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

        State State { get; set; }
        
    }
}