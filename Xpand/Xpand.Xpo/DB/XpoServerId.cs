using System;
using DevExpress.Xpo;

namespace Xpand.Xpo.DB {
    public sealed class XpoServerId : XPLiteObject {
        static readonly object SyncRoot = new object();
        [Key(false)]
        public int Zero {
            get { return 0; }
            set {
                if (value != 0)
                    throw new ArgumentException("0 expected");
            }
        }
        public string SequencePrefix;
        public XpoServerId(Session session) : base(session) { }
        static string cachedSequencePrefix;
        //static WeakReference dataLayerForCachedServerPrefix = new WeakReference(null);
        static IDataLayer dataLayerForCachedServerPrefix;
        public static void ResetCache() {
            dataLayerForCachedServerPrefix = null;
            // dataLayerForCachedServerPrefix.Target = null; <<< if WeakReference
        }
        public static string GetSequencePrefix(IDataLayer dataLayer) {
            if (dataLayer == null)
                throw new ArgumentNullException("dataLayer");
            lock (SyncRoot) {
                if (dataLayerForCachedServerPrefix/*.Target*/ != dataLayer) {
                    using (var session = new Session(dataLayer)) {
                        var sid = session.GetObjectByKey<XpoServerId>(0);
                        if (sid == null) {
                            // we can throw exception here instead of creating random prefix
                            sid = new XpoServerId(session) {
                                SequencePrefix = XpoDefault.NewGuid().ToString()
                            };
                            try {
                                sid.Save();
                            } catch {
                                sid = session.GetObjectByKey<XpoServerId>(0, true);
                                if (sid == null)
                                    throw;
                            }
                        }
                        cachedSequencePrefix = sid.SequencePrefix;
                        dataLayerForCachedServerPrefix = dataLayer;
                        // dataLayerForCachedServerPrefix.Target = dataLayer; <<< if WeakReference
                    }
                }
                return cachedSequencePrefix;
            }
        }
        public static int GetNextUniqueValue(IDataLayer dataLayer, string sequencePrefix) {
            if (dataLayer == null)
                throw new ArgumentNullException("dataLayer");
            string realSeqPrefix = sequencePrefix + '@' + GetSequencePrefix(dataLayer);
            return XpoSequencer.GetNextValue(dataLayer, realSeqPrefix);
        }

        public static int GetNextUniqueValue(IXPSimpleObject simpleObject) {
            return GetNextUniqueValue(simpleObject.Session.DataLayer, simpleObject.GetType().FullName);
        }
    }
}