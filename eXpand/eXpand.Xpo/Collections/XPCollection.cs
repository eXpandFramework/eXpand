using System;
using System.Collections;
using System.Linq.Expressions;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace eXpand.Xpo.Collections
{
    public class XpCollection<T> : XPCollection<T>
    {
        public XpCollection(Session session, object theOwner, XPMemberInfo refProperty) : base(session, theOwner, refProperty)
        {
        }

        public XpCollection()
        {
        }

        public XpCollection(CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(theCriteria, sortProperties)
        {
        }

        public XpCollection(Session session) : base(session)
        {
        }

        public XpCollection(Session session, CriteriaOperator theCriteria, params SortProperty[] sortProperties) : base(session, theCriteria, sortProperties)
        {
        }

        public XpCollection(Session session, bool loadingEnabled) : base(session, loadingEnabled)
        {
        }

        public XpCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }

        public XpCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }

        public XpCollection(Session session, IEnumerable originalCollection) : base(session, originalCollection)
        {
        }

        public XpCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter, bool caseSensitive) : base(session, originalCollection, copyFilter, caseSensitive)
        {
        }

        public XpCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter) : base(session, originalCollection, copyFilter)
        {
        }

        public XpCollection(XPBaseCollection originalCollection, CriteriaOperator filter) : base(originalCollection, filter)
        {
        }

        public XpCollection(XPBaseCollection originalCollection, CriteriaOperator filter, bool caseSensitive) : base(originalCollection, filter, caseSensitive)
        {
        }

        public XpCollection(XPBaseCollection originalCollection) : base(originalCollection)
        {
        }

        public XpCollection(Session session, XPBaseCollection originalCollection) : base(session, originalCollection)
        {
        }

        public XpCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition, bool selectDeleted) : base(criteriaEvaluationBehavior, session, condition, selectDeleted)
        {
        }

        public XpCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition) : base(criteriaEvaluationBehavior, session, condition)
        {
        }
        public XpCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Expression<Func<T,
                                                                                                                   bool>> expression)
            : base(criteriaEvaluationBehavior, session, new XPQuery<T>(session).TransformExpression(expression))
        {
        }
        public XpCollection(Session session, Expression<Func<T,bool>> expression,params SortProperty[] sortProperties)
            : base(session, new XPQuery<T>(session).TransformExpression(expression),sortProperties)
        {
        }
    }
}