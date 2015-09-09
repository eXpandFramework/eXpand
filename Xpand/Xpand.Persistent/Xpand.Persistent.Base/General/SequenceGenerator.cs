using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using Fasterflect;
using Xpand.Persistent.Base.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.Base.General {
    public interface ISequenceObject {
        string TypeName { get; set; }
        long NextSequence { get; set; }
        IList<ISequenceReleasedObject> SequenceReleasedObjects { get; }
    }

    public interface ISupportSequenceObject  {
        long Sequence { get; set; }
        string Prefix { get; }
    }
    public interface ISequenceReleasedObject {
        ISequenceObject SequenceObject { get; set; }
        long Sequence { get; set; }
    }

    public class SequenceGenerator : IDisposable {
        public static bool UseGuidKey = true;
        static Type _sequenceObjectType;
        static IDataLayer _defaultDataLayer;
        static SequenceGenerator _sequenceGenerator;
        public const int MaxGenerationAttemptsCount = 10;
        public const int MinGenerationAttemptsDelay = 100;
        private readonly ExplicitUnitOfWork _explicitUnitOfWork;
        private ISequenceObject _sequence;

        static SequenceGenerator() {
            ThrowProviderSupportedException = true;
        }
        public SequenceGenerator() {
            int count = MaxGenerationAttemptsCount;
            while (true) {
                try {
                    _explicitUnitOfWork = new ExplicitUnitOfWork(_defaultDataLayer);
                    XPCollection sequences;
                    using (sequences = new XPCollection(_explicitUnitOfWork, _sequenceObjectType)){
                        foreach (XPBaseObject seq in sequences)
                            seq.Save();
                    }
                    _explicitUnitOfWork.FlushChanges();
                    break;
                }
                catch (LockingException) {
                    Close();
                    count--;
                    if (count <= 0)
                        throw;
                    Thread.Sleep(MinGenerationAttemptsDelay * count);
                }
            }
        }

        public static Type SequenceObjectType {
            get { return _sequenceObjectType; }
        }

        public void Accept() {
            _explicitUnitOfWork.CommitChanges();
        }

        public static void SetNextSequence(ITypeInfo typeInfo, long nextId) {
            SetNextSequence(typeInfo, null, nextId);
        }

        public static void SetNextSequence(ITypeInfo typeInfo, string prefix, long nextId) {
            if (_sequenceGenerator == null)
                _sequenceGenerator = new SequenceGenerator();
            var objectByKey = _sequenceGenerator._explicitUnitOfWork.FindObject(_sequenceObjectType, GetCriteria(typeInfo.FullName, prefix), true);
            _sequenceGenerator._sequence = objectByKey != null ? (ISequenceObject)objectByKey : CreateSequenceObject(prefix + typeInfo.FullName, _sequenceGenerator._explicitUnitOfWork);
            _sequenceGenerator._sequence.NextSequence = nextId;
            _sequenceGenerator._explicitUnitOfWork.FlushChanges();
            _sequenceGenerator.Accept();
        }

        public long GetNextSequence(ITypeInfo typeInfo, string prefix) {
            if (typeInfo == null)
                throw new ArgumentNullException("typeInfo");
            return GetNextSequence(XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary.GetClassInfo(typeInfo.Type), prefix);
        }

        public long GetNextSequence(ITypeInfo typeInfo) {
            return GetNextSequence(typeInfo, null);
        }

        long GetNextSequence(XPClassInfo classInfo, string preFix) {
            if (classInfo == null)
                throw new ArgumentNullException("classInfo");
            _sequence = GetNowSequence(classInfo, preFix, _explicitUnitOfWork);
            long nextId = _sequence.NextSequence;
            _sequence.NextSequence++;
            _explicitUnitOfWork.FlushChanges();
            return nextId;
        }

        private static ISequenceObject GetNowSequence(XPClassInfo classInfo, string preFix, UnitOfWork unitOfWork) {
            var obj = unitOfWork.FindObject(_sequenceObjectType, GetCriteria(classInfo.FullName, preFix));
            return obj != null ? (ISequenceObject)obj : CreateSequenceObject(preFix + classInfo.FullName, unitOfWork);
        }

        private static CriteriaOperator GetCriteria(string typeFullName, string preFix) {
            return CriteriaOperator.Parse("TypeName=?", preFix + typeFullName);
        }

        public static long GetNowSequence(XPClassInfo classInfo, string prefix) {
            long nextSequence;
            using (var unitOfWork = new UnitOfWork(_defaultDataLayer)) {
                nextSequence = GetNowSequence(classInfo, prefix, unitOfWork).NextSequence;
            }
            return nextSequence;
        }

        public static long GetNowSequence(ITypeInfo typeInfo) {
            var xpClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(typeInfo.Type);
            return GetNowSequence(xpClassInfo, null);
        }

        public static long GetNowSequence(XPClassInfo xpClassInfo) {
            return GetNowSequence(xpClassInfo, null);
        }

        public long GetNextSequence(XPClassInfo classInfo) {
            return GetNextSequence(classInfo, null);
        }

        public void Close() {
            if (_explicitUnitOfWork != null)
                _explicitUnitOfWork.Dispose();
        }

        public void Dispose() {
            Close();
        }

        public static void RegisterSequences(IEnumerable<ITypeInfo> persistentTypes) {
            if (persistentTypes != null)
                using (var unitOfWork = new UnitOfWork(_defaultDataLayer)) {
                    Dictionary<string, bool> typeToExistsMap = GetTypeToExistsMap(unitOfWork);
                    foreach (ITypeInfo typeInfo in persistentTypes) {
                        using (var uow = new UnitOfWork(_defaultDataLayer)) {
                            if (typeToExistsMap.ContainsKey(typeInfo.FullName)) continue;
                            CreateSequenceObject(typeInfo.FullName, unitOfWork);
                            try {
                                uow.CommitChanges();
                            }
                            catch (ConstraintViolationException) {
                            }
                        }
                    }
                }
        }

        public static void RegisterSequences(IEnumerable<XPClassInfo> persistentClasses) {
            if (persistentClasses != null)
                using (var unitOfWork = new UnitOfWork(_defaultDataLayer)) {
                    Dictionary<string, bool> typeToExistsMap = GetTypeToExistsMap(unitOfWork);
                    foreach (XPClassInfo classInfo in persistentClasses) {
                        using (var uow = new UnitOfWork(_defaultDataLayer)) {
                            if (typeToExistsMap.ContainsKey(classInfo.FullName)) continue;
                            CreateSequenceObject(classInfo.FullName, unitOfWork);
                            try {
                                uow.CommitChanges();
                            }
                            catch (ConstraintViolationException) {
                            }
                        }
                    }
                }

        }

        static Dictionary<string, bool> GetTypeToExistsMap(UnitOfWork unitOfWork) {
            Dictionary<string, bool> typeToExistsMap;
            XPCollection sequenceList;
            using (sequenceList = new XPCollection(unitOfWork, _sequenceObjectType)){
                typeToExistsMap = new Dictionary<string, bool>();
                foreach (ISequenceObject seq in sequenceList) {
                    typeToExistsMap[seq.TypeName] = true;
                }
            }
            return typeToExistsMap;
        }

        public static ISequenceObject CreateSequenceObject(string fullName, UnitOfWork unitOfWork) {
            var sequenceObject = (ISequenceObject)_sequenceObjectType.CreateInstance(unitOfWork);
            sequenceObject.TypeName = fullName;
            sequenceObject.NextSequence = 0;
            return sequenceObject;
        }

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject, ITypeInfo typeInfo) {
            if (_defaultDataLayer == null || !((XPBaseObject) supportSequenceObject).Session.IsNewObject(supportSequenceObject))
                return;
            if (!IsProviderSupported(supportSequenceObject)) {
                if (ThrowProviderSupportedException)
                    throw new NotSupportedException("Current provider does not support isolated transactions");
                return;
            }
            if (_sequenceGenerator == null)
                _sequenceGenerator = new SequenceGenerator();
            long nextSequence = _sequenceGenerator.GetNextSequence(typeInfo, supportSequenceObject.Prefix);
            Session session = ((XPBaseObject
                )supportSequenceObject).Session;
            if (IsNotNestedUnitOfWork(session)) {
                SessionManipulationEventHandler[] sessionOnAfterCommitTransaction = { null };
                sessionOnAfterCommitTransaction[0] = (sender, args) => {
                    if (_sequenceGenerator != null) {
                        try {
                            _sequenceGenerator.Accept();
                        }
                        finally {
                            session.AfterCommitTransaction -= sessionOnAfterCommitTransaction[0];
                        }
                    }

                };
                session.AfterCommitTransaction += sessionOnAfterCommitTransaction[0];
            }
            supportSequenceObject.Sequence = nextSequence;
        }

        public static bool ThrowProviderSupportedException { get; set; }

        private static bool IsProviderSupported(ISupportSequenceObject supportSequenceObject) {
            return !(((XPBaseObject)supportSequenceObject).Session.DataLayer.ConnectionProvider(supportSequenceObject) is SQLiteConnectionProvider);
        }

        static bool IsNotNestedUnitOfWork(Session session) {
            return !(session is NestedUnitOfWork);
        }

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject){
            var info = XafTypesInfo.Instance.FindTypeInfo(supportSequenceObject.GetType());
            var typeInfo = info.IsInterface
                ? XafTypesInfo.Instance.FindTypeInfo(XpoTypesInfoHelper.GetXpoTypeInfoSource().GetGeneratedEntityType(info.Type))
                : XafTypesInfo.Instance.FindTypeInfo(((XPBaseObject) supportSequenceObject).ClassInfo.FullName);
            GenerateSequence(supportSequenceObject, typeInfo);
        }

        public static void Initialize(IDataLayer dataLayer, Type sequenceObjectType) {
            Guard.ArgumentNotNull(dataLayer,"datalayer");
            Guard.ArgumentNotNull(sequenceObjectType, "sequenceObjectType");
            _sequenceGenerator = null;
            _sequenceObjectType = sequenceObjectType;
            _defaultDataLayer = dataLayer;
            RegisterSequences(ApplicationHelper.Instance.Application.TypesInfo.PersistentTypes);
        }

        public static void Initialize(string connectionString, Type sequenceObjectType){
            Initialize(XpoDefault.GetDataLayer(connectionString, AutoCreateOption.None), sequenceObjectType);
        }

        public static void ReleaseSequence(ISupportSequenceObject supportSequenceObject) {
            if (_defaultDataLayer == null)
                return;
            var objectSpace = (XPObjectSpace)XPObjectSpace.FindObjectSpaceByObject(supportSequenceObject);
            var sequenceObject = objectSpace.GetObjectByKey(_sequenceObjectType, supportSequenceObject.Prefix +
                                                                                 ((XPBaseObject)supportSequenceObject).ClassInfo.FullName) as ISequenceObject;
            if (sequenceObject != null) {
                var objectFromInterface = objectSpace.CreateObjectFromInterface<ISequenceReleasedObject>();
                objectFromInterface.Sequence = supportSequenceObject.Sequence;
                objectFromInterface.SequenceObject = sequenceObject;
            }
        }
    }
    public interface ISequenceGeneratorUser {
        event CancelEventHandler InitSeqGenerator;
    }

    public class SequenceGeneratorUpdater : ModuleUpdater {
        public SequenceGeneratorUpdater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
            if (SequenceGenerator.UseGuidKey) {
                var classInfo = XpandModuleBase.Dictiorary.GetClassInfo(XpandModuleBase.SequenceObjectType);
                var dbTable = GetDbTable(classInfo.TableName);
                var typeNamePropertyName = GetSequenceObject().GetPropertyName(o => o.TypeName);
                if (dbTable != null && dbTable.PrimaryKey.Columns.Contains(typeNamePropertyName)) {
                    if (SequenceGeneratorHelper.IsMySql())
                        throw new NotImplementedException("Set SequenceGenerator.UseGuidKey=false or update the set Oid as key property manually");
                    var memberInfo = classInfo.Members.FirstOrDefault(info => info.IsCollection && typeof(ISequenceReleasedObject).IsAssignableFrom(info.CollectionElementType.ClassType));
                    if (memberInfo != null) {
                        var tableName = memberInfo.CollectionElementType.Table.Name;
                        if (GetDbTable(tableName) != null)
                            ExecuteNonQueryCommand("drop table " + tableName, false);
                    }
                    ExecuteNonQueryCommand(String.Format(
                        CultureInfo.InvariantCulture, "alter table {0} drop constraint PK_{0}", classInfo.TableName), false);
                }
            }
        }

        private DBTable GetDbTable(string name) {
            var xpObjectSpace = ObjectSpace as XPObjectSpace;
            var dataStoreSchemaExplorer = GetDataStoreSchemaExplorer(xpObjectSpace);
            return dataStoreSchemaExplorer.GetStorageTablesList(false).Where(s => s.ToLowerInvariant() == name.ToLowerInvariant()).Select(s => {
                var dbTable = new DBTable(s);
                ((ConnectionProviderSql)dataStoreSchemaExplorer).GetTableSchema(dbTable, true, true);
                return dbTable;
            }).FirstOrDefault(table => table.PrimaryKey != null);
        }

        private ISequenceObject GetSequenceObject() {
            return null;
        }

        private IDataStoreSchemaExplorer GetDataStoreSchemaExplorer(XPObjectSpace xpObjectSpace) {
            var connectionProvider = ((BaseDataLayer)xpObjectSpace.Session.DataLayer).ConnectionProvider;
            var multiDataStoreProxy = connectionProvider as MultiDataStoreProxy;
            if (multiDataStoreProxy != null)
                return (IDataStoreSchemaExplorer)multiDataStoreProxy.DataStore;
            return connectionProvider as IDataStoreSchemaExplorer;
        }
    }

    class SequenceGeneratorHelper {
        private const string SequenceGeneratorHelperName = "SequenceGeneratorHelper";
        XpandModuleBase _xpandModuleBase;

        public XafApplication Application {
            get { return _xpandModuleBase.Application; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static Type SequenceObjectType {
            get { return XpandModuleBase.SequenceObjectType; }
            set { XpandModuleBase.SequenceObjectType = value; }
        }

        public void InitializeSequenceGenerator() {
            if (_xpandModuleBase == null)
                return;
            try {
                var cancelEventArgs = new CancelEventArgs();
                _xpandModuleBase.OnInitSeqGenerator(cancelEventArgs);
                if (cancelEventArgs.Cancel)
                    return;
                if (!typeof(ISequenceObject).IsAssignableFrom(SequenceObjectType))
                    throw new TypeLoadException("Please make sure XPand.Persistent.BaseImpl is referenced from your application project and has its Copy Local==true");
                if (Application != null ) {
                    var xpObjectSpaceProvider = Application.ObjectSpaceProviders.OfType<XPObjectSpaceProvider>().FirstOrDefault();
                    if (xpObjectSpaceProvider!=null)
                        SequenceGenerator.Initialize(XpandModuleBase.ConnectionString, SequenceObjectType);
                }
            }
            catch (Exception e) {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        public static void ModifySequenceObjectWhenMySqlDatalayer(ITypesInfo typesInfo) {
            if (SequenceObjectType != null) {
                var typeInfo = typesInfo.FindTypeInfo(SequenceObjectType);
                if (IsMySql(typeInfo)) {
                    var memberInfo = (XafMemberInfo)typeInfo.FindMember<ISequenceObject>(o => o.TypeName);
                    memberInfo.RemoveAttributes<SizeAttribute>();
                    memberInfo.AddAttribute(new SizeAttribute(255));
                }
            }
        }

        internal static bool IsMySql() {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(SequenceObjectType);
            return IsMySql(typeInfo);
        }

        private static bool IsMySql(ITypeInfo typeInfo) {
            var sequenceObjectObjectSpaceProvider = GetSequenceObjectObjectSpaceProvider(typeInfo.Type);
            if (sequenceObjectObjectSpaceProvider != null) {
                var helper = new ConnectionStringParser(sequenceObjectObjectSpaceProvider.ConnectionString);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                return providerType == MySqlConnectionProvider.XpoProviderTypeString;
            }
            return false;
        }

        static IObjectSpaceProvider GetSequenceObjectObjectSpaceProvider(Type type) {
            return (ApplicationHelper.Instance.Application.ObjectSpaceProviders.Select(objectSpaceProvider
                => new { objectSpaceProvider, originalObjectType = objectSpaceProvider.EntityStore.GetOriginalType(type) })
                .Where(@t => (@t.originalObjectType != null) && @t.objectSpaceProvider.EntityStore.RegisteredEntities.Contains(@t.originalObjectType))
                .Select(@t => @t.objectSpaceProvider)).FirstOrDefault();
        }

        public void Attach(XpandModuleBase xpandModuleBase) {
            if (!xpandModuleBase.Executed<ISequenceGeneratorUser>(SequenceGeneratorHelperName)) {
                if (SequenceObjectType == null) {
                    SequenceObjectType = xpandModuleBase.LoadFromBaseImpl("Xpand.Persistent.BaseImpl.SequenceObject");
                }
                if (xpandModuleBase.RuntimeMode) {
                    _xpandModuleBase = xpandModuleBase;
                    Application.LoggedOff += ApplicationOnLoggedOff;
                    var helper = new ConnectionStringHelper();
                    helper.Attach(_xpandModuleBase);
                    helper.ConnectionStringUpdated += (sender, args) => InitializeSequenceGenerator();
                }
            }
        }

        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            ((XafApplication)sender).LoggedOff -= ApplicationOnLoggedOff;
            XpandModuleBase.CallMonitor.Remove(new KeyValuePair<string, ApplicationModulesManager>(SequenceGeneratorHelperName, _xpandModuleBase.ModuleManager));
        }
    }
}