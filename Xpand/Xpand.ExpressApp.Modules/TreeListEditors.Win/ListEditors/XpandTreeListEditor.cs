using System.Collections.Generic;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraPrinting;
using Xpand.ExpressApp.TreeListEditors.Win.Core;

namespace Xpand.ExpressApp.TreeListEditors.Win.ListEditors {
    [ListEditor(typeof(ITreeNode), true)]
    public class XpandTreeListEditor : DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditor, ITreeListEditor, IExportableEditor {
        private IPrintable printable;

        public XpandTreeListEditor(DevExpress.ExpressApp.Model.IModelListView model)
            : base(model) {
        }

        protected override object CreateControlsCore() {
            var control = base.CreateControlsCore();
            Printable = TreeList;

            return control;
        }

        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new XpandTreeListEditorModelSynchronizerList(this, Model);
        }

        public IList<PrintingSystemCommand> ExportTypes {
            get {
                IList<PrintingSystemCommand> exportTypes = new List<PrintingSystemCommand> {
                    PrintingSystemCommand.ExportXls,
                    PrintingSystemCommand.ExportHtm,
                    PrintingSystemCommand.ExportTxt,
                    PrintingSystemCommand.ExportMht,
                    PrintingSystemCommand.ExportPdf,
                    PrintingSystemCommand.ExportRtf,
                    PrintingSystemCommand.ExportGraphic
                };
                return exportTypes;
            }
        }
        public IPrintable Printable {
            get { return printable; }
            set {
                if (printable != value) {
                    printable = value;
                    OnPrintableChanged();
                }
            }
        }

        public event System.EventHandler<PrintableChangedEventArgs> PrintableChanged;

        private void OnPrintableChanged() {
            if (PrintableChanged != null) {
                PrintableChanged(this, new PrintableChangedEventArgs(printable));
            }
        }
    }

}
