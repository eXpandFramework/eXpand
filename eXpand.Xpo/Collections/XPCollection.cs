using System;
using System.Collections;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Collections
{
    public class XFPCollection<T> : XPCollection<T>
    {
        public XFPCollection(Session session, object theOwner, XPMemberInfo refProperty) : base(session, theOwner, refProperty)
        {
        }

        public XFPCollection()
        {
        }

        public XFPCollection(CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(theCriteria, sortProperties)
        {
        }

        public XFPCollection(Session session) : base(session)
        {
        }

        public XFPCollection(Session session, CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(session, theCriteria, sortProperties)
        {
        }

        public XFPCollection(Session session, bool loadingEnabled) : base(session, loadingEnabled)
        {
        }

        public XFPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }

        public XFPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }

        public XFPCollection(Session session, IEnumerable originalCollection) : base(session, originalCollection)
        {
        }

        public XFPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }

        public XFPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }

        public XFPCollection(XPBaseCollection originalCollection, CriteriaOperator filter) : base(originalCollection, filter)
        {
        }

        public XFPCollection(XPBaseCollection originalCollection, CriteriaOperator filter, bool caseSensitive) : base(originalCollection, filter, caseSensitive)
        {
        }

        public XFPCollection(XPBaseCollection originalCollection) : base(originalCollection)
        {
        }

        public XFPCollection(Session session, XPBaseCollection originalCollection) : base(session, originalCollection)
        {
        }

        public XFPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition, bool selectDeleted) : base(criteriaEvaluationBehavior, session, condition, selectDeleted)
        {
        }

        public XFPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition) : base(criteriaEvaluationBehavior, session, condition)
        {
        }
        public XFPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T,
                                                                                                                   bool>> expression)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(expression))
        {
        }
    }
}