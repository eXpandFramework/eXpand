using System.Collections.Generic;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands{
    public class XpandSelectRecordsCommand : BaseTableCommand {
        public const string Name = "XpandSelectRecords";
        public XpandSelectRecordsCommand() {
            HasMainParameter = true;
        }

        protected override void ProcessRow(int rowNumber, Parameter parameter, List<IGridColumn> columnIndexes, string[] cellValues, string searchTrace) {
            base.ProcessRow(rowNumber, parameter, columnIndexes, cellValues, searchTrace);
            table.GetInterface<IGridRowsSelection>().SelectRow(rowNumber);
        }
    }
}