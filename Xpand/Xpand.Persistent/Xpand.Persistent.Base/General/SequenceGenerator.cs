using System;
using System.Collections.Generic;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using ITypeInfo = DevExpress.ExpressApp.DC.ITypeInfo;

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

        public const int MaxGenerationAttemptsCount = 10;
        public const int MinGenerationAttemptsDelay = 100;
        private readonly ExplicitUnitOfWork _explicitUnitOfWork;
        private ISequenceObject _sequence;


        public SequenceGenerator() {
            int count = MaxGenerationAttemptsCount;
            while (true) {
                try {
                    _explicitUnitOfWork = new ExplicitUnitOfWork(DefaultDataLayer);
                    var sequences = new XPCollection(_explicitUnitOfWork, _sequenceObjectType);
                    foreach (XPBaseObject seq in sequences)
                        seq.Save();
                    _explicitUnitOfWork.FlushChanges();
                    break;
                } catch (LockingException) {
                    Close();
                    count--;
                    if (count <= 0)
                        throw;
                    Thread.Sleep(MinGenerationAttemptsDelay * count);
                }
            }
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
            return GetNextSequence(XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeInfo.Type), prefix);
        }

        public long GetNextSequence(ITypeInfo typeInfo) {
            return GetNextSequence(typeInfo, null);
        }

        long GetNextSequence(XPClassInfo classInfo, string preFix) {
            if (classInfo == null)
                throw new ArgumentNullException("classInfo");
            var objectByKey = _explicitUnitOfWork.GetObjectByKey(_sequenceObjectType, preFix + classInfo.FullName, true);
            _sequence = objectByKey != null ? (ISequenceObject)objectByKey : CreateSequenceObject(preFix + classInfo.FullName, _explicitUnitOfWork);
            long nextId = _sequence.NextSequence;
            _sequence.NextSequence++;
            _explicitUnitOfWork.FlushChanges();
            return nextId;
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

        static IDataLayer _defaultDataLayer;
        static Type _sequenceObjectType;

        public static IDataLayer DefaultDataLayer {
            get {
                if (_defaultDataLayer == null)
                    throw new NullReferenceException("DefaultDataLayer");
                return _defaultDataLayer;
            }
            set { _defaultDataLayer = value; }
        }

        public static void RegisterSequences(IEnumerable<ITypeInfo> persistentTypes) {
            if (persistentTypes != null)
                using (var unitOfWork = new UnitOfWork(DefaultDataLayer)) {
                    Dictionary<string, bool> typeToExistsMap = GetTypeToExistsMap(unitOfWork);
                    foreach (ITypeInfo typeInfo in persistentTypes) {
                        using (var uow = new UnitOfWork(DefaultDataLayer)) {
                            if (typeToExistsMap.ContainsKey(typeInfo.FullName)) continue;
                            CreateSequenceObject(typeInfo.FullName, unitOfWork);
                            try {
                                uow.CommitChanges();
                            } catch (ConstraintViolationException) {
                            }
                        }
                    }
                }
        }

        public static void RegisterSequences(IEnumerable<XPClassInfo> persistentClasses) {
            if (persistentClasses != null)
                using (var unitOfWork = new UnitOfWork(DefaultDataLayer)) {
                    Dictionary<string, bool> typeToExistsMap = GetTypeToExistsMap(unitOfWork);
                    foreach (XPClassInfo classInfo in persistentClasses) {
                        using (var uow = new UnitOfWork(DefaultDataLayer)) {
                            if (typeToExistsMap.ContainsKey(classInfo.FullName)) continue;
                            CreateSequenceObject(classInfo.FullName, unitOfWork);
                            try {
                                uow.CommitChanges();
                            } catch (ConstraintViolationException) {
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
            var sequenceObject = (ISequenceObject)Activator.CreateInstance(_sequenceObjectType, new object[] { unitOfWork });
            sequenceObject.TypeName = fullName;
            sequenceObject.NextSequence = 0;
            return sequenceObject;
        }

        static SequenceGenerator _sequenceGenerator;

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject, ITypeInfo typeInfo) {
            if (_defaultDataLayer == null)
                return;
            if (_sequenceGenerator == null)
                _sequenceGenerator = new SequenceGenerator();
            long nextSequence = _sequenceGenerator.GetNextSequence(typeInfo, supportSequenceObject.Prefix);
            Session session = supportSequenceObject.Session;
            if (!(session is NestedUnitOfWork)) {
                SessionManipulationEventHandler[] sessionOnAfterCommitTransaction = { null };
                sessionOnAfterCommitTransaction[0] = (sender, args) => {
                    if (_sequenceGenerator != null) {
                        try {
                            _sequenceGenerator.Accept();
                        } finally {
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

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject) {
            GenerateSequence(supportSequenceObject, XafTypesInfo.Instance.FindTypeInfo(supportSequenceObject.ClassInfo.FullName));
        }



        public static void Initialize(string connectionString, Type sequenceObjectType) {
            _sequenceObjectType = sequenceObjectType;
            DefaultDataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
            RegisterSequences(XafTypesInfo.Instance.PersistentTypes);
        }

        public static void ReleaseSequence(ISupportSequenceObject supportSequenceObject) {
            if (_defaultDataLayer == null)
                return;
            var objectSpace = (ObjectSpace)ObjectSpace.FindObjectSpaceByObject(supportSequenceObject);
            var sequenceObject = objectSpace.GetObjectByKey(_sequenceObjectType, supportSequenceObject.Prefix + supportSequenceObject.ClassInfo.FullName) as ISequenceObject;
            if (sequenceObject != null) {
                var objectFromInterface = objectSpace.CreateObjectFromInterface<ISequenceReleasedObject>();
                objectFromInterface.Sequence = supportSequenceObject.Sequence;
                objectFromInterface.SequenceObject = sequenceObject;
            }
        }
    }
}
