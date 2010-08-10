using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace FeatureCenter.Module.Win.PropertyEditors.NullAble
{
    public class PENACustomer:BaseObject
    {
        public PENACustomer(Session session) : base(session) {
        }
        private bool? _booleanPropery;
        public bool? BooleanPropery{
            get
            {
                return _booleanPropery;
            }
            set
            {
                SetPropertyValue("BooleanPropery", ref _booleanPropery, value);
            }
        }
    }
}
