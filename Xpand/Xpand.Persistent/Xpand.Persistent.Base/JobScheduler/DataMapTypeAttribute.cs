using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using Xpand.Xpo.Filtering;

namespace Xpand.Persistent.Base.JobScheduler {
    public abstract class DataMapTypeAttribute : Attribute {
        readonly Type _type;

        protected DataMapTypeAttribute(Type type) {
            _type = type;
        }

        public Type Type {
            get { return _type; }
        }

        public static CriteriaOperator GetCriteria<T>(Session session, Type jobType) where T : DataMapTypeAttribute {
            IEnumerable<CriteriaOperator> criteriaOperators = XafTypesInfo.Instance.FindTypeInfo(jobType).FindAttributes<T>().Select(attribute => attribute.Type.GetClassTypeFilter(session));
            return new GroupOperator(criteriaOperators.ToArray());
        }
    }

    public class JobDataMapTypeAttribute:DataMapTypeAttribute {
        public JobDataMapTypeAttribute(Type type) : base(type) {
        }
    }
    public class JobDetailDataMapTypeAttribute : DataMapTypeAttribute {
        public JobDetailDataMapTypeAttribute(Type type) : base(type) {
        }
    }
}