using System.ComponentModel;

namespace eXpand.Persistent.Base.Taxonomies{
    public interface IObjectWithAssignments {
        IBindingList Assignments { get; }
    }
}