using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using ITypeInfo = DevExpress.ExpressApp.DC.ITypeInfo;
using Fasterflect;

namespace Xpand.Persistent.Base.General {
    public interface ISequenceObject {
        string TypeName { get; set; }
        long NextSequence { get; set; }
        IList<ISequenceReleasedObject> SequenceReleasedObjects { get; }
    }

    public interface ISupportSequenceObject : IXPClassInfoProvider, ISessionProvider {
        long Sequence { get; set; }
        string Prefix { get; }
    }
    public interface ISequenceReleasedObject {
        ISequenceObject SequenceObject { get; set; }
        long Sequence { get; set; }
    }

    public class SequenceGenerator : IDisposable {
        static Type _sequenceObjectType;
        static IDataLayer _defaultDataLayer;
        static SequenceGenerator _sequenceGenerator;
        public const int MaxGenerationAttemptsCount = 10;
        public const int MinGenerationAttemptsDelay = 100;
        private readonly ExplicitUnitOfWork _explicitUnitOfWork;
        private ISequenceObject _sequence;


        public SequenceGenerator() {
            int count = MaxGenerationAttemptsCount;
            while (true) {
                try {
                    _explicitUnitOfWork = new ExplicitUnitOfWork(_defaultDataLayer);
                    var sequences = new XPCollection(_explicitUnitOfWork, _sequenceObjectType);
                    foreach (XPBaseObject seq in sequences)
                        seq.Save();
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
            var objectByKey = _sequenceGenerator._explicitUnitOfWork.GetObjectByKey(_sequenceObjectType, prefix + typeInfo.FullName, true);
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
            _sequence=GetNowSequence(classInfo, preFix,_explicitUnitOfWork);
            long nextId = _sequence.NextSequence;
            _sequence.NextSequence++;
            _explicitUnitOfWork.FlushChanges();
            return nextId;
        }

        private static ISequenceObject GetNowSequence(XPClassInfo classInfo, string preFix,UnitOfWork unitOfWork){
            var objectByKey = unitOfWork.GetObjectByKey(_sequenceObjectType, preFix + classInfo.FullName, true);
            return objectByKey != null
                ? (ISequenceObject) objectByKey
                : CreateSequenceObject(preFix + classInfo.FullName, unitOfWork);
        }

        public static long GetNowSequence(XPClassInfo classInfo, string prefix){
            long nextSequence;
            using (var unitOfWork = new UnitOfWork(_defaultDataLayer)){
                nextSequence = GetNowSequence(classInfo, prefix, unitOfWork).NextSequence;
            }
            return nextSequence;
        }

        public static long GetNowSequence(ITypeInfo typeInfo){
            var xpClassInfo = XpandModuleBase.Dictiorary.GetClassInfo(typeInfo.Type);
            return GetNowSequence(xpClassInfo,null);
        }

        public static long GetNowSequence(XPClassInfo xpClassInfo) {
            return GetNowSequence(xpClassInfo,null);
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
            var sequenceList = new XPCollection(unitOfWork, _sequenceObjectType);
            var typeToExistsMap = new Dictionary<string, bool>();
            foreach (ISequenceObject seq in sequenceList) {
                typeToExistsMap[seq.TypeName] = true;
            }
            return typeToExistsMap;
        }

        public static ISequenceObject CreateSequenceObject(string fullName, UnitOfWork unitOfWork) {
            var sequenceObject = (ISequenceObject)_sequenceObjectType.CreateInstance(new object[] { unitOfWork });
            sequenceObject.TypeName = fullName;
            sequenceObject.NextSequence = 0;
            return sequenceObject;
        }

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject, ITypeInfo typeInfo) {
            if (_defaultDataLayer == null)
                return;
            if (supportSequenceObject.Session is NestedUnitOfWork ||
                !supportSequenceObject.Session.IsNewObject(supportSequenceObject))
                return;
            if (_sequenceGenerator == null)
                _sequenceGenerator = new SequenceGenerator();
            long nextSequence = _sequenceGenerator.GetNextSequence(typeInfo, supportSequenceObject.Prefix);
            Session session = supportSequenceObject.Session;
            if (IsNotNestedUnitOfWork(session)) {
                SessionManipulationEventHandler[] sessionOnAfterCommitTransaction = { null };
                sessionOnAfterCommitTransaction[0] = (sender, args) => {
                    if (_sequenceGenerator != null) {
                        try {
                            _sequenceGenerator.Accept();
                        }
                        finally {
                            session.AfterCommitTransaction -= sessionOnAfterCommitTransaction[0];
                            _sequenceGenerator.Dispose();
                            _sequenceGenerator = null;
                        }
                    }

                };
                session.AfterCommitTransaction += sessionOnAfterCommitTransaction[0];
            }
            supportSequenceObject.Sequence = nextSequence;
        }

        static bool IsNotNestedUnitOfWork(Session session) {
            return !(session is NestedUnitOfWork);
        }

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject) {
            GenerateSequence(supportSequenceObject, XafTypesInfo.Instance.FindTypeInfo(supportSequenceObject.ClassInfo.FullName));
        }

