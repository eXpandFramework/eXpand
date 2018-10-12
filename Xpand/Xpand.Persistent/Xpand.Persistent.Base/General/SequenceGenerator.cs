using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Security.ClientServer;
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
using Xpand.Persistent.Base.Security;
using Xpand.Persistent.Base.Xpo;
using Xpand.Xpo.ConnectionProviders;
using MSSqlConnectionProvider = DevExpress.Xpo.DB.MSSqlConnectionProvider;
using MySqlConnectionProvider = DevExpress.Xpo.DB.MySqlConnectionProvider;
using OracleConnectionProvider = DevExpress.Xpo.DB.OracleConnectionProvider;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property,AllowMultiple = true)]
    public class SequenceGeneratorAttribute:Attribute {
        public SequenceGeneratorAttribute(string sequenceName,bool releaseDeleted){
            ReleaseDeleted = releaseDeleted;
            SequenceName = sequenceName;
        }

        public bool ReleaseDeleted{ get; }

        public string SequenceName{ get; set; }
    }

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
        private static readonly object Locker=new object();

        private SequenceGenerator(){
            _explicitUnitOfWork = new ExplicitUnitOfWork(_defaultDataLayer);
        }

        public void Lock(){
            int count = MaxGenerationAttemptsCount;
            while (true) {
                try {
                    var sequences = new XPCollection(PersistentCriteriaEvaluationBehavior.BeforeTransaction,_explicitUnitOfWork, _sequenceObjectType, null).Cast<ISessionProvider>();
                    foreach (var seq in sequences) {
                        seq.Session.Save(seq);
                    }
                    _explicitUnitOfWork.FlushChanges();
                    break;
                }
                catch (LockingException) {
                    Close();
                    count--;
                    if (count <= 0) {
                        throw;
                    }
                    Thread.Sleep(MinGenerationAttemptsDelay * count);
                }
            }

        }

        static SequenceGenerator() {
            ThrowProviderSupportedException = true;
        }

        public static Type SequenceObjectType => _sequenceObjectType;

        public void Accept() {
            _explicitUnitOfWork.CommitChanges();
        }

        public static void SetNextSequence(ITypeInfo typeInfo, long nextId) {
            SetNextSequence(typeInfo, null, nextId);
        }

        public static void SetNextSequence(ITypeInfo typeInfo, string prefix, long nextId) {
            var objectByKey = _sequenceGenerator._explicitUnitOfWork.FindObject(_sequenceObjectType, GetCriteria(prefix+typeInfo.FullName), true);
            _sequenceGenerator._sequence = objectByKey != null ? (ISequenceObject)objectByKey : CreateSequenceObject(prefix + typeInfo.FullName, _sequenceGenerator._explicitUnitOfWork);
            _sequenceGenerator._sequence.NextSequence = nextId;
            _sequenceGenerator._explicitUnitOfWork.FlushChanges();
            _sequenceGenerator.Accept();
        }


        long GetNextSequence(string sequenceType) {
            if (sequenceType == null)
                throw new ArgumentNullException(nameof(sequenceType));
            lock (Locker){
                Lock();
                _sequence = GetNowSequence(sequenceType,  _explicitUnitOfWork);
                long nextId = _sequence.NextSequence;
                var sequenceReleasedObject = _sequence.SequenceReleasedObjects.Where(o => o!=null).ToArray().OrderBy(o => o.Sequence)
                    .FirstOrDefault();
                if (sequenceReleasedObject != null){
                    nextId = sequenceReleasedObject.Sequence;
                    _explicitUnitOfWork.Delete(sequenceReleasedObject);
                }
                else
                    _sequence.NextSequence++;
                _explicitUnitOfWork.FlushChanges();
                return nextId;
            }
        }

        private static ISequenceObject GetNowSequence(string sequenceType,  UnitOfWork unitOfWork) {
            var obj = unitOfWork.FindObject(_sequenceObjectType, GetCriteria(sequenceType));
            return obj != null ? GetSequenceObject(obj,unitOfWork) : CreateSequenceObject( sequenceType, unitOfWork);
        }

        private static ISequenceObject GetSequenceObject(object obj, UnitOfWork unitOfWork){
            var sequenceObject = (ISequenceObject)obj;
            unitOfWork.Reload(sequenceObject,true);
            return sequenceObject;
        }

        private static CriteriaOperator GetCriteria(string typeFullName) {
            return CriteriaOperator.Parse("TypeName=?",  typeFullName);
        }

        public static long GetNowSequence(XPClassInfo classInfo, string prefix) {
            long nextSequence;
            using (var unitOfWork = new UnitOfWork(_defaultDataLayer)) {
                nextSequence = GetNowSequence(prefix+classInfo.FullName, unitOfWork).NextSequence;
            }
            return nextSequence;
        }

        public static long GetNowSequence(ITypeInfo typeInfo) {
            var xpClassInfo = typeInfo.QueryXPClassInfo();
            return GetNowSequence(xpClassInfo, null);
        }

        public static long GetNowSequence(XPClassInfo xpClassInfo) {
            return GetNowSequence(xpClassInfo, null);
        }

        public long GetNextSequence(XPClassInfo classInfo) {
            return GetNextSequence(classInfo.FullName);
        }

        public void Close(){
            _explicitUnitOfWork?.Dispose();
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
                            uow.CommitChanges();
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

        public static long GenerateSequence( string sequenceName){
            using (var objectSpace = ApplicationHelper.Instance.Application.CreateObjectSpace()){
                long seq = 0;
                GenerateSequence(objectSpace.Session(), sequenceName,l => seq=l);
                return seq;
            }
        }

        public static void GenerateSequence(ISessionProvider sessionProvider, string sequenceType,Action<long> seq){
            if (sessionProvider.Session.IsNewObject(sessionProvider))
                GenerateSequence(sessionProvider.Session, sequenceType, seq);
        }

        public static void GenerateSequence(Session session, string sequenceType,Action<long> seq){
            if (CanGenerate(session) ) {
                long nextSequence = _sequenceGenerator.GetNextSequence(sequenceType);
                SessionManipulationEventHandler[] sessionOnAfterCommitTransaction = { null };
                sessionOnAfterCommitTransaction[0] = (sender, args) => {
                    try {
                        _sequenceGenerator.Accept();
                    }
                    finally {
                        session.AfterCommitTransaction -= sessionOnAfterCommitTransaction[0];
                    }
                };
                session.AfterCommitTransaction += sessionOnAfterCommitTransaction[0];
                seq( nextSequence);
            }
        }

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject, ITypeInfo typeInfo) {
            if (((ISessionProvider)supportSequenceObject).Session.IsNewObject(supportSequenceObject))
                GenerateSequence(((ISessionProvider) supportSequenceObject).Session,supportSequenceObject.Prefix+typeInfo.FullName,l => supportSequenceObject.Sequence=l);
        }

        private static bool CanGenerate(Session session){
            if (ApplicationHelper.Instance.Application != null && ApplicationHelper.Instance.Application.Security.IsRemoteClient())
                return false;
            if (_defaultDataLayer == null || 
                (session.ObjectLayer is SecuredSessionObjectLayer) || session is NestedUnitOfWork)
                return false;
            if (!IsProviderSupported(session)) {
                if (ThrowProviderSupportedException)
                    throw new NotSupportedException("Current provider does not support isolated transactions");
                return false;
            }
            return true;
        }

        public static bool ThrowProviderSupportedException { get; set; }

        private static bool IsProviderSupported(Session session) {
            return !(session.DataLayer.ConnectionProvider(session) is SQLiteConnectionProvider);
        }

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject){
            var info = XafTypesInfo.Instance.FindTypeInfo(supportSequenceObject.GetType());
            var typeInfo = info.IsInterface
                ? XafTypesInfo.Instance.FindTypeInfo(XpoTypesInfoHelper.GetXpoTypeInfoSource().GetGeneratedEntityType(info.Type))
                : XafTypesInfo.Instance.FindTypeInfo(((XPBaseObject) supportSequenceObject).ClassInfo.FullName);
            GenerateSequence(supportSequenceObject, typeInfo);
        }

        public static Dictionary<Type, Type> SupportedFactories { get; } = new Dictionary<Type, Type> {{
                typeof(MSSqlConnectionProvider),typeof(MSSqlProviderFactory)}, {
                typeof(MySqlConnectionProvider),typeof(MySqlProviderFactory)}, {
                typeof(OracleConnectionProvider),typeof(OracleProviderFactory)
            }
        };

        public static void Initialize(IDataLayer dataLayer, Type sequenceObjectType) {
            Guard.ArgumentNotNull(dataLayer,"datalayer");
            Guard.ArgumentNotNull(sequenceObjectType, "sequenceObjectType");
            _sequenceGenerator = null;
            _sequenceObjectType = sequenceObjectType;
            _defaultDataLayer = dataLayer;
            RegisterSequences(ApplicationHelper.Instance.Application.TypesInfo.PersistentTypes);
            _sequenceGenerator = new SequenceGenerator();
        }


        public static void Initialize(string connectionString, Type sequenceObjectType) {
            if (IsFactorySupported(connectionString)) {
                var dataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.None);
                Initialize(dataLayer, sequenceObjectType);
            }
        }

        public static bool IsFactorySupported(Type connectionProviderType) {
            return SupportedFactories.Any(pair => pair.Key.IsAssignableFrom(connectionProviderType));
        }

        public static bool IsFactorySupported(string connectionString){
            var factory = GetProviderFactory(connectionString);
            return factory == null || SupportedFactories.Any(pair => pair.Value.IsInstanceOfType(factory));
        }

        private static ProviderFactory GetProviderFactory(string connectionString){
            var connectionStringParser = new ConnectionStringParser(connectionString);
            var xpoProvider = connectionStringParser.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            var factory = DataStoreBase.Factories.FirstOrDefault(_ => _.ProviderKey == xpoProvider);
            return factory;
        }


        public static void ReleaseSequence(string sequenceName,long sequence){
            using (var objectSpace = ApplicationHelper.Instance.Application.CreateObjectSpace()){
                ReleaseSequence(objectSpace.Session(), sequenceName, sequence);
                objectSpace.CommitChanges();
            }
        }

        public static void ReleaseSequence(ISupportSequenceObject supportSequenceObject) {
            var typeName = supportSequenceObject.Prefix + supportSequenceObject.GetType().FullName;
            var objectSpace = (XPObjectSpace)XPObjectSpace.FindObjectSpaceByObject(supportSequenceObject);
            ReleaseSequence(objectSpace.Session, typeName, supportSequenceObject.Sequence);
        }

        public static void ReleaseSequence(Session session, string typeName, long sequence){
            var sequenceObject = session.QueryObject<ISequenceObject>(seq => seq.TypeName == typeName, _sequenceObjectType);
            if (sequenceObject != null){
                var releasedObject = session.Create<ISequenceReleasedObject>();
                releasedObject.Sequence = sequence;
                releasedObject.SequenceObject = sequenceObject;
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
                var classInfo = XafTypesInfo.CastTypeToTypeInfo(XpandModuleBase.SequenceObjectType).QueryXPClassInfo();
                var dbTable = GetDbTable(classInfo.TableName);
                var typeNamePropertyName = nameof(ISequenceObject.TypeName);
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
            return dataStoreSchemaExplorer?.GetStorageTables().FirstOrDefault(table => table.Name.ToLower()==name.ToLower()&& table.PrimaryKey != null);
        }

        private IDataStoreSchemaExplorer GetDataStoreSchemaExplorer(XPObjectSpace xpObjectSpace) {
            var connectionProvider = ((BaseDataLayer)xpObjectSpace.Session.DataLayer).ConnectionProvider;
            if (connectionProvider is MultiDataStoreProxy multiDataStoreProxy)
                return (IDataStoreSchemaExplorer)multiDataStoreProxy.DataStore;
            return connectionProvider as IDataStoreSchemaExplorer;
        }
    }

    class SequenceGeneratorHelper {
        private const string SequenceGeneratorHelperName = "SequenceGeneratorHelper";
        XpandModuleBase _xpandModuleBase;

        public XafApplication Application => _xpandModuleBase.Application;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static Type SequenceObjectType {
            get => XpandModuleBase.SequenceObjectType;
            set => XpandModuleBase.SequenceObjectType = value;
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
                var xpObjectSpaceProvider = Application?.ObjectSpaceProviders.OfType<XPObjectSpaceProvider>().FirstOrDefault();
                if (xpObjectSpaceProvider != null)
                    SequenceGenerator.Initialize(XpandModuleBase.ConnectionString, SequenceObjectType);
            }
            catch (Exception e) {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        internal static void ModifySequenceObjectWhenMySqlDatalayer(ITypesInfo typesInfo) {
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

        private static bool IsMySql(ITypeInfo typeInfo){
            var objectSpaceProvider = ApplicationHelper.Instance.Application.ObjectSpaceProviders.FindProvider(typeInfo.Type);
            return objectSpaceProvider != null && objectSpaceProvider.GetProviderType() == ConnectionProviderType.MySQL;
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
                    helper.ConnectionStringUpdated += (sender, args) => {
                        if (((IModelOptionsSequenceGenerator) Application.Model.Options).EnableSequenceGenerator)
                            InitializeSequenceGenerator();
                    };
                }
            }
        }

        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            ((XafApplication)sender).LoggedOff -= ApplicationOnLoggedOff;
            XpandModuleBase.CallMonitor.Remove(new KeyValuePair<string, ApplicationModulesManager>(SequenceGeneratorHelperName, _xpandModuleBase.ModuleManager));
        }
    }
}