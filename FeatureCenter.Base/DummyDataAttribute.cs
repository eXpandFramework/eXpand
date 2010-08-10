using System;

namespace FeatureCenter.Base {
    public class DummyDataAttribute : Attribute {
        readonly bool _exclude;

        public DummyDataAttribute(bool exclude) {
            _exclude = exclude;
        }

        public bool Exclude {
            get { return _exclude; }
        }
    }
}