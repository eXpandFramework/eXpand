using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.Xpo.Metadata;
using ITypeInfo = DevExpress.ExpressApp.DC.ITypeInfo;

namespace Xpand.Persistent.Base.General {
    public interface ISequenceObject {
        string TypeName { get; set; }

        long NextSequence { get; set; }
    }

    public class SequenceGenerator : IDisposable {
        private readonly ExplicitUnitOfWork _explicitUnitOfWork;
        private ISequenceObject _sequence;
        static SequenceGenerator() {
            _sequenceObjectType = XafTypesInfo.Instance.FindBussinessObjectType<ISequenceObject>();
        }

        public SequenceGenerator() {
            _explicitUnitOfWork = new ExplicitUnitOfWork(DefaultDataLayer);
            
            var sequences = new XPCollection(_explicitUnitOfWork, _sequenceObjectType);
            int count = 3;
            while (true) {
                try {
                    foreach (XPBaseObject seq in sequences)
                        seq.Save();
                    _explicitUnitOfWork.FlushChanges();
                    break;
                } catch (LockingException) {
                    count--;
                    if (count <= 0)
                        throw;
                }
            }
        }
        public void Accept() {
            _explicitUnitOfWork.CommitChanges();
        }
        public long GetNextSequence(ITypeInfo typeInfo) {
            if (typeInfo == null)
                throw new ArgumentNullException("typeInfo");
            return GetNextSequence(XafTypesInfo.XpoTypeInfoSource.XPDictionary.GetClassInfo(typeInfo.Type));
        }
        public long GetNextSequence(XPClassInfo classInfo) {
            if (classInfo == null)
                throw new ArgumentNullException("classInfo");
            _sequence = (ISequenceObject) _explicitUnitOfWork.GetObjectByKey(_sequenceObjectType,classInfo.FullName, true);
            if (_sequence == null) {
                throw new InvalidOperationException(string.Format("Sequence for the {0} type was not found.", classInfo.FullName));
            }
            long nextId = _sequence.NextSequence;
            _sequence.NextSequence++;
            _explicitUnitOfWork.FlushChanges();
            return nextId;
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
                    var sequenceList = new XPCollection(unitOfWork,_sequenceObjectType);
                    var typeToExistsMap = new Dictionary<string, bool>();
                    foreach (ISequenceObject seq in sequenceList) {
                        typeToExistsMap[seq.TypeName] = true;
                    }
                    foreach (ITypeInfo typeInfo in persistentTypes) {
                        if (typeToExistsMap.ContainsKey(typeInfo.FullName)) continue;
                        CreateSequenceObject(typeInfo.FullName,unitOfWork);
                    }
                    unitOfWork.CommitChanges();
                }
        }

        static void CreateSequenceObject(string fullName, UnitOfWork unitOfWork) {
            var sequenceObject = (ISequenceObject)Activator.CreateInstance(_sequenceObjectType, new object[] { unitOfWork });
            sequenceObject.TypeName = fullName;
            sequenceObject.NextSequence = 0;
        }

        public static void RegisterSequences(IEnumerable<XPClassInfo> persistentClasses) {
            if (persistentClasses != null)
                using (var unitOfWork = new UnitOfWork(DefaultDataLayer)) {
                    var sequenceList = new XPCollection(unitOfWork,_sequenceObjectType);
                    var typeToExistsMap = new Dictionary<string, bool>();
                    foreach (ISequenceObject seq in sequenceList) {
                        typeToExistsMap[seq.TypeName] = true;
                    }
                    foreach (XPClassInfo classInfo in persistentClasses) {
                        if (typeToExistsMap.ContainsKey(classInfo.FullName)) continue;
                        CreateSequenceObject(classInfo.FullName, unitOfWork);
                    }
                    unitOfWork.CommitChanges();
                }
        }

        static SequenceGenerator _sequenceGenerator;
        public static long GenerateSequence(XPBaseObject baseObject) {
            if (_sequenceGenerator==null)
                _sequenceGenerator=new SequenceGenerator();
            long nextSequence = _sequenceGenerator.GetNextSequence(baseObject.ClassInfo);
            Session session = baseObject.Session;
            if (!(session is NestedUnitOfWork)) {
                SessionManipulationEventHandler[] sessionOnAfterCommitTransaction= {null};
                sessionOnAfterCommitTransaction[0] = (sender, args) => {
                    if (_sequenceGenerator!=null) {
                        _sequenceGenerator.Accept();
                        _sequenceGenerator.Dispose();
                        _sequenceGenerator = null;
                    }
                    session.AfterCommitTransaction-=sessionOnAfterCommitTransaction[0];
                };
                session.AfterCommitTransaction +=sessionOnAfterCommitTransaction[0];
            }
            return nextSequence;
        }

        public static void Initialize(XafApplication xafApplication) {
            DefaultDataLayer = XpoDefault.GetDataLayer(xafApplication.Connection == null ? xafApplication.ConnectionString : xafApplication.Connection.ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            RegisterSequences(XafTypesInfo.Instance.PersistentTypes);
        }
    }
}
