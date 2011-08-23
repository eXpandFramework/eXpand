using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.ImportExport {
    public enum ErrorHandling {
        DoNothing,
        CreateErrorObjects,
        ThrowException
    }

    public interface IXmlFileChooser {
        IFileData FileData { get; set; }
        ErrorHandling ErrorHandling { get; }
    }
}