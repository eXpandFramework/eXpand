using System.Data;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Xpand.ExpressApp.IO.Core;

namespace Xpand.Tests.Xpand.IO.InitData {
    class ColumnInfo {
        public ColumnInfo(string name, DBColumnType columnType) {
            Name = name;
            ColumnType = columnType;
        }

        public string Name { get; set; }
        public DBColumnType ColumnType { get; set; }
    }

    class InputInfo {
        readonly XPClassInfo _inputClassInfo;
        readonly UnitOfWork _inputUnitOfWork;
        readonly DataSet _dataSet;

        public InputInfo(XPClassInfo inputClassInfo, UnitOfWork inputUnitOfWork, DataSet dataSet) {
            _inputClassInfo = inputClassInfo;
            _inputUnitOfWork = inputUnitOfWork;
            _dataSet = dataSet;
        }

        public XPClassInfo InputClassInfo {
            get { return _inputClassInfo; }
        }

        public UnitOfWork InputUnitOfWork {
            get { return _inputUnitOfWork; }
        }

        public DataSet DataSet {
            get { return _dataSet; }
        }
    }
    class InitDataImporterHelper {
        public static DBTable CreateDbTable(string table, params ColumnInfo[] columnInfos) {
            var dbTable = new DBTable(table);
            if (columnInfos != null)
                foreach (var columnInfo in columnInfos) {
                    dbTable.AddColumn(new DBColumn { Name = columnInfo.Name, ColumnType = columnInfo.ColumnType });
                }
            return dbTable;
        }

        public static InputInfo CreateInputClassInfo(XPDictionary outputDictionary, string inputPutClass, params ColumnInfo[] columnInfos) {
            var table = CreateDbTable(inputPutClass, columnInfos);

            var dataSet = new DataSet();
            var inputUnitOfWork = new UnitOfWork(new SimpleDataLayer(new DataSetDataStore(dataSet, AutoCreateOption.DatabaseAndSchema)));
            var dictionaryMapper = new DictionaryMapper(new[] { table });
            dictionaryMapper.Map(outputDictionary, inputUnitOfWork.Dictionary);
            var classInfo = inputUnitOfWork.Dictionary.QueryClassInfo(null, inputPutClass);
            return new InputInfo(classInfo, inputUnitOfWork, dataSet);
        }
    }
}
