using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using PropertyChanged;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.ImportExport;

namespace Xpand.Persistent.BaseImpl.ImportExport {
    [DomainComponent]
    [ImplementPropertyChanged]
    public class FileChooser :  IFileChooser {

        [FileTypeFilter("Strong Keys", 1, "*.xml")]
        public XpandFileData FileData{get; set; }


        [Tooltip("If false will raise an exception")]
        public ErrorHandling ErrorHandling { get; set; } = ErrorHandling.CreateErrorObjects;

        #region IFileChooser Members
        IFileData IFileChooser.FileData {
            get { return FileData; }
            set { FileData = value as XpandFileData; }
        }
        #endregion
    }
}