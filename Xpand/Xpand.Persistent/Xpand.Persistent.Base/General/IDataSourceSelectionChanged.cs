using System.Collections;

namespace Xpand.Persistent.Base.General {
    public interface IDataSourceSelectionChanged {
        IList SelectedObjects { get; set; }
    }
}