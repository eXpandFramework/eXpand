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

        static SequenceGenerator() {
            _sequenceObjectType = XafTypesInfo.Instance.FindBussinessObjectType<ISequenceObject>();
        }

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

        public long GetNextSequence(ITypeInfo typeInfo) {
            if (typeInfo == null)
                throw new ArgumentNullException("typeInfo");
            return GetNextSequence(XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeInfo.Type), null);
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

        static IDataLayer defaultDataLayer;
        static readonly Type _sequenceObjectType;

        public static IDataLayer DefaultDataLayer {
            get {
                if (defaultDataLayer == null)
                    throw new NullReferenceException("DefaultDataLayer");
                return defaultDataLayer;
            }
            set { defaultDataLayer = value; }
        }

        public static void RegisterSequences(IEnumerable<ITypeInfo> persistentTypes) {
            if (persistentTypes != null)
                using (var unitOfWork = new UnitOfWork(DefaultDataLayer)) {
                    var typeToExistsMap = GetTypeToExistsMap(unitOfWork);
                    foreach (ITypeInfo typeInfo in persistentTypes) {
                        if (typeToExistsMap.ContainsKey(typeInfo.FullName)) continue;
                        CreateSequenceObject(typeInfo.FullName, unitOfWork);
                    }
                    unitOfWork.CommitChanges();
                }
        }

        public static void RegisterSequences(IEnumerable<XPClassInfo> persistentClasses) {
            if (persistentClasses != null)
                using (var unitOfWork = new UnitOfWork(DefaultDataLayer)) {
                    var typeToExistsMap = GetTypeToExistsMap(unitOfWork);
                    foreach (XPClassInfo classInfo in persistentClasses) {
                        if (typeToExistsMap.ContainsKey(classInfo.FullName)) continue;
                        CreateSequenceObject(classInfo.FullName, unitOfWork);
                    }
                    unitOfWork.CommitChanges();
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

        public static void GenerateSequence(ISupportSequenceObject supportSequenceObject) {
            if (_sequenceGenerator == null)
                _sequenceGenerator = new SequenceGenerator();
            long nextSequence = _sequenceGenerator.GetNextSequence(supportSequenceObject.ClassInfo, supportSequenceObject.Prefix);
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



        public static void Initialize(string connectionString) {
            DefaultDataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
            RegisterSequences(XafTypesInfo.Instance.PersistentTypes);
        }

        public static void ReleaseSequence(ISupportSequenceObject supportSequenceObject) {
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