        public static void Initialize(string connectionString, Type sequenceObjectType) {
            _sequenceGenerator = null;
            _sequenceObjectType = sequenceObjectType;
            _defaultDataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.None);
            RegisterSequences(ApplicationHelper.Instance.Application.TypesInfo.PersistentTypes);
        }

        public static void ReleaseSequence(ISupportSequenceObject supportSequenceObject) {
            if (_defaultDataLayer == null)
                return;
            var objectSpace = (XPObjectSpace)XPObjectSpace.FindObjectSpaceByObject(supportSequenceObject);
            var sequenceObject = objectSpace.GetObjectByKey(_sequenceObjectType, supportSequenceObject.Prefix + supportSequenceObject.ClassInfo.FullName) as ISequenceObject;
            if (sequenceObject != null) {
                var objectFromInterface = objectSpace.CreateObjectFromInterface<ISequenceReleasedObject>();
                objectFromInterface.Sequence = supportSequenceObject.Sequence;
                objectFromInterface.SequenceObject = sequenceObject;
            }
        }
    }
    public interface ISequenceGeneratorUser {
    }

    class SequenceGeneratorHelper {
        private const string SequenceGeneratorHelperName = "SequenceGeneratorHelper";
        XpandModuleBase _xpandModuleBase;

        public XafApplication Application {
            get { return _xpandModuleBase.Application; }
        }

        void XpandModuleBaseOnConnectionStringUpdated(object sender, EventArgs eventArgs) {
            InitializeSequenceGenerator();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type SequenceObjectType {
            get { return _xpandModuleBase.SequenceObjectType; }
            set { _xpandModuleBase.SequenceObjectType = value; }
        }

        void InitializeSequenceGenerator() {

            try {
                var cancelEventArgs = new CancelEventArgs();
                _xpandModuleBase.OnInitSeqGenerator(cancelEventArgs);
                if (cancelEventArgs.Cancel)
                    return;
                if (!typeof(ISequenceObject).IsAssignableFrom(SequenceObjectType))
                    throw new TypeLoadException("Please make sure XPand.Persistent.BaseImpl is referenced from your application project and has its Copy Local==true");
                if (Application != null && Application.ObjectSpaceProvider != null && !(Application.ObjectSpaceProvider is DataServerObjectSpaceProvider)) {
                    SequenceGenerator.Initialize(XpandModuleBase.ConnectionString, SequenceObjectType);
                }
            }
            catch (Exception e) {
                if (e.InnerException != null)
                    throw e.InnerException;
                throw;
            }
        }

        void AddToAdditionalExportedTypes(string[] strings) {
            _xpandModuleBase.AddToAdditionalExportedTypes(strings);
            SequenceObjectType = _xpandModuleBase.AdditionalExportedTypes.Single(type => type.FullName == "Xpand.Persistent.BaseImpl.SequenceObject");
        }

        public void Attach(XpandModuleBase xpandModuleBase, ConnectionStringHelper helper) {
            _xpandModuleBase = xpandModuleBase;
            if (!_xpandModuleBase.Executed<ISequenceGeneratorUser>(SequenceGeneratorHelperName)) {
                if (_xpandModuleBase.RuntimeMode) {
                    Application.LoggedOff += ApplicationOnLoggedOff;
                    AddToAdditionalExportedTypes(new[] { "Xpand.Persistent.BaseImpl.SequenceObject" });
                    helper.ConnectionStringUpdated += XpandModuleBaseOnConnectionStringUpdated;
                }
            }
        }

        void ApplicationOnLoggedOff(object sender, EventArgs eventArgs) {
            ((XafApplication)sender).LoggedOff -= ApplicationOnLoggedOff;
            XpandModuleBase.CallMonitor.Remove(new KeyValuePair<string, ApplicationModulesManager>(SequenceGeneratorHelperName, _xpandModuleBase.ModuleManager));
        }
    }
}
