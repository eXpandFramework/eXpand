using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.Base.General{
    public interface IFileChooser {
        IFileData FileData { get; set; }
        ErrorHandling ErrorHandling { get; set; }
    }
}