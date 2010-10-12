using System;

namespace Xpand.Persistent.Base.ImportExport {
    public class SerializationStrategyAttribute : Attribute {
        readonly SerializationStrategy _serializationStrategy;

        public SerializationStrategyAttribute(SerializationStrategy serializationStrategy) {
            _serializationStrategy = serializationStrategy;
        }

        public SerializationStrategy SerializationStrategy {
            get { return _serializationStrategy; }
        }
    }
}