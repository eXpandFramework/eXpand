namespace Xpand.Persistent.Base.ImportExport {
    public interface IIOError {
        FailReason Reason { get; set; }
        string ElementXml { get; set; }
        string InnerXml { get; set; }
    }
}