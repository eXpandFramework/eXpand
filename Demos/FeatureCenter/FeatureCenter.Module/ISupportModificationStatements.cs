using System.ComponentModel;
using DevExpress.Xpo;

namespace FeatureCenter.Module {
    public interface ISupportModificationStatements {
        [NonPersistent, Browsable(false)]
        string ModificationStatements { get; set; }
    }
}