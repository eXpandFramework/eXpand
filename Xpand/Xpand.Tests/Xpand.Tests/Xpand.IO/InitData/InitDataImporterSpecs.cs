using System.Data;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using Machine.Specifications;
using Xpand.ExpressApp.IO.Core;

namespace Xpand.Tests.Xpand.IO.InitData {
    [Subject(typeof(InitDataImporter))]
    public class When_importing_an_object {
        private const string InputPutClass = "InputPutClass";
        static InitDataImporter _initDataImporter;
        static UnitOfWork _outputUnitOfWork;
        static UnitOfWork _inputUnitOfWork;
        static ImportingObjectClass _importingObjectClass;
        static DataSet _dataSet;
        static XPClassInfo _inputClassInfo;
        static XPClassInfo _outputClassInfo;


        [InitialData(Name = InputPutClass)]
        public class ImportingObjectClass : BaseObject {
            public ImportingObjectClass(Session session)
                : base(session) {
            }

            [InitialData]
            public string Test { get; set; }
        }

        Establish context = () => {
            _outputClassInfo = new ReflectionClassInfo(typeof(ImportingObjectClass), new ReflectionDictionary());
            var inputInfo = InitDataImporterHelper.CreateInputClassInfo(_outputClassInfo.Dictionary, InputPutClass, new ColumnInfo("Test", DBColumnType.String));
            _inputClassInfo = inputInfo.InputClassInfo;

            _initDataImporter = new InitDataImporter();
            _outputUnitOfWork = new UnitOfWork(new SimpleDataLayer(_outputClassInfo.Dictionary, new InMemoryDataStore()));
            _inputUnitOfWork = inputInfo.InputUnitOfWork;
            _dataSet = inputInfo.DataSet;
            var newObject = _inputClassInfo.CreateNewObject(_inputUnitOfWork);
            _inputClassInfo.FindMember("Test").SetValue(newObject, "Test");
            _inputUnitOfWork.CommitChanges();
        };

        Because of = () => {
            var unitOfWork = new UnitOfWork(new SimpleDataLayer(new DataSetDataStore(_dataSet, AutoCreateOption.None)));
            _initDataImporter.Import(() => new UnitOfWork(_outputUnitOfWork.ObjectLayer),
                                     () => new UnitOfWork(unitOfWork.ObjectLayer));
        };

        It should_create_a_new_output_object = () => {
            _importingObjectClass = _outputUnitOfWork.FindObject<ImportingObjectClass>(null);
            _importingObjectClass.ShouldNotBeNull();
        };

        It should_assign_all_mapped_properties = () => _importingObjectClass.Test.ShouldEqual("Test");
    }
    [Subject(typeof(InitDataImporter))]
    public class When_input_membertype_is_different_than_output {
        private const string InputPutClass = "InputPutClass";
        static InitDataImporter _initDataImporter;
        static UnitOfWork _outputUnitOfWork;
        static UnitOfWork _inputUnitOfWork;
        static DataSet _dataSet;
        static DifferentTypeMemberClass _differentTypeMemberClass;
        static XPClassInfo _inputClassInfo;
        static XPClassInfo _outputClassInfo;


        [InitialData(Name = InputPutClass)]
        public class DifferentTypeMemberClass : BaseObject {
            public DifferentTypeMemberClass(Session session)
                : base(session) {
            }

            [InitialData]
            public string Test { get; set; }
            [InitialData]
            public MyEnum MyEnum { get; set; }
        }

        public enum MyEnum {
            one, two
        }
        Establish context = () => {
            _outputClassInfo = new ReflectionClassInfo(typeof(DifferentTypeMemberClass), new ReflectionDictionary());
            var inputInfo = InitDataImporterHelper.CreateInputClassInfo(_outputClassInfo.Dictionary, InputPutClass, new ColumnInfo("Test", DBColumnType.Int32), new ColumnInfo("MyEnum", DBColumnType.Double));
            _inputClassInfo = inputInfo.InputClassInfo;

            _initDataImporter = new InitDataImporter();
            _outputUnitOfWork = new UnitOfWork(new SimpleDataLayer(_outputClassInfo.Dictionary, new InMemoryDataStore()));
            _inputUnitOfWork = inputInfo.InputUnitOfWork;
            _dataSet = inputInfo.DataSet;
            var newObject = _inputClassInfo.CreateNewObject(_inputUnitOfWork);
            _inputClassInfo.FindMember("Test").SetValue(newObject, 2);
            _inputClassInfo.FindMember("MyEnum").SetValue(newObject, 1d);
            _inputUnitOfWork.CommitChanges();
        };

        Because of = () => {
            var unitOfWork = new UnitOfWork(new SimpleDataLayer(new DataSetDataStore(_dataSet, AutoCreateOption.None)));
            _initDataImporter.Import(() => new UnitOfWork(_outputUnitOfWork.ObjectLayer),
                                     () => new UnitOfWork(unitOfWork.ObjectLayer));
        };

        It should_convert_the_value_when_simple_type = () => {
            _differentTypeMemberClass = _outputUnitOfWork.FindObject<DifferentTypeMemberClass>(null);
            _differentTypeMemberClass.Test.ShouldEqual("2");
        };

        It should_convert_the_value_when_reference_type = () => _differentTypeMemberClass.MyEnum.ShouldEqual(MyEnum.two);
    }
}
