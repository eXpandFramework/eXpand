using System;
using System.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;

namespace Xpand.Xpo.DB {
    [Obsolete("Use SequenceGenerator", true)]
    public sealed class XpoSequencer : XPBaseObject {
        
        [Key(true)]
        public Guid Oid;
        
        [Size(254), Indexed(Unique = true)]
        public string SequencePrefix;
        public int Counter;
        public XpoSequencer(Session session) : base(session) { }
        public const int MaxIdGenerationAttempts = 7;
        public const int MinConflictDelay = 50;
        public const int MaxConflictDelay = 500;
        public static int GetNextValue(IDataLayer dataLayer, string sequencePrefix) {
            if(dataLayer == null)
                throw new ArgumentNullException("dataLayer");
            if(sequencePrefix == null)
                sequencePrefix = string.Empty;

            int attempt = 1;
            while(true) {
                try {
                    using(var generatorSession = new Session(dataLayer)) {
                        var generator =
                            generatorSession.FindObject<XpoSequencer>(new OperandProperty("SequencePrefix") ==sequencePrefix) ??
                            new XpoSequencer(generatorSession) {SequencePrefix = sequencePrefix};
                        generator.Counter++;
                        generator.Save();
                        return generator.Counter;
                    }
                }
                catch(LockingException) {
                    if(attempt >= MaxIdGenerationAttempts)
                        throw;
                }
                if(attempt > MaxIdGenerationAttempts / 2)
                    Thread.Sleep(new Random().Next(MinConflictDelay, MaxConflictDelay));

                attempt++;
            }
        }
    }
}
